using UnityEngine;

namespace Flexalon.Samples
{
    // Allow the user to click and drag this object or its children.
    public class Drag : MonoBehaviour
    {
        [SerializeField]
        public float _interpolationSpeed = 5;

        protected bool _dragging = false;
        protected Vector3 _target;
        protected Vector3 _offset;
        protected float _distance;
        protected GameObject _draggedObject;

        void Update()
        {
            if (_dragging)
            {
                if (Input.GetMouseButton(0))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    _target = ray.origin + ray.direction * _distance - _offset;
                    _draggedObject.transform.position = Vector3.Lerp(_draggedObject.transform.position, _target, Time.deltaTime * _interpolationSpeed);
                    OnDragMove(_draggedObject, ray);
                }
                else
                {
                    OnDragStop(_draggedObject);
                    _draggedObject = null;
                    _dragging = false;
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
                {
                    if (hit.transform == transform || hit.transform.IsChildOf(transform))
                    {
                        _target = hit.transform.position;
                        _offset = hit.point - hit.transform.position;
                        _distance = hit.distance;
                        _dragging = true;
                        _draggedObject = hit.transform.gameObject;
                        OnDragStart(_draggedObject);
                    }
                }
            }
        }

        protected virtual void OnDragStart(GameObject go)
        {
        }

        protected virtual void OnDragMove(GameObject go, Ray ray)
        {
        }

        protected virtual void OnDragStop(GameObject go)
        {
        }
    }
}