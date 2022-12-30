using UnityEngine;

namespace Flexalon
{
    public abstract class FlexalonComponent : MonoBehaviour
    {
        protected FlexalonNode _node;
        public FlexalonNode Node => _node;

        void Update()
        {
            DoUpdate();
        }

        void OnEnable()
        {
            _node = Flexalon.GetOrCreateNode(gameObject);

            DoOnEnable();

            if (!_node.HasResult)
            {
                MarkDirty();
            }
            else
            {
                UpdateProperties();
            }
        }

        void OnDestroy()
        {
            if (_node != null)
            {
                ResetProperties();
                _node.MarkDirty();
                _node = null;
            }
        }

        public void MarkDirty()
        {
            if (_node != null)
            {
                UpdateProperties();
                _node.MarkDirty();
            }
        }

        public void ForceUpdate()
        {
            _node = Flexalon.GetOrCreateNode(gameObject);
            MarkDirty();
            _node.ForceUpdate();
        }

        void OnDidApplyAnimationProperties()
        {
            MarkDirty();
        }

        protected virtual void UpdateProperties() {}

        protected virtual void ResetProperties() {}

        protected virtual void DoOnEnable() {}

        public virtual void DoUpdate() {}

        public virtual void OnUndoRedo()
        {
            MarkDirty();
        }
    }
}