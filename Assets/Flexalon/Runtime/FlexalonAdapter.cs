using UnityEngine;

namespace Flexalon
{
    public interface Adapter
    {
        // Measure the size of this component considering component size parameters.
        Bounds Measure(FlexalonNode node, Vector3 size);

        // Update components sizes to match size and return desired transform scale.
        Vector3 UpdateSize(FlexalonNode node);
    }

    internal interface InternalAdapter : Adapter
    {
        bool IsValid();
        bool SizeChanged();
    }

    internal class DefaultAdapter : Adapter
    {
        private InternalAdapter _adapter;

        public DefaultAdapter(GameObject gameObject)
        {
            CheckComponent(gameObject);
            _adapter?.SizeChanged();
        }

        public bool CheckComponent(GameObject gameObject)
        {
            if (_adapter == null)
            {
                CreateAdapter(gameObject);
                return _adapter != null;
            }

            if (!_adapter.IsValid())
            {
                CreateAdapter(gameObject);
                return true;
            }

            if (_adapter != null && _adapter.SizeChanged())
            {
                return true;
            }

            return false;
        }

        public void CreateAdapter(GameObject gameObject)
        {
            _adapter = null;

            if (gameObject.TryGetComponent<TMPro.TMP_Text>(out var text))
            {
                _adapter = new TextAdapter(text);
            }
            else if (gameObject.TryGetComponent<RectTransform>(out var rectTransform))
            {
                _adapter = new RectTransformAdapter(rectTransform);
            }
            else if (gameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                _adapter = new SpriteRendererAdapter(spriteRenderer);
            }
            else if (gameObject.TryGetComponent<MeshRenderer>(out var renderer) && gameObject.TryGetComponent<MeshFilter>(out var meshFilter) && meshFilter.sharedMesh)
            {
                _adapter = new MeshRendererAdapter(renderer, meshFilter);
            }
            else if (gameObject.TryGetComponent<Collider>(out var collider))
            {
                _adapter = new ColliderAdapter(collider);
            }
            else if (gameObject.TryGetComponent<Collider2D>(out var collider2d))
            {
                _adapter = new Collider2DAdapter(collider2d);
            }
        }

        public Bounds Measure(FlexalonNode node, Vector3 size)
        {
            if (_adapter != null)
            {
                return _adapter.Measure(node, size);
            }
            else
            {
                return new Bounds(Vector3.zero, size);
            }
        }

        public Vector3 UpdateSize(FlexalonNode node)
        {
            if (_adapter != null)
            {
                return _adapter.UpdateSize(node);
            }
            else
            {
                return Vector3.one;
            }
        }
    }

    internal class SpriteRendererAdapter : InternalAdapter
    {
        private SpriteRenderer _renderer;
        private Bounds _lastRendererBounds;

        public SpriteRendererAdapter(SpriteRenderer renderer)
        {
            _renderer = renderer;
        }

        public bool IsValid()
        {
            return _renderer;
        }

        public bool SizeChanged()
        {
            var spriteBounds = GetBounds();
            if (_lastRendererBounds != spriteBounds)
            {
                _lastRendererBounds = spriteBounds;
                return true;
            }

            return false;
        }

        private Bounds GetBounds()
        {
            return _renderer.sprite?.bounds ?? new Bounds();
        }

        public Bounds Measure(FlexalonNode node, Vector3 size)
        {
            return Math.MeasureComponentBounds2D(GetBounds(), node, size);
        }

        public Vector3 UpdateSize(FlexalonNode node)
        {
            var bounds = GetBounds();
            if (bounds.size == Vector3.zero) // Invalid bounds
            {
                return Vector3.one;
            }

            var r = node.Result;
            var scale = new Vector3(
                r.AdapterBounds.size.x / bounds.size.x,
                r.AdapterBounds.size.y / bounds.size.y,
                1);
            return scale;
        }
    }

    internal class MeshRendererAdapter : InternalAdapter
    {
        private MeshRenderer _renderer;
        private MeshFilter _meshFilter;
        private Bounds _lastRendererBounds;

