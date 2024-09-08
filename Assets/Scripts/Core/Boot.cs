using UnityEngine;

namespace Core
{
    public class Boot : MonoBehaviour
    {
        [SerializeField] private Player.Player _player;

        private void Awake()
        {
            _player.Initialize();
        }
    }
}