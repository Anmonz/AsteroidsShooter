using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.AndryKram
{
    /// <summary>
    /// Управляет количество жизней 
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health = 3;//текущее количество
        [SerializeField] private int _maxHealth = 3;//максимальное количестов жизней
        [SerializeField] private int _maxTotalHealth = 6;//максимальное до которого можно увеличить

        [SerializeField] private HealthBarController healthBar;//управление шкалой жизни (Найти другой asset)

        [SerializeField] private UnityEvent _onZeroHealthEvent;//евент окончания жизней
        public UnityEvent OnZeroHealthEvent { get => _onZeroHealthEvent; }

        /// <summary>
        /// Инициализирует количество жизней
        /// </summary>
        public void InitHealthComponent(int health, int maxHealth, int maxTotalHealth)
        {
            this._health = health;
            this._maxHealth = maxHealth;
            this._maxTotalHealth = maxTotalHealth;

            //инициализирует шкалу жизней
            healthBar.InitBar(_maxTotalHealth, _health);
        }

        /// <summary>
        /// Получение урона
        /// </summary>
        public void TakeDamage()
        {
            _health--;
            healthBar.UpdateHeartsHUD(_health, _maxHealth);

            if (_health <=0 )
            {
                _onZeroHealthEvent?.Invoke();
            }
        }

        /// <summary>
        /// Лечение
        /// </summary>
        /// <param name="health"></param>
        public void Heal(int health)
        {
            if (_health + health < _maxHealth)
                _health += health;
            else
                _health = _maxHealth;

            healthBar.UpdateHeartsHUD(_health, _maxHealth);
        }

        /// <summary>
        /// Получение нового сердца в шкалу (увеличение максимального количества жизней)
        /// </summary>
        public void TakeHeart()
        {
            _maxHealth++;
            healthBar.UpdateHeartsHUD(_health, _maxHealth);
        }

        /// <summary>
        /// Удаление сердца из шкалы (уменьшение максимального количества жизней)
        /// </summary>
        public void DeleteHeart()
        {
            _maxHealth--;
            healthBar.UpdateHeartsHUD(_health, _maxHealth);
        }
    }
}
