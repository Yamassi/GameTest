using System;
using Surfaces;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class Sphere : MonoBehaviour, IPickable
    {
        [SerializeField] private Transform _pickUpItems;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private SphereCollider _collider;

        private const int DefaultLayer = 0, IgnoreRayCastLayer = 2;
        private const float DropDownOffset = 0.95f;
        public bool IsPutDownCorrectPlace { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<SphereCollider>();
            IsPutDownCorrectPlace = false;
        }

        public void PutDown(RaycastHit hit)
        {
            float offset = 0.5f;
            Vector3 newPosition = hit.point + hit.normal * offset;

            gameObject.layer = 0;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
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

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out Wall wall))
            {
                IsPutDownCorrectPlace = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Wall wall))
            {
                IsPutDownCorrectPlace = false;
            }
        }
    }
}