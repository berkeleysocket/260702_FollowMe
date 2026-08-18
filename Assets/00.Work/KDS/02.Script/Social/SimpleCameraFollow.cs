using UnityEngine;

namespace FollowMe.KDS
{
    public class SimpleCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 1.5f, -10f);
        [SerializeField] private float _smooth = 8f;
        [SerializeField] private float _minX = -2f;
        [SerializeField] private float _maxX = 125f;

        public void SetTarget(Transform target) => _target = target;

        private void LateUpdate()
        {
            if (_target == null) return;
            Vector3 desired = _target.position + _offset;
            desired.x = Mathf.Clamp(desired.x, _minX, _maxX);
            desired.z = _offset.z;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-_smooth * Time.deltaTime));
        }
    }
}
