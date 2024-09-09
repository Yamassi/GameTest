using UnityEngine;

namespace Items
{
    public interface IPickable
    {
        public GameObject PickUp();
        public void PutDown(RaycastHit hit);
        public bool IsPutDownCorrectPlace { get; }
    }
}