        public MeshRendererAdapter(MeshRenderer renderer, MeshFilter meshFilter)
        {
            _renderer = renderer;
            _meshFilter = meshFilter;
        }

        public bool IsValid()
        {
            return _renderer && _meshFilter && _meshFilter.sharedMesh;
        }

        public bool SizeChanged()
        {
            if (_lastRendererBounds != _meshFilter.sharedMesh.bounds)
            {
                _lastRendererBounds = _meshFilter.sharedMesh.bounds;
                return true;
            }

            return false;
        }

        public Bounds Measure(FlexalonNode node, Vector3 size)
        {
            return Math.MeasureComponentBounds(_meshFilter.sharedMesh.bounds, node, size);
        }

        public Vector3 UpdateSize(FlexalonNode node)
        {
            var bounds = _meshFilter.sharedMesh.bounds;
            if (bounds.size == Vector3.zero) // Invalid bounds
            {
                return Vector3.one;
            }

            var r = node.Result;
            var scale = Math.Div(r.AdapterBounds.size, bounds.size);
            scale.x = scale.x > 100000f ? 1 : scale.x;
            scale.y = scale.y > 100000f ? 1 : scale.y;
            scale.z = scale.z > 100000f ? 1 : scale.z;
            return scale;
        }
    }

    internal class RectTransformAdapter : InternalAdapter
    {
        private RectTransform _rectTransform;
        private Vector2 _lastSizeDelta;

        public RectTransformAdapter(RectTransform rectTransform)
        {
            _rectTransform = rectTransform;
        }

        public bool IsValid()
        {
            return _rectTransform;
        }

        public bool SizeChanged()
        {
            if (_lastSizeDelta != _rectTransform.sizeDelta)
            {
                _lastSizeDelta = _rectTransform.sizeDelta;
                return true;
            }

            return false;
        }

        public Bounds Measure(FlexalonNode node, Vector3 size)
        {
            var bounds = new Bounds(Vector3.zero, _rectTransform.sizeDelta);
            return Math.MeasureComponentBounds(bounds, node, size);
        }

        public Vector3 UpdateSize(FlexalonNode node)
        {
            var r = node.Result;
            Vector3 size = _rectTransform.sizeDelta;
            var scale = new Vector3(
                r.AdapterBounds.size.x / _rectTransform.sizeDelta.x,
                r.AdapterBounds.size.y / _rectTransform.sizeDelta.y,
                1);
            return scale;
        }
    }

    internal class TextAdapter : InternalAdapter
    {
        private TMPro.TMP_Text _text;
        private string _lastFont;
        private TMPro.FontWeight _lastFontWeight;
        private float _lastFontSize;
        private string _lastText;

        public TextAdapter(TMPro.TMP_Text text)
        {
            _text = text;
        }

        public bool IsValid()
        {
            return _text;
        }

        public bool SizeChanged()
        {
            if (_lastFont != _text.font.ToString() ||
                _lastFontWeight != _text.fontWeight ||
                _lastFontSize != _text.fontSize ||
                _lastText != _text.text)
            {
                _lastFont = _text.font.ToString();
                _lastFontWeight = _text.fontWeight;
                _lastFontSize = _text.fontSize;
                _lastText = _text.text;
                return true;
            }

            return false;
        }

        public Bounds Measure(FlexalonNode node, Vector3 size)
        {
            bool componentX = node.GetSizeType(Axis.X) == SizeType.Component;
            bool componentY = node.GetSizeType(Axis.Y) == SizeType.Component;
            bool componentZ = node.GetSizeType(Axis.Z) == SizeType.Component;
            var rect = _text.rectTransform.rect;
            var bounds = new Bounds();
            Vector2 preferred;

            if (componentX && componentY)
            {
                preferred = _text.GetPreferredValues(99999999, 0);
            }
            else if (componentX && !componentY)
            {
                preferred = _text.GetPreferredValues(0, size.y);
            }
            else if (!componentX && componentY)
            {
                preferred = _text.GetPreferredValues(size.x, 0);
            }
            else
            {
                preferred = size;
            }

            bounds.size = new Vector3(
                componentX ? preferred.x : size.x,
                componentY ? preferred.y : size.y,
                componentZ ? 0 : size.z);
            return bounds;
        }

