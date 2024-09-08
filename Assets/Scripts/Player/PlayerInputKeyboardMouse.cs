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
        private CompositeDisposable _disposable;
        private Transform _transform;
        private const float _hitRange = 50f;

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
            }).AddTo(_disposable);
        }

        private void MouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(_transform.position,
                        _transform.TransformDirection(Vector3.forward), out hit, _hitRange))
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
                    _transform.TransformDirection(Vector3.forward), out hit, _hitRange))
            {
                LookUpRayCast?.Invoke(hit);
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