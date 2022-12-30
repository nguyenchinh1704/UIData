using System.Collections.Generic;
using UnityEngine;

namespace Flexalon
{
    [HelpURL("https://www.flexalon.com/docs/cloner")]
    public class FlexalonCloner : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _objects;
        public List<GameObject> Objects
        {
            get => _objects;
            set { _objects = value; MarkDirty(); }
        }

        public enum CloneTypes
        {
            Iterative,
            Random
        }

        [SerializeField]
        private CloneTypes _cloneType = CloneTypes.Iterative;
        public CloneTypes CloneType
        {
            get => _cloneType;
            set { _cloneType = value; MarkDirty(); }
        }

        [SerializeField]
        private uint _count;
        public uint Count
        {
            get => _count;
            set { _count = value; MarkDirty(); }
        }

        [SerializeField]
        private int _randomSeed;
        public int RandomSeed
        {
                get => _randomSeed;
                set { _randomSeed = value; MarkDirty(); }
        }

        [SerializeField]
        private GameObject _dataSource = null;
        public GameObject DataSource
        {
            get => _dataSource;
            set
            {
                UnhookDataSource();
                _dataSource = value;
                HookDataSource();
                MarkDirty();
            }
        }

        [SerializeField]
        private List<GameObject> _clones = new List<GameObject>();

        void OnEnable()
        {
            HookDataSource();
            MarkDirty();
        }

        void HookDataSource()
        {
            if (isActiveAndEnabled && _dataSource != null && _dataSource)
            {
                if (_dataSource.TryGetComponent<DataSource>(out var component))
                {
                    component.DataChanged += MarkDirty;
                }
            }
        }

        void UnhookDataSource()
        {
            if (_dataSource != null && _dataSource)
            {
                if (_dataSource.TryGetComponent<DataSource>(out var component))
                {
                    component.DataChanged -= MarkDirty;
                }
            }
        }

        void OnDisable()
        {
            UnhookDataSource();
            MarkDirty();
        }

        public void MarkDirty()
        {
            foreach(var clone in _clones)
            {
                if (Application.isPlaying)
                {
                    Destroy(clone);
                }
                else
                {
                    DestroyImmediate(clone);
                }
            }

            _clones.Clear();

            if (isActiveAndEnabled && _objects != null && _objects.Count > 0)
            {
                switch (_cloneType)
                {
                    case CloneTypes.Iterative:
                        GenerateIterativeClones();
                        break;
                    case CloneTypes.Random:
                        GenerateRandomClones();
                        break;
                }
            }
        }

        IReadOnlyList<object> GetData()
        {
            if (_dataSource != null && _dataSource)
            {
                return _dataSource.GetComponent<DataSource>()?.Data;
            }

            return null;
        }

        void GenerateIterativeClones()
        {
            int i = 0;
            var data = GetData();
            var count = data?.Count ?? (int)_count;
            while (_clones.Count < count)
            {
                GenerateClone(i, data);
                i = (i + 1) % _objects.Count;
            }
        }

        void GenerateRandomClones()
        {
            var random = new System.Random(_randomSeed);
            var data = GetData();
            var count = data?.Count ?? (int)_count;
            while (_clones.Count < count)
            {
                GenerateClone(random.Next(_objects.Count), data);
            }
        }

        void GenerateClone(int index, IReadOnlyList<object> data)
        {
            var clone = Instantiate(_objects[index], Vector3.zero, Quaternion.identity, transform);
            _clones.Add(clone);

            if (data != null && clone.TryGetComponent<DataBinding>(out var dataBinding))
            {
                dataBinding.SetData(data[_clones.Count - 1]);
            }
        }
    }
}