        public Vector3 UpdateSize(FlexalonNode node)
        {
            var r = node.Result;
            _text.rectTransform.sizeDelta = new Vector2(r.AdapterBounds.size[0], r.AdapterBounds.size[1]);
            return Vector3.one;
        }
    }

    internal class ColliderAdapter : InternalAdapter
    {
        private Collider _collider;
        private Bounds _lastBounds;

        public ColliderAdapter(Collider collider)
        {
            _collider = collider;
        }

        public bool IsValid()
        {
            return _collider;
        }

        public Bounds GetBounds()
        {
            if (_collider is BoxCollider)
            {
                var box = _collider as BoxCollider;
                return new Bounds(box.center, box.size);
            }
            else if (_collider is SphereCollider)
            {
                var sphere = _collider as SphereCollider;
                return new Bounds(sphere.center, Vector3.one * sphere.radius * 2);
            }
            else if (_collider is CapsuleCollider)
            {
                var capsule = _collider as CapsuleCollider;
                var size = Vector3.one * capsule.radius;
                size[capsule.direction] = capsule.height;
                return new Bounds(capsule.center, size);
            }
            else if (_collider is MeshCollider)
            {
                var mesh = _collider as MeshCollider;
                return mesh.sharedMesh?.bounds ?? new Bounds(Vector3.zero, Vector3.zero);
            }

            return new Bounds(Vector3.zero, Vector3.zero);
        }

        public bool SizeChanged()
        {
            var bounds = GetBounds();
            if (_lastBounds != bounds)
            {
                _lastBounds = bounds;
                return true;
            }

            return false;
        }

        public Bounds Measure(FlexalonNode node, Vector3 size)
        {
            return Math.MeasureComponentBounds(GetBounds(), node, size);
        }

        public Vector3 UpdateSize(FlexalonNode node)
        {
            var bounds = GetBounds();
            if (bounds.size == Vector3.zero) // Invalid bounds
            {
                return Vector3.one;
            }

            var r = node.Result;
            var scale = Math.Div(r.AdapterBounds.size, bounds.size);
            return scale;
        }
    }

    internal class Collider2DAdapter : InternalAdapter
    {
        private Collider2D _collider;
        private Bounds _lastBounds;

        public Collider2DAdapter(Collider2D collider)
        {
            _collider = collider;
        }

        public bool IsValid()
        {
            return _collider;
        }

        public Bounds GetBounds()
        {
            if (_collider is BoxCollider2D)
            {
                var box = _collider as BoxCollider2D;
                return new Bounds(box.offset, box.size + Vector2.one * box.edgeRadius);
            }
            else if (_collider is CircleCollider2D)
            {
                var circle = _collider as CircleCollider2D;
                return new Bounds(circle.offset, Vector2.one * circle.radius * 2);
            }
            else if (_collider is CapsuleCollider2D)
            {
                var capsule = _collider as CapsuleCollider2D;
                return new Bounds(capsule.offset, capsule.size);
            }

            return new Bounds(Vector3.zero, Vector3.zero);
        }

        public bool SizeChanged()
        {
            var bounds = GetBounds();
            if (_lastBounds != bounds)
            {
                _lastBounds = bounds;
                return true;
            }

            return false;
        }

        public Bounds Measure(FlexalonNode node, Vector3 size)
        {
            return Math.MeasureComponentBounds2D(GetBounds(), node, size);
        }

        public Vector3 UpdateSize(FlexalonNode node)
        {
            var bounds = GetBounds();
            if (bounds.size == Vector3.zero) // Invalid bounds
            {
                return Vector3.one;
            }

            var r = node.Result;
            var scale = new Vector3(
                r.AdapterBounds.size.x / bounds.size.x,
                r.AdapterBounds.size.y / bounds.size.y,
                1);
            return scale;
        }
    }
}