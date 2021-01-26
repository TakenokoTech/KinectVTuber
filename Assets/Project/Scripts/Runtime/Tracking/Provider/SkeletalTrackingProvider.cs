using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using Project.Scripts.Runtime.Tracking.MonoBehaviour;

namespace Project.Scripts.Runtime.Tracking.Provider
{
    public class SkeletalTrackingProvider : BackgroundDataProvider
    {
        bool readFirstFrame = false;
        TimeSpan initialTimestamp;

        BinaryFormatter binaryFormatter { get; set; } = new BinaryFormatter();

        public Stream RawDataLoggingFile = null;

        protected override void RunBackgroundThreadAsync(int id)
        {
            try
            {
                UnityEngine.Debug.Log("Starting body tracker background thread.");

                // Buffer allocations.
                var currentFrameData = new BackgroundData();
                // Open device.
                using (var device = Device.Open(id))
                {
                    device.StartCameras(new DeviceConfiguration()
                    {
                        CameraFPS = FPS.FPS30,
                        ColorResolution = ColorResolution.Off,
                        DepthMode = DepthMode.NFOV_Unbinned,
                        WiredSyncMode = WiredSyncMode.Standalone,
                    });

                    UnityEngine.Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum);

                    var deviceCalibration = device.GetCalibration();
                    var configuration = new TrackerConfiguration
                        {ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default};

                    using (var tracker = Tracker.Create(deviceCalibration, configuration))
                    {
                        UnityEngine.Debug.Log("Body tracker created.");
                        while (RunBackgroundThread)
                        {
                            using (var sensorCapture = device.GetCapture())
                                tracker.EnqueueCapture(sensorCapture);

                            using (var frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                            {
                                if (frame == null)
                                {
                                    UnityEngine.Debug.Log("Pop result from tracker timeout!");
                                }
                                else
                                {
                                    IsRunning = true;
                                    // Get number of bodies in the current frame.
                                    currentFrameData.NumOfBodies = frame.NumberOfBodies;

                                    // Copy bodies.
                                    for (uint i = 0; i < currentFrameData.NumOfBodies; i++)
                                    {
                                        currentFrameData.Bodies[i]
                                            .CopyFromBodyTrackingSdk(frame.GetBody(i), deviceCalibration);
                                    }

                                    // Store depth image.
                                    var bodyFrameCapture = frame.Capture;
                                    var depthImage = bodyFrameCapture.Depth;
                                    if (!readFirstFrame)
                                    {
                                        readFirstFrame = true;
                                        initialTimestamp = depthImage.DeviceTimestamp;
                                    }

                                    currentFrameData.TimestampInMs =
                                        (float) (depthImage.DeviceTimestamp - initialTimestamp).TotalMilliseconds;
                                    currentFrameData.DepthImageWidth = depthImage.WidthPixels;
                                    currentFrameData.DepthImageHeight = depthImage.HeightPixels;

                                    // Read image data from the SDK.
                                    var depthFrame = MemoryMarshal.Cast<byte, ushort>(depthImage.Memory.Span);

                                    // Repack data and store image data.
                                    var byteCounter = 0;
                                    currentFrameData.DepthImageSize =
                                        currentFrameData.DepthImageWidth * currentFrameData.DepthImageHeight * 3;

                                    for (var it = currentFrameData.DepthImageWidth * currentFrameData.DepthImageHeight -
                                                  1;
                                        it > 0;
                                        it--)
                                    {
                                        var b = (byte) (depthFrame[it] / (ConfigLoaderMono.Instance.Configs.SkeletalTracking.MaximumDisplayedDepthInMillimeters) * 255);
                                        currentFrameData.DepthImage[byteCounter++] = b;
                                        currentFrameData.DepthImage[byteCounter++] = b;
                                        currentFrameData.DepthImage[byteCounter++] = b;
                                    }

                                    if (RawDataLoggingFile != null && RawDataLoggingFile.CanWrite)
                                        binaryFormatter.Serialize(RawDataLoggingFile, currentFrameData);

                                    // Update data variable that is being read in the UI thread.
                                    SetCurrentFrameData(ref currentFrameData);
                                }
                            }
                        }

                        tracker.Dispose();
                    }

                    device.Dispose();
                }

                RawDataLoggingFile?.Close();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }
    }
}