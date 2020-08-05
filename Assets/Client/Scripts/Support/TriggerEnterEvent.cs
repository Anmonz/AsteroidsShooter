using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.AndryKram
{
    /// <summary>
    /// Отслеживает столкновение триггера
    /// </summary>
    public class TriggerEnterEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onTriggerEnterEvent;

        public delegate void OnTriggerEnterDelegate();
        public OnTriggerEnterDelegate OnTriggerEnterDelegateEvent { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            _onTriggerEnterEvent?.Invoke();
            OnTriggerEnterDelegateEvent?.Invoke();
        }
    }
}