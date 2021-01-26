using System;
using System.IO;
using System.Net.WebSockets;
using JetBrains.Annotations;
using Project.Scripts.Runtime.Utils;
using Project.Scripts.Runtime.Vrm.Entity;
using UnityEngine;

namespace Project.Scripts.Runtime.Vrm
{
    public class ModelHumanoidMono : MonoBehaviour
    {
        [SerializeField] private string characterName;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform rootBone;
    
        private HumanPose _humanPose;
        private readonly VrmAnimationJson _ani = new VrmAnimationJson ();
        // private Animator _animator;
        
        void Start () {
            //_animator = GetComponent<Animator> ();
            LoadBone ();
        }

        void Update () {
            UpdateBone ();
        }

        private void OnDestroy () {
        }

        private void LoadBone () {
            
            for (var i = 0; i <= 54; i++) {
                var bone = animator.GetBoneTransform ((HumanBodyBones) i);
                _ani.vrmAnimation.Add (new VrmAnimationJson.VrmAnimation ());
                _ani.vrmAnimation[i].keys.Add (new VrmAnimationJson.Key ());
            }
        }

        private void UpdateBone () {
            
            var humanPoseHandler = new HumanPoseHandler (animator.avatar, rootBone);
            humanPoseHandler.GetHumanPose (ref _humanPose);

            for (var i = 0; i <= 54; i++) {
                try
                {
                    var bone = animator.GetBoneTransform ((HumanBodyBones) i);

                    var localPosition = bone.localPosition;
                    var pos = new float[3] { localPosition.x, localPosition.y, localPosition.z };
                
                    var localRotation = bone.localRotation;
                    var rot = new float[4] { localRotation.x, localRotation.y, localRotation.z, localRotation.w };
                
                    var localScale = bone.localScale;
                    var scl = new float[3] { localScale.x, localScale.y, localScale.z };
                    
                    _ani.vrmAnimation[i].name = "" + i;
                    _ani.vrmAnimation[i].bone = bone.name;
                    _ani.vrmAnimation[i].keys[0] = new VrmAnimationJson.Key (pos, rot, scl, 0);
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            
            var path = Application.dataPath + "/Temp/humanoid.json";
            new StreamWriter(path, false).Apply((it) =>
            {
                var json = JsonUtility.ToJson (_ani);
                it.WriteLine (json);
                it.Flush ();
                it.Close ();
            });
        }
    }
}