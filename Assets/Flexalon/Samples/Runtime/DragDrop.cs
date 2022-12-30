using UnityEngine;

namespace Flexalon.Samples
{
    // Allows the user to click and drag to reorder objects in a layout.
    // It works by replacing the dragged object with an invisible clone
    // and reordering the position of that clone in the layout. To look
    // nice, the objects should have animator components.
    public class DragDrop : Drag
    {
        private GameObject _placeholder;
        private float _hitCooldown;

        protected override void OnDragStart(GameObject go)
        {
            // Insert a placeholder which will take space in the layout while we drag around the object.
            _placeholder = Instantiate(go, go.transform.parent);
            _placeholder.GetComponent<Renderer>().enabled = false;
            _placeholder.transform.SetSiblingIndex(go.transform.GetSiblingIndex());

            // Remove the object from the layout so it can be draggged freely.
            go.transform.SetParent(null, true);
            go.transform.position -= transform.forward * 0.1f;
            go.transform.rotation = Quaternion.identity;
            go.GetComponent<Collider>().enabled = false;
        }

        protected override void OnDragMove(GameObject go, Ray ray)
        {
            // Avoid swapping items too frequently by adding a short cooldown.
            _hitCooldown -= Time.deltaTime;
            if (_hitCooldown <= 0 && Physics.Raycast(ray, out var hit))
            {
                foreach (Transform child in transform)
                {
                    if (hit.transform == child)
                    {
                        // Swap the placeholder with the hit object.
                        _placeholder.transform.SetSiblingIndex(hit.transform.GetSiblingIndex());
                        _distance = hit.distance;
                        _hitCooldown = 0.2f;
                        break;
                    }
                }
            }
        }

        protected override void OnDragStop(GameObject go)
        {
            go.transform.SetParent(transform);
            go.transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());
            go.GetComponent<Collider>().enabled = true;
            Destroy(_placeholder);
        }
    }
}