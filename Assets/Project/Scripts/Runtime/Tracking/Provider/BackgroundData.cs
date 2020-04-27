using System;
using System.Runtime.Serialization;
using Project.Scripts.Runtime.Tracking.Entity;

// Class which contains all data sent from background thread to main thread.
namespace Project.Scripts.Runtime.Tracking.Provider
{
    [Serializable]
    public class BackgroundData : ISerializable
    {
        public float TimestampInMs { get; set; }
        public byte[] DepthImage { get; set; }
        public int DepthImageWidth { get; set; }
        public int DepthImageHeight { get; set; }
        public int DepthImageSize { get; set; }
        public ulong NumOfBodies { get; set; }
        public Body[] Bodies { get; set; }

        public BackgroundData(int maxDepthImageSize = 1024 * 1024 * 3, int maxBodiesCount = 20, int maxJointsSize = 100)
        {
            DepthImage = new byte[maxDepthImageSize];
            Bodies = new Body[maxBodiesCount];
            for (var i = 0; i < maxBodiesCount; i++) Bodies[i] = new Body(maxJointsSize);
        }

        public BackgroundData(SerializationInfo info, StreamingContext context)
        {
            TimestampInMs = (float) info.GetValue("TimestampInMs", typeof(float));
            DepthImageWidth = (int) info.GetValue("DepthImageWidth", typeof(int));
            DepthImageHeight = (int) info.GetValue("DepthImageHeight", typeof(int));
            DepthImageSize = (int) info.GetValue("DepthImageSize", typeof(int));
            NumOfBodies = (ulong) info.GetValue("NumOfBodies", typeof(ulong));
            Bodies = (Body[]) info.GetValue("Bodies", typeof(Body[]));
            DepthImage = (byte[]) info.GetValue("DepthImage", typeof(byte[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Writing only relevant data to serialized stream, without the placeholder data
            // (the real depthimage size is not maxdepthimagesize, but smaller).
            info.AddValue("TimestampInMs", TimestampInMs, typeof(float));
            info.AddValue("DepthImageWidth", DepthImageWidth, typeof(int));
            info.AddValue("DepthImageHeight", DepthImageHeight, typeof(int));
            info.AddValue("DepthImageSize", DepthImageSize, typeof(int));
            info.AddValue("NumOfBodies", NumOfBodies, typeof(ulong));

            var validBodies = new Body[NumOfBodies];
            for (var i = 0; i < (int) NumOfBodies; i++) validBodies[i] = Bodies[i];
            info.AddValue("Bodies", validBodies, typeof(Body[]));

            var validDepthImage = new byte[DepthImageSize];
            for (var i = 0; i < DepthImageSize; i++) validDepthImage[i] = DepthImage[i];
            info.AddValue("DepthImage", validDepthImage, typeof(byte[]));
        }
    }
}
