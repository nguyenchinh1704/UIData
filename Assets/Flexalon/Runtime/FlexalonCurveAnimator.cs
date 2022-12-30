using UnityEngine;

namespace Flexalon
{
    [HelpURL("https://www.flexalon.com/docs/animators")]
    public class FlexalonCurveAnimator : MonoBehaviour, TransformUpdater
    {
        private FlexalonNode _node;

        [SerializeField]
        private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve Curve
        {
            get => _curve;
            set { _curve = value; }
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

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Vector3 _startScale;

        private Vector3 _endPosition;
        private Quaternion _endRotation;
        private Vector3 _endScale;

        private float _positionTime;
        private float _rotationTime;
        private float _scaleTime;

        void OnEnable()
        {
            _node = Flexalon.GetOrCreateNode(gameObject);
            _node.SetTransformUpdater(this);
        }

        void OnDisable()
        {
            _node?.SetTransformUpdater(null);
            _node = null;

            _startPosition = _endPosition = transform.localPosition;
            _startRotation = _endRotation = transform.localRotation;
            _startScale = _endScale = transform.localScale;
            _positionTime = _rotationTime = _scaleTime = 0;
        }

        public bool UpdatePosition(FlexalonNode node, Vector3 position)
        {
            if (position != _endPosition)
            {
                _startPosition = transform.localPosition;
                _endPosition = position;
                _positionTime = 0;
            }

            _positionTime += Time.smoothDeltaTime;

            if (!_animatePosition || _positionTime > _curve.keys[_curve.keys.Length - 1].time)
            {
                transform.localPosition = position;
                return true;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, _curve.Evaluate(_positionTime));
                return false;
            }
        }

        public bool UpdateRotation(FlexalonNode node, Quaternion rotation)
        {
            if (rotation != _endRotation)
            {
                _startRotation = transform.localRotation;
                _endRotation = rotation;
                _rotationTime = 0;
            }

            _rotationTime += Time.smoothDeltaTime;

            if (!_animateRotation || _rotationTime > _curve.keys[_curve.keys.Length - 1].time)
            {
                transform.localRotation = rotation;
                return true;
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(_startRotation, _endRotation, _curve.Evaluate(_rotationTime));
                return false;
            }
        }

        public bool UpdateScale(FlexalonNode node, Vector3 scale)
        {
            if (scale != _endScale)
            {
                _startScale = transform.localScale;
                _endScale = scale;
                _scaleTime = 0;
            }

            _scaleTime += Time.smoothDeltaTime;

            if (!_animateScale || _scaleTime > _curve.keys[_curve.keys.Length - 1].time)
            {
                transform.localScale = scale;
                return true;
            }
            else
            {
                transform.localScale = Vector3.Lerp(_startScale, _endScale, _curve.Evaluate(_scaleTime));
                return false;
            }
        }
    }
}