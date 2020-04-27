using System;
using Microsoft.Azure.Kinect.BodyTracking;
using Project.Scripts.Runtime.Utils;
using UnityEngine;
using RootMotion.FinalIK;

namespace Project.Scripts.Runtime.Vrm
{
    public class BodyTrackingMono : MonoBehaviour
    {
        private const string Tag = "BodyTrackingMono";

        [SerializeField] private GameObject model;
        
        [SerializeField] private GameObject head;
        [SerializeField] private GameObject spineChest;
        [SerializeField] private GameObject pelvis;
        [SerializeField] private GameObject elbowRight;
        [SerializeField] private GameObject wristRight;
        [SerializeField] private GameObject elbowLeft;
        [SerializeField] private GameObject wristLeft;
        [SerializeField] private GameObject kneeRight;
        [SerializeField] private GameObject footRight;
        [SerializeField] private GameObject kneeLeft;
        [SerializeField] private GameObject footLeft;
        
        [SerializeField] private GameObject targetHead;
        [SerializeField] private GameObject targetChest;
        [SerializeField] private GameObject targetPelvis;
        [SerializeField] private GameObject targetRightForearm;
        [SerializeField] private GameObject targetRightArm;
        [SerializeField] private GameObject targetLeftForearm;
        [SerializeField] private GameObject targetLeftArm;
        [SerializeField] private GameObject targetRightCalf;
        [SerializeField] private GameObject targetRightLeg;
        [SerializeField] private GameObject targetLeftCalf;
        [SerializeField] private GameObject targetLeftLeg;

        [SerializeField] private bool enableMark = false; 
        [SerializeField] private float xzScale;
        [SerializeField] private float yScale;

        private const int JOINT_NUM = 11;
        private readonly Vector3[] _jointOffset = new Vector3[JOINT_NUM];

        private Transform[] trackPoints;
        private GameObject[] debugSphere;

        private Vector3 _offset = Vector3.zero;
        private VRIK _vrik;
        private Transform[] _vrikJoints;
        private float modelWidth, modelHeight;

        private void Awake()
        {
            trackPoints = new[]
            {
                null,
                targetHead.transform,
                targetChest.transform,
                targetPelvis.transform,
                targetRightForearm.transform,
                targetRightArm.transform,
                targetLeftForearm.transform,
                targetLeftArm.transform,
                targetRightCalf.transform,
                targetRightLeg.transform,
                targetLeftCalf.transform,
                targetLeftLeg.transform,
            };
            _vrik = model.GetComponent<VRIK>();

            debugSphere = new GameObject[JOINT_NUM];
            for (var i = 0; i < JOINT_NUM; i++)
            {
                if (enableMark)
                {
                    debugSphere[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    debugSphere[i].transform.SetParent(trackPoints[i + 1].transform);
                    debugSphere[i].transform.localScale = Vector3.one * 0.05f;
                }
            }

            _vrikJoints = new Transform[JOINT_NUM];
            _vrikJoints[0] = _vrik.references.head;
            _vrikJoints[1] = _vrik.references.chest;
            _vrikJoints[2] = _vrik.references.pelvis;
            _vrikJoints[3] = _vrik.references.rightForearm;
            _vrikJoints[4] = _vrik.references.rightHand;
            _vrikJoints[5] = _vrik.references.leftForearm;
            _vrikJoints[6] = _vrik.references.leftHand;
            _vrikJoints[7] = _vrik.references.rightCalf;
            _vrikJoints[8] = _vrik.references.rightFoot;
            _vrikJoints[9] = _vrik.references.leftCalf;
            _vrikJoints[10] = _vrik.references.leftFoot;
        }

        void Start()
        {
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Log.D(Tag, "Input.GetMouseButtonDown(0)", "");
                Calibrate();
            }

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            model.transform.localScale = new Vector3(xzScale, yScale, xzScale);
            var objs = new GameObject[JOINT_NUM]
            {
                head, spineChest, pelvis, elbowRight, wristRight, elbowLeft, wristLeft, kneeRight, footRight, kneeLeft,
                footLeft
            };
            for (var i = 0; i < objs.Length; i++)
            {
                //Vector3 pos = 0.001f * joint.ToVector3();
                var pos = objs[i].transform.position;
                var rot = objs[i].transform.rotation;
                trackPoints[i + 1].localPosition = new Vector3(pos.x, pos.y, pos.z);
                trackPoints[i + 1].localRotation = new Quaternion(rot.x, rot.y, rot.z, rot.w);
                trackPoints[i + 1].position += _jointOffset[i];
            }
        }

        private void Calibrate()
        {
            // オフセット・スケールの初期化
            _offset = Vector3.zero;
            transform.localScale = Vector3.one;

            // スケールの調整
            modelWidth = Vector3.Distance(wristLeft.transform.position, wristRight.transform.position);
            var playerWidth = Vector3.Distance(trackPoints[5].position, trackPoints[7].position);
            xzScale = modelWidth / playerWidth;
            Log.D(Tag, "[width] model: {0}, player: {1}, diff: {2}", modelWidth, playerWidth, modelWidth - playerWidth);
            
            modelHeight = head.transform.position.y;
            var playerHeight = trackPoints[1].position.y - trackPoints[9].position.y;
            yScale = modelHeight / playerHeight;
            Log.D(Tag, "[height] model: {0}, player: {1}, diff: {2}", modelHeight, playerHeight, modelHeight - playerHeight);
            
            // transform.position = _vrik.references.head.position - trackPoints[1].position;
            // for (var i = 0; i < JOINT_NUM; i++) _jointOffset[i] = _vrikJoints[i].position - trackPoints[i + 1].position;

            // VRIKに各ポイントをアタッチ
            // _vrik.solver.spine.pelvisTarget = trackPoints[3];
            // _vrik.solver.leftLeg.target = trackPoints[11];
            // _vrik.solver.rightLeg.target = trackPoints[9];
        }
    }
}