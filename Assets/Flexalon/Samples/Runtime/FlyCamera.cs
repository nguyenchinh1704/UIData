using UnityEngine;

namespace Flexalon.Samples
{
    // Simple fly camera. Use WASD to move. Rotate with right mouse button. Pan with mouse wheel button.
    public class FlyCamera : MonoBehaviour
    {
        public float Speed = 0.02f;
        public float RotateSpeed = 0.1f;
        public float InterpolationSpeed = 10.0f;

        public Vector3 targetPosition;
        public Quaternion targetRotation;
        public float alpha;
        public float beta;

        private Vector3 mousePos;

        void Start()
        {
            targetPosition = transform.position;
            targetRotation = transform.rotation;
            var euler = targetRotation.eulerAngles;
            alpha = euler.y;
            beta = euler.x;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                targetPosition += transform.forward * Speed;
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                targetPosition += -transform.right * Speed;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                targetPosition += transform.right * Speed;
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                targetPosition += -transform.forward * Speed;
            }

            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                mousePos = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                var delta = Input.mousePosition - mousePos;
                alpha += delta.x * RotateSpeed;
                beta -= delta.y * RotateSpeed;
                targetRotation = Quaternion.Euler(beta, alpha, 0);
                mousePos = Input.mousePosition;
            }

            if (Input.GetMouseButtonDown(2))
            {
                mousePos = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                var delta = Input.mousePosition - mousePos;
                targetPosition -= delta.y * transform.up * Speed;
                targetPosition -= delta.x * transform.right * Speed;
                mousePos = Input.mousePosition;
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * InterpolationSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * InterpolationSpeed);
        }
    }
}