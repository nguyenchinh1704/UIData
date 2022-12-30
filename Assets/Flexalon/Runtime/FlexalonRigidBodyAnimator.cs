using UnityEngine;

namespace Flexalon
{
    [HelpURL("https://www.flexalon.com/docs/animators")]
    public class FlexalonRigidBodyAnimator : MonoBehaviour, TransformUpdater
    {
        private FlexalonNode _node;
        private Rigidbody _rigidBody;

        [SerializeField]
        private float _positionForce = 5.0f;
        public float PositionForce
        {
            get => _positionForce;
            set { _positionForce = value; }
        }

        [SerializeField]
        private float _rotationForce = 5.0f;
        public float RotationForce
        {
            get => _rotationForce;
            set { _rotationForce = value; }
        }

        [SerializeField]
        private float _scaleInterpolationSpeed = 5.0f;
        public float ScaleInterpolationSpeed
        {
            get => _scaleInterpolationSpeed;
            set { _scaleInterpolationSpeed = value; }
        }

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;

        void OnEnable()
        {
            _node = Flexalon.GetOrCreateNode(gameObject);
            _node.SetTransformUpdater(this);
            _rigidBody = GetComponent<Rigidbody>();
            _targetPosition = transform.localPosition;
            _targetRotation = transform.localRotation;
        }

        void OnDisable()
        {
            _node.SetTransformUpdater(null);
            _node = null;
        }

        public bool UpdatePosition(FlexalonNode node, Vector3 position)
        {
            if (_rigidBody)
            {
                _targetPosition = position;
                return false;
            }
            else
            {
                transform.localPosition = position;
                return true;
            }
        }

        public bool UpdateRotation(FlexalonNode node, Quaternion rotation)
        {
            if (_rigidBody)
            {
                _targetRotation = rotation;
                return false;
            }
            else
            {
                transform.localRotation = rotation;
                return true;
            }
        }

        public bool UpdateScale(FlexalonNode node, Vector3 scale)
        {
            if (Vector3.Distance(transform.localScale, scale) < 0.01f)
            {
                transform.localScale = scale;
                return true;
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, scale, _scaleInterpolationSpeed * Time.deltaTime);
                return false;
            }
        }

            void FixedUpdate()
            {
                if (_rigidBody)
                {
                    var worldPos = transform.parent?.localToWorldMatrix.MultiplyPoint(_targetPosition) ?? _targetPosition;
                    _rigidBody.AddForce((worldPos - transform.position) * _positionForce, ForceMode.Force);

                    var rot = Quaternion.Slerp(transform.localRotation, _targetRotation, _rotationForce * Time.deltaTime);
                    var rotWorldSpace = (transform.parent?.rotation ?? Quaternion.identity) * rot;
                    _rigidBody.MoveRotation(rotWorldSpace);
                }
            }
    }
}