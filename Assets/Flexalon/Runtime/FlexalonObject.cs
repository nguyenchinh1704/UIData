using UnityEngine;

namespace Flexalon
{
    [ExecuteAlways, DisallowMultipleComponent, HelpURL("https://www.flexalon.com/docs/flexalonObject")]
    public class FlexalonObject : FlexalonComponent
    {
        public Vector3 Size
        {
            get => new Vector3(_width, _height, _depth);
            set
            {
                Width = value.x;
                Height = value.y;
                Depth = value.z;
            }
        }

        public Vector3 SizeOfParent
        {
            get => new Vector3(_widthOfParent, _heightOfParent, _depthOfParent);
            set
            {
                WidthOfParent = value.x;
                HeightOfParent = value.y;
                DepthOfParent = value.z;
            }
        }

        [SerializeField]
        private SizeType _widthType = SizeType.Component;
        public SizeType WidthType
        {
            get { return _widthType; }
            set {
                _widthType = value;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _width = 1;
        public float Width
        {
            get { return _width; }
            set {
                _width = Mathf.Max(value, 0);
                _widthType = SizeType.Fixed;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _widthOfParent = 1;
        public float WidthOfParent
        {
            get { return _widthOfParent; }
            set {
                _widthOfParent = Mathf.Max(value, 0);
                _widthType = SizeType.Fill;
                MarkDirty();
            }
        }

        [SerializeField]
        private SizeType _heightType = SizeType.Component;
        public SizeType HeightType
        {
            get { return _heightType; }
            set {
                _heightType = value;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _height = 1;
        public float Height
        {
            get { return _height; }
            set {
                _height = Mathf.Max(value, 0);
                _heightType = SizeType.Fixed;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _heightOfParent = 1;
        public float HeightOfParent
        {
            get { return _heightOfParent; }
            set {
                _heightOfParent = Mathf.Max(value, 0);
                _heightType = SizeType.Fill;
                MarkDirty();
            }
        }

        [SerializeField]
        private SizeType _depthType = SizeType.Component;
        public SizeType DepthType
        {
            get { return _depthType; }
            set {
                _depthType = value;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _depth = 1;
        public float Depth
        {
            get { return _depth; }
            set {
                _depth = Mathf.Max(value, 0);
                _depthType = SizeType.Fixed;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _depthOfParent = 1;
        public float DepthOfParent
        {
            get { return _depthOfParent; }
            set {
                _depthOfParent = Mathf.Max(value, 0);
                _depthType = SizeType.Fill;
                MarkDirty();
            }
        }

        [SerializeField]
        private Vector3 _offset = Vector3.zero;
        public Vector3 Offset
        {
            get { return _offset; }
            set { _offset = value; MarkDirty(); }
        }

        [SerializeField]
        private Vector3 _scale = Vector3.one;
        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; MarkDirty(); }
        }

        [SerializeField]
        private Quaternion _rotation = Quaternion.identity;
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginLeft;
        public float MarginLeft
        {
            get { return _marginLeft; }
            set { _marginLeft = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginRight;
        public float MarginRight
        {
            get { return _marginRight; }
            set { _marginRight = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginTop;
        public float MarginTop
        {
            get { return _marginTop; }
            set { _marginTop = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginBottom;
        public float MarginBottom
        {
            get { return _marginBottom; }
            set { _marginBottom = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginFront;
        public float MarginFront
        {
            get { return _marginFront; }
            set { _marginFront = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginBack;
        public float MarginBack
        {
            get { return _marginBack; }
            set { _marginBack = value; MarkDirty(); }
        }

        public Directions Margin =>
            new Directions(new float[] {
                _marginRight, _marginLeft, _marginTop, _marginBottom, _marginBack, _marginFront});

        [SerializeField]
        private float _paddingLeft;
        public float PaddingLeft
        {
            get { return _paddingLeft; }
            set { _paddingLeft = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingRight;
        public float PaddingRight
        {
            get { return _paddingRight; }
            set { _paddingRight = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingTop;
        public float PaddingTop
        {
            get { return _paddingTop; }
            set { _paddingTop = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingBottom;
        public float PaddingBottom
        {
            get { return _paddingBottom; }
            set { _paddingBottom = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingFront;
        public float PaddingFront
        {
            get { return _paddingFront; }
            set { _paddingFront = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingBack;
        public float PaddingBack
        {
            get { return _paddingBack; }
            set { _paddingBack = value; MarkDirty(); }
        }

        public Directions Padding =>
            new Directions(new float[] {
                _paddingRight, _paddingLeft, _paddingTop, _paddingBottom, _paddingBack, _paddingFront});

        protected override void ResetProperties()
        {
            _node.SetFlexalonObject(null);
        }

        protected override void UpdateProperties()
        {
            _node.SetFlexalonObject(this);
        }

#if UNITY_EDITOR
        public override void DoUpdate()
        {
            // Detect changes to the object's position rotation and scale, which may happen
            // when the developer uses the transform control, enters new values in the
            // inspector, or various other scenarios. Maintain those edits
            // by modifying the offset, rotation, and scale on the FlexalonObject.
            if (!Application.isPlaying && !Node.Dirty)
            {
                var result = _node.Result;
                if ((Node.Parent != null && !Node.Parent.Dirty) || (Node.Constraint != null && Node.Constraint.Target != null))
                {
                    if (result.TransformPosition != transform.localPosition)
                    {
                        UnityEditor.Undo.RecordObject(this, "Offset change");
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                        UnityEditor.Undo.RecordObject(result, "Offset change");
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                        _offset += (transform.localPosition - result.TransformPosition);
                        result.TransformPosition = transform.localPosition;
                    }

                    if (result.TransformRotation != transform.localRotation)
                    {
                        UnityEditor.Undo.RecordObject(this, "Rotation change");
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                        UnityEditor.Undo.RecordObject(result, "Rotation change");
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                        Flexalon.GetOrCreate().RecordFrameChanges = true;
                        _rotation *= transform.localRotation * Quaternion.Inverse(result.TransformRotation);
                        _rotation.Normalize();
                        result.TransformRotation = transform.localRotation;
                        _node.ApplyScaleAndRotation();
                        _node.Parent?.MarkDirty();
                    }

                    if (result.TransformScale != transform.localScale)
                    {
                        UnityEditor.Undo.RecordObject(this, "Scale change");
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                        UnityEditor.Undo.RecordObject(result, "Scale change");
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                        Flexalon.GetOrCreate().RecordFrameChanges = true;
                        _scale = Math.Mul(Scale, Math.Div(transform.localScale, result.TransformScale));
                        result.TransformScale = transform.localScale;
                        _node.ApplyScaleAndRotation();
                        _node.Parent?.MarkDirty();
                    }
                }
                else
                {
                    if (result.TransformRotation != transform.localRotation)
                    {
                        UnityEditor.Undo.RecordObject(result, "Rotation change");
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                        result.TransformRotation = transform.localRotation;
                        _node.ApplyScaleAndRotation();
                    }

                    if (result.TransformScale != transform.localScale)
                    {
                        UnityEditor.Undo.RecordObject(result, "Scale change");
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                        result.TransformScale = transform.localScale;
                        _node.ApplyScaleAndRotation();
                    }
                }
            }
        }
#endif
    }
}