﻿using System;
using System.Runtime.Serialization;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;

// Class with relevant information about body
// bodyId and 2d and 3d points of all joints
namespace Project.Scripts.Runtime.Tracking.Entity
{
    [Serializable]
    public struct Body : ISerializable
    {
        public System.Numerics.Vector3[] JointPositions3D;
        public System.Numerics.Vector2[] JointPositions2D;
        public System.Numerics.Quaternion[] JointRotations;
        public JointConfidenceLevel[] JointPrecisions;
        
        internal int Length { get; private set; }
        internal uint Id { get; private set; }
        
        public Body(int maxJointsLength)
        {
            JointPositions3D = new System.Numerics.Vector3[maxJointsLength];
            JointPositions2D = new System.Numerics.Vector2[maxJointsLength];
            JointRotations = new System.Numerics.Quaternion[maxJointsLength];
            JointPrecisions = new JointConfidenceLevel[maxJointsLength];

            Length = 0;
            Id = 0;
        }

        public Body(SerializationInfo info, StreamingContext context)
        {
            var JointPositions3D_X = (float[])info.GetValue("JointPositions3D_X", typeof(float[]));
            var JointPositions3D_Y = (float[])info.GetValue("JointPositions3D_Y", typeof(float[]));
            var JointPositions3D_Z = (float[])info.GetValue("JointPositions3D_Z", typeof(float[]));
            JointPositions3D = new System.Numerics.Vector3[JointPositions3D_X.Length];
            for (var i = 0; i < JointPositions3D_X.Length; i++)
            {
                JointPositions3D[i].X = JointPositions3D_X[i];
                JointPositions3D[i].Y = JointPositions3D_Y[i];
                JointPositions3D[i].Z = JointPositions3D_Z[i];
            }

            var JointPositions2D_X = (float[])info.GetValue("JointPositions2D_X", typeof(float[]));
            var JointPositions2D_Y = (float[])info.GetValue("JointPositions2D_Y", typeof(float[]));
            JointPositions2D = new System.Numerics.Vector2[JointPositions2D_X.Length];
            for (var i = 0; i < JointPositions2D_X.Length; i++)
            {
                JointPositions2D[i].X = JointPositions2D_X[i];
                JointPositions2D[i].Y = JointPositions2D_Y[i];
            }

            var JointRotations_X = (float[])info.GetValue("JointRotations_X", typeof(float[]));
            var JointRotations_Y = (float[])info.GetValue("JointRotations_Y", typeof(float[]));
            var JointRotations_Z = (float[])info.GetValue("JointRotations_Z", typeof(float[]));
            var JointRotations_W = (float[])info.GetValue("JointRotations_W", typeof(float[]));
            JointRotations = new System.Numerics.Quaternion[JointRotations_X.Length];
            for (var i = 0; i < JointRotations_X.Length; i++)
            {
                JointRotations[i].X = JointRotations_X[i];
                JointRotations[i].Y = JointRotations_Y[i];
                JointRotations[i].Z = JointRotations_Z[i];
                JointRotations[i].W = JointRotations_W[i];
            }

            var ConfidenceLevel = (uint[])info.GetValue("ConfidenceLevel", typeof(uint[]));
            JointPrecisions = new JointConfidenceLevel[ConfidenceLevel.Length];
            for (var i = 0; i < ConfidenceLevel.Length; i++)
            {
                JointPrecisions[i] = (JointConfidenceLevel)ConfidenceLevel[i];
            }


            Length = (int)info.GetValue("Length", typeof(int));
            Id = (uint)info.GetValue("Id", typeof(uint));
        }
        
