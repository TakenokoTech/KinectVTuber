using UnityEngine;

namespace Project.Scripts.Runtime.Vrm
{
    public class PersonCamera : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private GameObject charcter;

        private Transform _transform;
        
        void Start()
        {
            _transform = transform;   
        }

        void Update()
        {
            // Quaternion move_rotation = Quaternion.LookRotation(charcter.transform.position - transform.position, Vector3.up);
            // this.transform.rotation = move_rotation;
            
            var position = charcter.transform.position;
            _transform.localPosition = position;

            var rot = charcter.transform.rotation;
            _transform.localRotation = new Quaternion(0, rot.y, 0, rot.w);
        }
    }
}
