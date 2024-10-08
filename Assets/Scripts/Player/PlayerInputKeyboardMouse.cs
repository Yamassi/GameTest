using System;
using UniRx;
using UnityEngine;

namespace Player
{
    public class PlayerInputKeyboardMouse : IInput
    {
        public event Action<Vector2> MoveDirection;
        public event Action<Vector2> LookUpMoveDirection;
        public event Action<RaycastHit> LookUpRayCast;
        public event Action<RaycastHit> ClickRaycast;
        public event Action<float> Rotate;

        private CompositeDisposable _disposable;
        private Transform _transform;
        private const float _hitRange = 15f;
        private const int IgnoreRayCastLayer = ~(1 << 2);

        public void Initialize(Transform transform)
        {
            _transform = transform;
            _disposable = new CompositeDisposable();
            InputControl();
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Clear()
        {
            _disposable.Clear();
        }

        private void InputControl()
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                MovementInput();
                MouseInput();
                MouseClick();
                MouseScroll();
            }).AddTo(_disposable);
        }

        private void MouseScroll()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput != 0)
            {
                Rotate?.Invoke(scrollInput);
            }
        }

        private void MouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(_transform.position,
                        _transform.TransformDirection(Vector3.forward), out hit, _hitRange, IgnoreRayCastLayer))
                {
                    ClickRaycast?.Invoke(hit);
                }
            }
        }

        private void MouseInput()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            LookUpMoveDirection?.Invoke(new Vector2(mouseX, mouseY));

            RaycastHit hit;

            if (Physics.Raycast(_transform.position,
                    _transform.TransformDirection(Vector3.forward), out hit, _hitRange, IgnoreRayCastLayer))
            {
                LookUpRayCast?.Invoke(hit);
            }
            else
            {
                LookUpRayCast?.Invoke(new RaycastHit());
            }
        }

        private void MovementInput()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            MoveDirection?.Invoke(new Vector2(moveX, moveZ));
        }
    }
}