using UnityEngine;

namespace Project.Scripts.Runtime.Tracking.Skeleton
{
    public class CameraFlipper : UnityEngine.MonoBehaviour
    {
        private Camera _camera;

        [Tooltip("Flip by x axis")]
        public bool flipByX;

        void Start()
        {
            _camera = GetComponent<Camera>();
        }
        
        void OnPreCull()
        {
            if (!flipByX) return;
            
            _camera.ResetWorldToCameraMatrix();
            _camera.ResetProjectionMatrix();
            _camera.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
        }

        void OnPreRender()
        {
            if (flipByX) GL.invertCulling = true;
        }

        void OnPostRender()
        {
            if (flipByX) GL.invertCulling = false;
        }
    }
}