        public static Body DeepCopy(Body copyFromBody)
        {
            var maxJointsLength = copyFromBody.Length;
            var copiedBody = new Body(maxJointsLength);

            for (var i = 0; i < maxJointsLength; i++)
            {
                copiedBody.JointPositions2D[i] = copyFromBody.JointPositions2D[i];
                copiedBody.JointPositions3D[i] = copyFromBody.JointPositions3D[i];
                copiedBody.JointRotations[i] = copyFromBody.JointRotations[i];
                copiedBody.JointPrecisions[i] = copyFromBody.JointPrecisions[i];
            }
            copiedBody.Id = copyFromBody.Id;
            copiedBody.Length = copyFromBody.Length;
            return copiedBody;
        }

        public void CopyFromBodyTrackingSdk(Microsoft.Azure.Kinect.BodyTracking.Body body, Calibration sensorCalibration)
        {
            Id = body.Id;
            Length = Microsoft.Azure.Kinect.BodyTracking.Skeleton.JointCount;

            for (var bodyPoint = 0; bodyPoint < Length; bodyPoint++)
            {
                // K4ABT joint position unit is in millimeter. We need to convert to meters before we use the values.
                JointPositions3D[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).Position / 1000.0f;
                JointRotations[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).Quaternion;
                JointPrecisions[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).ConfidenceLevel;

                var jointPosition = JointPositions3D[bodyPoint];
                var position2d = sensorCalibration.TransformTo2D(
                    jointPosition,
                    CalibrationDeviceType.Depth,
                    CalibrationDeviceType.Depth);

                if (position2d != null)
                {
                    JointPositions2D[bodyPoint] = position2d.Value;
                }
                else
                {
                    JointPositions2D[bodyPoint].X = Constants.Invalid2DCoordinate;
                    JointPositions2D[bodyPoint].Y = Constants.Invalid2DCoordinate;
                }
            }
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var JointPositions3D_X = new float[Length];
            var JointPositions3D_Y = new float[Length];
            var JointPositions3D_Z = new float[Length];
            for (var i = 0; i < Length; i++)
            {
                JointPositions3D_X[i] = JointPositions3D[i].X;
                JointPositions3D_Y[i] = JointPositions3D[i].Y;
                JointPositions3D_Z[i] = JointPositions3D[i].Z;
            }
            info.AddValue("JointPositions3D_X", JointPositions3D_X, typeof(float[]));
            info.AddValue("JointPositions3D_Y", JointPositions3D_Y, typeof(float[]));
            info.AddValue("JointPositions3D_Z", JointPositions3D_Z, typeof(float[]));

            var JointPositions2D_X = new float[Length];
            var JointPositions2D_Y = new float[Length];
            for (var i = 0; i < Length; i++)
            {
                JointPositions2D_X[i] = JointPositions2D[i].X;
                JointPositions2D_Y[i] = JointPositions2D[i].Y;
            }
            info.AddValue("JointPositions2D_X", JointPositions2D_X, typeof(float[]));
            info.AddValue("JointPositions2D_Y", JointPositions2D_Y, typeof(float[]));

            var JointRotations_X = new float[Length];
            var JointRotations_Y = new float[Length];
            var JointRotations_Z = new float[Length];
            var JointRotations_W = new float[Length];
            for (var i = 0; i < Length; i++)
            {
                JointRotations_X[i] = JointRotations[i].X;
                JointRotations_Y[i] = JointRotations[i].Y;
                JointRotations_Z[i] = JointRotations[i].Z;
                JointRotations_W[i] = JointRotations[i].W;

            }
            info.AddValue("JointRotations_X", JointRotations_X, typeof(float[]));
            info.AddValue("JointRotations_Y", JointRotations_Y, typeof(float[]));
            info.AddValue("JointRotations_Z", JointRotations_Z, typeof(float[]));
            info.AddValue("JointRotations_W", JointRotations_W, typeof(float[]));

            var ConfidenceLevels = new uint[Length];
            for (var i = 0; i < Length; i++)
            {
                ConfidenceLevels[i] = (uint)JointPrecisions[i];
            }
            info.AddValue("ConfidenceLevels", ConfidenceLevels, typeof(uint[]));

            info.AddValue("Length", Length, typeof(int));
            info.AddValue("Id", Id, typeof(uint));
        }
    }
}

