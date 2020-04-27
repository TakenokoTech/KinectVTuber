using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using Project.Scripts.Runtime.Tracking.MonoBehaviour;
using Project.Scripts.Runtime.Utils;
using UnityEngine;

namespace Project.Scripts.Runtime.Vrm
{
    public class KinectHumanoidMono : MonoBehaviour
    {
        private const string Tag = "KinectHumanoidMono";

        [SerializeField] private TrackerHandlerMono trackerHandlerMono;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject pelvis;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            foreach (var bone in _boneMap)
            {
                try
                {
                    var rot = trackerHandlerMono.GetRelativeJointRotation(bone.Value);
                    animator.GetBoneTransform(bone.Key).localRotation = new Quaternion(rot.x, rot.y, rot.z, rot.w);
                }
                catch (Exception exception)
                {
                    Log.D(Tag, $"{bone}: {exception.Message}");
                }
            }
            
            transform.localPosition = pelvis.transform.localPosition;
            transform.localRotation = pelvis.transform.localRotation;
        }

        private readonly Dictionary<HumanBodyBones, JointId> _boneMap = new Dictionary<HumanBodyBones, JointId>()
        {
            {HumanBodyBones.Hips, JointId.SpineNavel},
            {HumanBodyBones.LeftUpperLeg, JointId.HipLeft},
            {HumanBodyBones.RightUpperLeg, JointId.HipRight},
            {HumanBodyBones.LeftLowerLeg, JointId.KneeLeft},
            {HumanBodyBones.RightLowerLeg, JointId.KneeRight},
            {HumanBodyBones.LeftFoot, JointId.FootLeft},
            {HumanBodyBones.RightFoot, JointId.FootRight},
            {HumanBodyBones.Spine, JointId.SpineNavel},
            {HumanBodyBones.Chest, JointId.SpineChest},
            //// {HumanBodyBones.UpperChest, JointId.SpineChest},
            {HumanBodyBones.Neck, JointId.Neck},
            {HumanBodyBones.Head, JointId.Head},
            {HumanBodyBones.LeftShoulder, JointId.ClavicleLeft},
            {HumanBodyBones.RightShoulder, JointId.ClavicleRight},
            {HumanBodyBones.LeftUpperArm, JointId.ShoulderLeft},
            {HumanBodyBones.RightUpperArm, JointId.ShoulderRight},
            {HumanBodyBones.LeftLowerArm, JointId.ElbowLeft},
            {HumanBodyBones.RightLowerArm, JointId.ElbowRight},
            {HumanBodyBones.LeftHand, JointId.HandLeft},
            {HumanBodyBones.RightHand, JointId.HandRight},
            {HumanBodyBones.LeftToes, JointId.FootLeft},
            {HumanBodyBones.RightToes, JointId.FootRight},
            // {HumanBodyBones.LeftEye, JointId.EyeLeft},
            // {HumanBodyBones.RightEye, JointId.EyeRight},
            // {HumanBodyBones.Jaw, JointId.Nose},
            // {HumanBodyBones.LeftThumbProximal, JointId.Count},
            // {HumanBodyBones.LeftThumbIntermediate, JointId.Count},
            // {HumanBodyBones.LeftThumbDistal, JointId.Count},
            // {HumanBodyBones.LeftIndexProximal, JointId.Count},
            // {HumanBodyBones.LeftIndexIntermediate, JointId.Count},
            // {HumanBodyBones.LeftIndexDistal, JointId.Count},
            // {HumanBodyBones.LeftMiddleProximal, JointId.Count},
            // {HumanBodyBones.LeftMiddleIntermediate, JointId.Count},
            // {HumanBodyBones.LeftMiddleDistal, JointId.Count},
            // {HumanBodyBones.LeftRingProximal, JointId.Count},
            // {HumanBodyBones.LeftRingIntermediate, JointId.Count},
            // {HumanBodyBones.LeftRingDistal, JointId.Count},
            // {HumanBodyBones.LeftLittleProximal, JointId.Count},
            // {HumanBodyBones.LeftLittleIntermediate, JointId.Count},
            // {HumanBodyBones.LeftLittleDistal, JointId.Count},
            // {HumanBodyBones.RightThumbProximal, JointId.Count},
            // {HumanBodyBones.RightThumbIntermediate, JointId.Count},
            // {HumanBodyBones.RightThumbDistal, JointId.Count},
            // {HumanBodyBones.RightIndexProximal, JointId.Count},
            // {HumanBodyBones.RightIndexIntermediate, JointId.Count},
            // {HumanBodyBones.RightIndexDistal, JointId.Count},
            // {HumanBodyBones.RightMiddleProximal, JointId.Count},
            // {HumanBodyBones.RightMiddleIntermediate, JointId.Count},
            // {HumanBodyBones.RightMiddleDistal, JointId.Count},
            // {HumanBodyBones.RightRingProximal, JointId.Count},
            // {HumanBodyBones.RightRingIntermediate, JointId.Count},
            // {HumanBodyBones.RightRingDistal, JointId.Count},
            // {HumanBodyBones.RightLittleProximal, JointId.Count},
            // {HumanBodyBones.RightLittleIntermediate, JointId.Count},
            // {HumanBodyBones.RightLittleDistal, JointId.Count},
        };
    }
}