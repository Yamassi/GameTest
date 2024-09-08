using UnityEngine;

namespace Items
{
    public class Brick : MonoBehaviour, IPickable
    {
        public GameObject PickUp() => gameObject;
    }
}