﻿using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;
using Body = Project.Scripts.Runtime.Tracking.Entity.Body;

namespace Project.Scripts.Runtime.Tracking.Skeleton
{
    [System.Serializable]
    public class SkeletonPosition
    {
        public SkeletonPosition(
            Body body,
            List<JointId> jointsMapper,
            Vector3 referentCameraPosition)
        {
            foreach (var entry in jointsMapper)
            {
                var point = body.JointPositions3D[(int)entry];
                var newDelta = new Vector3(point.X, -point.Y, point.Z);
                currentJointPositions[entry] = referentCameraPosition + newDelta;
            }
        }

        public SkeletonPosition()
        {
        }

        public float Timestamp { get; set; }

        public Dictionary<JointId, Vector3> currentJointPositions { get; set; } = new Dictionary<JointId, Vector3>();

        #region overriden operators

        // Add two skeleton positions to create new.
        public static SkeletonPosition operator +(SkeletonPosition b, SkeletonPosition c)
        {
            var a = new SkeletonPosition();
            foreach (var key in b.currentJointPositions.Keys)
            {
                a.currentJointPositions.Add(key, b.currentJointPositions[key] + c.currentJointPositions[key]);
            }
            return a;
        }

        // Subtract two skeleton positions to create new.
        public static SkeletonPosition operator -(SkeletonPosition b, SkeletonPosition c)
        {
            var a = new SkeletonPosition();
            foreach (var key in b.currentJointPositions.Keys)
            {
                a.currentJointPositions.Add(key, b.currentJointPositions[key] - c.currentJointPositions[key]);
            }
            return a;
        }

        // Divide operator, used to find average value for given sequence.
        public static SkeletonPosition operator /(SkeletonPosition lhs, float rhs)
        {
            var a = new SkeletonPosition();
            foreach (var key in lhs.currentJointPositions.Keys)
            {
                a.currentJointPositions.Add(key, lhs.currentJointPositions[key] / rhs);
            }
            return a;
        }

        #endregion
    }
}
