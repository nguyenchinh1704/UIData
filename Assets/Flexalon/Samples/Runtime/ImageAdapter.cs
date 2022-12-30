using UnityEngine;

namespace Flexalon.Samples
{
    // An example adapter which maintains the aspect ratio of the material's mainTexture.
    // Expects to be used with a Quad mesh.
    [ExecuteAlways]
    public class ImageAdapter : FlexalonComponent, Adapter
    {
        private Texture _texture;
        private Renderer _renderer;

        // Returns a size which will maintain the aspect ratio of whichever
        // axis is set to SizeType.Component.
        public Bounds Measure(FlexalonNode node, Vector3 size)
        {
            if (_texture)
            {
                bool componentX = node.GetSizeType(Axis.X) == SizeType.Component;
                bool componentY = node.GetSizeType(Axis.Y) == SizeType.Component;
                var aspectRatio = (float)_texture.width / _texture.height;
                if (componentX && !componentY)
                {
                    size[0] = size[1] * aspectRatio;
                }
                else if (componentY && !componentX)
                {
                    size[1] = size[0] / aspectRatio;
                }
            }

            return new Bounds(Vector3.zero, size);
        }

        // Returns the desired scale which fits the measured bounds on X/Y.
        public Vector3 UpdateSize(FlexalonNode node)
        {
            if (_texture && _renderer)
            {
                var r = node.Result;
                Vector3 size = _renderer.GetComponent<MeshFilter>()?.sharedMesh.bounds.size ?? Vector3.one;
                var scale = new Vector3(
                    r.AdapterBounds.size.x / size.x,
                    r.AdapterBounds.size.y / size.y,
                    1);
                return scale;
            }

            return Vector3.one;
        }

        protected override void UpdateProperties()
        {
            _node.SetAdapter(this);
        }

        protected override void ResetProperties()
        {
            _node.SetAdapter(null);
        }

        public override void DoUpdate()
        {
            // Detect if the texture changes, in which case we need to invalidate the layout.
            if (TryGetComponent<Renderer>(out var renderer))
            {
                _renderer = renderer;
                Texture texture = null;
                if (renderer.sharedMaterial)
                {
                    if (renderer.sharedMaterial.HasProperty("_BaseColorMap")) // HDRP.Lit
                    {
                        texture = renderer.sharedMaterial.GetTexture("_BaseColorMap");
                    }
                    else if (renderer.sharedMaterial.HasProperty("_BaseMap")) // URP.Lit
                    {
                        texture = renderer.sharedMaterial.GetTexture("_BaseMap");
                    }
                    else if (renderer.sharedMaterial.HasProperty("_MainTex")) // Standard
                    {
                        texture = renderer.sharedMaterial.GetTexture("_MainTex");
                    }
                }

                if (texture != _texture)
                {
                    _texture = texture;
                    MarkDirty();
                }
            }
        }
    }
}