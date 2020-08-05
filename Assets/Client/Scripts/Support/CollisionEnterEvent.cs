using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.AndryKram
{
    /// <summary>
    /// Отслеживает столкновение объекта с другими 
    /// </summary>
    public class CollisionEnterEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onCollisionEnterEvent;

        public delegate void OnCollisionEnterDelegate();
        public OnCollisionEnterDelegate OnCollisionEnterDelegateEvent { get; set; }

        private void Awake()
        {
            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null) rigidbody = this.gameObject.AddComponent<Rigidbody>();
            //rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            _onCollisionEnterEvent?.Invoke();
            OnCollisionEnterDelegateEvent?.Invoke();
        }
    }
}
