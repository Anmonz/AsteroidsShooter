using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram
{
    /// <summary>
    /// Перемещает текстуру материала по Y
    /// </summary>
    public class ScrollTextureOnMaterial : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private float speed;//скорость перемещения
        private float move;//перемещение

        /// <summary>
        /// Перемещает текстуру
        /// </summary>
        void Update()
        {
            move += Time.deltaTime * speed;
            if (move > 1) { move -= 1; }

            _meshRenderer.material.mainTextureOffset = new Vector2(0, move);
        }
    }
}
