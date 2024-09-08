using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class Highlight : MonoBehaviour
    {
        [SerializeField] private List<Renderer> _renderers;
        [SerializeField] private List<Material> _materials;

        private void Awake()
        {
            _materials = new List<Material>();
            foreach (var renderer in _renderers)
            {
                _materials.AddRange(new List<Material>(renderer.materials));
            }
        }

        public void EnableWrongPlaceHighlight()
        {
            foreach (var material in _materials)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.red);
            }
        }

        public void EnableСorrectPlaceHighlight()
        {
            foreach (var material in _materials)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.green);
            }
        }

        public void EnableInHandsHighlight()
        {
            Color color = Color.white;
            color.a = 0.5f;
            foreach (var material in _materials)
            {
                material.SetFloat("_Mode", 3);  // 3 - Transparent
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
            }
        }

        public void EnablePickUpHighlight()
        {
            foreach (var material in _materials)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.white);
            }
        }

        public void DisableHighlight()
        {
            foreach (var material in _materials)
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}