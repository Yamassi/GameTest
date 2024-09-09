using System;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class Brick : MonoBehaviour, IPickable
    {
        [SerializeField] private Transform _pickUpItems;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private BoxCollider _boxCollider;

        private const int DefaultLayer = 0, IgnoreRayCastLayer = 2;

        public bool IsCorrectPlace { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();
            IsCorrectPlace = true;
        }

        public void PutDown(Vector3 position)
        {
            gameObject.layer = 0;
            _boxCollider.isTrigger = false;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.SetParent(_pickUpItems, false);
            transform.position = position;
        }

        public GameObject PickUp()
        {
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            if (gameObject.layer == DefaultLayer)
                gameObject.layer = IgnoreRayCastLayer;
            _boxCollider.isTrigger = true;
            return gameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Brick brick))
            {
                IsCorrectPlace = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Brick brick))
            {
                IsCorrectPlace = true;
            }
        }
    }
}