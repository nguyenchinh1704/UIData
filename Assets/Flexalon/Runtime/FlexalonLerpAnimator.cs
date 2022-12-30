using UnityEngine;

namespace Flexalon
{
    [HelpURL("https://www.flexalon.com/docs/animators")]
    public class FlexalonLerpAnimator : MonoBehaviour, TransformUpdater
    {
        private FlexalonNode _node;

        [SerializeField]
        private float _interpolationSpeed = 5.0f;
        public float InterpolationSpeed
        {
            get => _interpolationSpeed;
            set { _interpolationSpeed = value; }
        }

        [SerializeField]
        private bool _animatePosition = true;
        public bool AnimatePosition
        {
            get => _animatePosition;
            set { _animatePosition = value; }
        }

        [SerializeField]
        private bool _animateRotation = true;
        public bool AnimateRotation
        {
            get => _animateRotation;
            set { _animateRotation = value; }
        }

        [SerializeField]
        private bool _animateScale = true;
        public bool AnimateScale
        {
            get => _animateScale;
            set { _animateScale = value; }
        }

        void OnEnable()
        {
            _node = Flexalon.GetOrCreateNode(gameObject);
            _node.SetTransformUpdater(this);
        }

        void OnDisable()
        {
            _node?.SetTransformUpdater(null);
            _node = null;
        }

        public bool UpdatePosition(FlexalonNode node, Vector3 position)
        {
            if (!_animatePosition || Vector3.Distance(transform.localPosition, position) < 0.02f)
            {
                transform.localPosition = position;
                return true;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, position, _interpolationSpeed * Time.smoothDeltaTime);
                return false;
            }
        }

        public bool UpdateRotation(FlexalonNode node, Quaternion rotation)
        {

            if (!_animateRotation || Mathf.Abs(Quaternion.Angle(transform.localRotation, rotation)) < 0.02f)
            {
                transform.localRotation = rotation;
                return true;
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, rotation, _interpolationSpeed * Time.smoothDeltaTime);
                return false;
            }
        }

        public bool UpdateScale(FlexalonNode node, Vector3 scale)
        {
            if (!_animateScale || Vector3.Distance(transform.localScale, scale) < 0.02f)
            {
                transform.localScale = scale;
                return true;
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, scale, _interpolationSpeed * Time.smoothDeltaTime);
                return false;
            }
        }
    }
}