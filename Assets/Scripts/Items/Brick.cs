using Surfaces;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class Brick : MonoBehaviour, IPickable
    {
        [SerializeField] private Transform _pickUpItems;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private BoxCollider _collider;
        private bool _isOnGround;

        private const int DefaultLayer = 0, IgnoreRayCastLayer = 2;
        private const float DropDownOffset = 0.95f;
        public bool IsPutDownCorrectPlace { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<BoxCollider>();
            IsPutDownCorrectPlace = false;
            _isOnGround = true;
        }

        public void PutDown(RaycastHit hit)
        {
            Vector3 newPosition;
            if (_isOnGround)
            {
                float offset = 0.5f;
                newPosition = hit.point + hit.normal * offset;
            }
            else
            {
                Bounds hitBounds = hit.collider.bounds;
                Bounds pickableBounds = _collider.bounds;
                newPosition = transform.position;
                newPosition.y = hitBounds.max.y + pickableBounds.extents.y;
            }

            gameObject.layer = DefaultLayer;
            transform.localScale = Vector3.one;

            transform.localRotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
            transform.SetParent(_pickUpItems, false);
            transform.position = newPosition;

            _collider.isTrigger = false;
        }

        public GameObject PickUp()
        {
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            if (gameObject.layer == DefaultLayer)
                gameObject.layer = IgnoreRayCastLayer;
            _collider.isTrigger = true;
            return gameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Brick brick))
            {
                IsPutDownCorrectPlace = IsCorrectBrickPlaceCheck(brick.transform);
                _isOnGround = false;
            }
            else if (other.TryGetComponent(out Ground ground))
            {
                IsPutDownCorrectPlace = true;
                _isOnGround = true;
            }
            else
            {
                IsPutDownCorrectPlace = false;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out Brick brick))
            {
                IsPutDownCorrectPlace = IsCorrectBrickPlaceCheck(brick.transform);
                _isOnGround = false;
            }
            else if (other.TryGetComponent(out Ground ground))
            {
                IsPutDownCorrectPlace = true;
                _isOnGround = true;
            }
            else
            {
                IsPutDownCorrectPlace = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Brick brick))
            {
                IsPutDownCorrectPlace = false;
                _isOnGround = false;
            }
            else if (other.TryGetComponent(out Ground ground))
            {
                IsPutDownCorrectPlace = false;
                _isOnGround = true;
            }
            else
            {
                IsPutDownCorrectPlace = false;
            }
        }

        private bool IsCorrectBrickPlaceCheck(Transform item)
        {
            if (Mathf.Abs(item.transform.position.y - transform.position.y) > DropDownOffset)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}