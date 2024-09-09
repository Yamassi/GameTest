using UnityEngine;

namespace Items
{
    public interface IPickable
    {
        public GameObject PickUp();
        public void PutDown(Vector3 position);
        public bool IsCorrectPlace { get; }
    }
}