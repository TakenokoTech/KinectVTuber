using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using Project.Scripts.Runtime.Tracking.Provider;
using UnityEngine;
using Body = Project.Scripts.Runtime.Tracking.Entity.Body;

namespace Project.Scripts.Runtime.Tracking.MonoBehaviour
{
    public class TrackerHandlerMono : UnityEngine.MonoBehaviour
    {
        Dictionary<JointId, JointId> parentJointMap;
        Dictionary<JointId, Quaternion> basisJointMap;
        Quaternion[] absoluteJointRotations = new Quaternion[(int)JointId.Count];
        public bool drawSkeletons = true;
        Quaternion Y_180_FLIP = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

        // Start is called before the first frame update
        void Awake()
        {
            parentJointMap = new Dictionary<JointId, JointId>
            {
                [JointId.Pelvis] = JointId.Count,
                [JointId.SpineNavel] = JointId.Pelvis,
                [JointId.SpineChest] = JointId.SpineNavel,
                [JointId.Neck] = JointId.SpineChest,
                [JointId.ClavicleLeft] = JointId.SpineChest,
                [JointId.ShoulderLeft] = JointId.ClavicleLeft,
                [JointId.ElbowLeft] = JointId.ShoulderLeft,
                [JointId.WristLeft] = JointId.ElbowLeft,
                [JointId.HandLeft] = JointId.WristLeft,
                [JointId.HandTipLeft] = JointId.HandLeft,
                [JointId.ThumbLeft] = JointId.HandLeft,
                [JointId.ClavicleRight] = JointId.SpineChest,
                [JointId.ShoulderRight] = JointId.ClavicleRight,
                [JointId.ElbowRight] = JointId.ShoulderRight,
                [JointId.WristRight] = JointId.ElbowRight,
                [JointId.HandRight] = JointId.WristRight,
                [JointId.HandTipRight] = JointId.HandRight,
                [JointId.ThumbRight] = JointId.HandRight,
                [JointId.HipLeft] = JointId.SpineNavel,
                [JointId.KneeLeft] = JointId.HipLeft,
                [JointId.AnkleLeft] = JointId.KneeLeft,
                [JointId.FootLeft] = JointId.AnkleLeft,
                [JointId.HipRight] = JointId.SpineNavel,
                [JointId.KneeRight] = JointId.HipRight,
                [JointId.AnkleRight] = JointId.KneeRight,
                [JointId.FootRight] = JointId.AnkleRight,
                [JointId.Head] = JointId.Pelvis,
                [JointId.Nose] = JointId.Head,
                [JointId.EyeLeft] = JointId.Head,
                [JointId.EarLeft] = JointId.Head,
                [JointId.EyeRight] = JointId.Head,
                [JointId.EarRight] = JointId.Head
            };

            // pelvis has no parent so set to count

            var zpositive = Vector3.forward;
            var xpositive = Vector3.right;
            var ypositive = Vector3.up;
            // spine and left hip are the same
            var leftHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
            var spineHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
            var rightHipBasis = Quaternion.LookRotation(xpositive, zpositive);
            // arms and thumbs share the same basis
            var leftArmBasis = Quaternion.LookRotation(ypositive, -zpositive);
            var rightArmBasis = Quaternion.LookRotation(-ypositive, zpositive);
            var leftHandBasis = Quaternion.LookRotation(-zpositive, -ypositive);
            var rightHandBasis = Quaternion.identity;
            var leftFootBasis = Quaternion.LookRotation(xpositive, ypositive);
            var rightFootBasis = Quaternion.LookRotation(xpositive, -ypositive);

            basisJointMap = new Dictionary<JointId, Quaternion>
            {
                [JointId.Pelvis] = spineHipBasis,
                [JointId.SpineNavel] = spineHipBasis,
                [JointId.SpineChest] = spineHipBasis,
                [JointId.Neck] = spineHipBasis,
                [JointId.ClavicleLeft] = leftArmBasis,
                [JointId.ShoulderLeft] = leftArmBasis,
                [JointId.ElbowLeft] = leftArmBasis,
                [JointId.WristLeft] = leftHandBasis,
                [JointId.HandLeft] = leftHandBasis,
                [JointId.HandTipLeft] = leftHandBasis,
                [JointId.ThumbLeft] = leftArmBasis,
                [JointId.ClavicleRight] = rightArmBasis,
                [JointId.ShoulderRight] = rightArmBasis,
                [JointId.ElbowRight] = rightArmBasis,
                [JointId.WristRight] = rightHandBasis,
                [JointId.HandRight] = rightHandBasis,
                [JointId.HandTipRight] = rightHandBasis,
                [JointId.ThumbRight] = rightArmBasis,
                [JointId.HipLeft] = leftHipBasis,
                [JointId.KneeLeft] = leftHipBasis,
                [JointId.AnkleLeft] = leftHipBasis,
                [JointId.FootLeft] = leftFootBasis,
                [JointId.HipRight] = rightHipBasis,
                [JointId.KneeRight] = rightHipBasis,
                [JointId.AnkleRight] = rightHipBasis,
                [JointId.FootRight] = rightFootBasis,
                [JointId.Head] = spineHipBasis,
                [JointId.Nose] = spineHipBasis,
                [JointId.EyeLeft] = spineHipBasis,
                [JointId.EarLeft] = spineHipBasis,
                [JointId.EyeRight] = spineHipBasis,
                [JointId.EarRight] = spineHipBasis
            };
        }

