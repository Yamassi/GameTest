using System;
using UnityEngine;

namespace Player
{
    public interface IInput
    {
        public event Action<Vector2> MoveDirection;
        public event Action<Vector2> LookUpMoveDirection;
        public event Action<RaycastHit> LookUpRayCast;
        public event Action<RaycastHit> ClickRaycast;
        public void Initialize(Transform transform);
        public void Clear();
    }
}