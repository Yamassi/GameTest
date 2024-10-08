using Items;
using Surfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _lookUpSensivity;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _handsPoint;

        private CharacterController _characterController;
        private Highlight _currentlyHighlightedPickable;
        private IInput _input;

        private Vector2 _moveDirection;
        private Vector2 _mouseDirection;
        private Vector3 _playerVelocity;
        private float _rotate;

        private const float _rotationSpeed = 150;
        private const float _minCameraView = -70, _maxCameraView = 80;
        private float _xRotation = 0f;

        private RaycastHit _hit;
        private GameObject _inHandObject;

        private bool _isPickedUp;


        public void Initialize()
        {
            Debug.Log("Initialize Player");
            _characterController = GetComponent<CharacterController>();
            _input = new PlayerInputKeyboardMouse();
            _input.Initialize(_camera.transform);
            _input.MoveDirection += OnInputMoveDirection;
            _input.LookUpMoveDirection += OnMouseMoveDirection;
            _input.LookUpRayCast += OnRaycast;
            _input.ClickRaycast += Interact;
            _input.Rotate += OnRotate;
            _isPickedUp = false;
        }

        private void OnDestroy()
        {
            _input.MoveDirection -= OnInputMoveDirection;
            _input.LookUpMoveDirection -= OnMouseMoveDirection;
            _input.LookUpRayCast -= OnRaycast;
            _input.ClickRaycast -= Interact;
            _input.Rotate -= OnRotate;
            _input.Clear();
        }

        private void OnRotate(float rotate) => _rotate = rotate;
        private void OnRaycast(RaycastHit hit) => _hit = hit;
        private void OnMouseMoveDirection(Vector2 mouseDirection) => _mouseDirection = mouseDirection;
        private void OnInputMoveDirection(Vector2 moveDirection) => _moveDirection = moveDirection;

        private void Update()
        {
            Movement();
            Mouse();
        }

        private void FixedUpdate()
        {
            Gravity();
        }

        private void Gravity()
        {
            if (_characterController.isGrounded)
            {
                _playerVelocity.y = 0f;
            }
            else
            {
                _playerVelocity.y += -9.18f * Time.deltaTime;
                _characterController.Move(_playerVelocity * Time.deltaTime);
            }
        }

        private void Mouse()
        {
            LookUp();
            if (_isPickedUp)
            {
                HighlightPutDownPlaces();
            }
            else
            {
                HighlightPickableItems();
            }
        }

        private void Interact(RaycastHit hit)
        {
            if (_isPickedUp)
            {
                PutDown(hit);
            }
            else
            {
                PickUp(hit);
            }
        }

        private void PutDown(RaycastHit hit)
        {
            IPickable pickable = _inHandObject.GetComponent<IPickable>();

            if (_hit.collider != null && pickable.IsPutDownCorrectPlace)
            {
                pickable.PutDown(_hit);
                _isPickedUp = false;
                _inHandObject = null;
            }
        }

        private void PickUp(RaycastHit hit)
        {
            if (hit.collider != null && hit.collider.TryGetComponent(out IPickable pickable))
            {
                Debug.Log("Pick Up");
                _inHandObject = pickable.PickUp();
                _inHandObject.transform.SetParent(_handsPoint, false);
                _inHandObject.GetComponent<Highlight>().EnableInHandsHighlight();
                _isPickedUp = true;
            }
        }

        private void LookUp()
        {
            float mouseX = _mouseDirection.x * _lookUpSensivity * Time.deltaTime;
            float mouseY = _mouseDirection.y * _lookUpSensivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, _minCameraView, _maxCameraView);

            _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }

        private void HighlightPutDownPlaces()
        {
            IPickable pickable = _inHandObject.GetComponent<IPickable>();
            Highlight highlight = _inHandObject.GetComponent<Highlight>();

            if (_hit.collider != null && pickable.IsPutDownCorrectPlace)
            {
                ReplaceInHandObject();
                highlight.EnableСorrectPlaceHighlight();
            }
            else if (_hit.collider != null && !pickable.IsPutDownCorrectPlace)
            {
                ReplaceInHandObject();
                highlight.EnableWrongPlaceHighlight();
            }
            else
            {
                pickable.PickUp();
                highlight.EnablePickUpHighlight();
            }
        }

        private void ReplaceInHandObject()
        {
            float offset = 0.49f;
            Vector3 newPosition = _hit.point + _hit.normal * offset;

            _inHandObject.transform.position = newPosition;
            _inHandObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, _hit.normal);

            Rotate();
        }

        private void Rotate()
        {
            float rotationAmount = _rotate * _rotationSpeed;
            rotationAmount = Mathf.Clamp(rotationAmount, -45f, 45f);

            float currentYRotation = _inHandObject.transform.eulerAngles.y;

            float targetRotationY = Mathf.Round(currentYRotation + rotationAmount);

            _inHandObject.transform.eulerAngles = new Vector3(
                _inHandObject.transform.eulerAngles.x,
                targetRotationY,
                _inHandObject.transform.eulerAngles.z);
        }

        private void HighlightPickableItems()
        {
            if (_hit.collider != null && _hit.collider.TryGetComponent(out IPickable pickable))
            {
                Highlight highlight = _hit.collider.GetComponent<Highlight>();

                if (_currentlyHighlightedPickable != null && _currentlyHighlightedPickable != highlight)
                {
                    _currentlyHighlightedPickable.DisableHighlight();
                }

                highlight.EnablePickUpHighlight();

                _currentlyHighlightedPickable = highlight;
            }
            else
            {
                if (_currentlyHighlightedPickable != null)
                {
                    _currentlyHighlightedPickable.DisableHighlight();
                    _currentlyHighlightedPickable = null;
                }
            }
        }

        private void Movement()
        {
            Vector3 movement = transform.forward * _moveDirection.y + transform.right * _moveDirection.x;
            _characterController.Move(movement * _movementSpeed * Time.deltaTime);
        }
    }
}