        public void updateTracker(BackgroundData trackerFrameData)
        {
            var closestBody = findClosestTrackedBody(trackerFrameData);
            var skeleton = trackerFrameData.Bodies[closestBody];
            renderSkeleton(skeleton, 0);
        }

        int findIndexFromId(BackgroundData frameData, int id)
        {
            var retIndex = -1;
            for (var i = 0; i < (int)frameData.NumOfBodies; i++)
            {
                if ((int) frameData.Bodies[i].Id != id) continue;
                retIndex = i;
                break;
            }
            return retIndex;
        }

        private int findClosestTrackedBody(BackgroundData trackerFrameData)
        {
            var closestBody = -1;
            const float MAX_DISTANCE = 5000.0f;
            var minDistanceFromKinect = MAX_DISTANCE;
            for (var i = 0; i < (int)trackerFrameData.NumOfBodies; i++)
            {
                var pelvisPosition = trackerFrameData.Bodies[i].JointPositions3D[(int)JointId.Pelvis];
                var pelvisPos = new Vector3(pelvisPosition.X, pelvisPosition.Y, pelvisPosition.Z);
                if (!(pelvisPos.magnitude < minDistanceFromKinect)) continue;
                closestBody = i;
                minDistanceFromKinect = pelvisPos.magnitude;
            }
            return closestBody;
        }

        public void turnOnOffSkeletons()
        {
            drawSkeletons = !drawSkeletons;
            const int bodyRenderedNum = 0;
            for (var jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
            {
                transform.GetChild(bodyRenderedNum).GetChild(jointNum).gameObject.GetComponent<MeshRenderer>().enabled = drawSkeletons;
                transform.GetChild(bodyRenderedNum).GetChild(jointNum).GetChild(0).GetComponent<MeshRenderer>().enabled = drawSkeletons;
            }
        }

        public void renderSkeleton(Body skeleton, int skeletonNumber)
        {
            for (var jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
            {
                var jointPos = new Vector3(skeleton.JointPositions3D[jointNum].X, -skeleton.JointPositions3D[jointNum].Y, skeleton.JointPositions3D[jointNum].Z);
                var offsetPosition = transform.rotation * jointPos;
                var positionInTrackerRootSpace = transform.position + offsetPosition;
                var jointRot = Y_180_FLIP * new Quaternion(skeleton.JointRotations[jointNum].X, skeleton.JointRotations[jointNum].Y,
                    skeleton.JointRotations[jointNum].Z, skeleton.JointRotations[jointNum].W) * Quaternion.Inverse(basisJointMap[(JointId)jointNum]);
                absoluteJointRotations[jointNum] = jointRot;
                // these are absolute body space because each joint has the body root for a parent in the scene graph
                transform.GetChild(skeletonNumber).GetChild(jointNum).localPosition = jointPos;
                transform.GetChild(skeletonNumber).GetChild(jointNum).localRotation = jointRot;

                const int boneChildNum = 0;
                if (parentJointMap[(JointId)jointNum] != JointId.Head && parentJointMap[(JointId)jointNum] != JointId.Count)
                {
                    var parentTrackerSpacePosition = new Vector3(
                        skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].X,
                        -skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Y,
                        skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Z);
                    var boneDirectionTrackerSpace = jointPos - parentTrackerSpacePosition;
                    var boneDirectionWorldSpace = transform.rotation * boneDirectionTrackerSpace;
                    var boneDirectionLocalSpace = Quaternion.Inverse(transform.GetChild(skeletonNumber).GetChild(jointNum).rotation) * Vector3.Normalize(boneDirectionWorldSpace);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localScale = new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localRotation = Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position = transform.GetChild(skeletonNumber).GetChild(jointNum).position - 0.5f * boneDirectionWorldSpace;
                }
                else
                {
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).gameObject.SetActive(false);
                }
            }
        }

        public Quaternion GetRelativeJointRotation(JointId jointId)
        {
            var parent = parentJointMap[jointId];
            var parentJointRotationBodySpace = absoluteJointRotations[(int)parent];
            
            if (parent == JointId.Count) parentJointRotationBodySpace = Y_180_FLIP;
                         
            var jointRotationBodySpace = absoluteJointRotations[(int)jointId];
            var relativeRotation =  Quaternion.Inverse(parentJointRotationBodySpace) * jointRotationBodySpace;

            return relativeRotation;
        }

    }
}
