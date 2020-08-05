
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram
{
    /// <summary>
    /// Содержит информацию о уровне
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelScriptableObject", order = 1)]
    public class LevelScriptableObject : ScriptableObject
    {
        [Header("Win Data")]
        [SerializeField] private bool _isWinOnTimeLived = true;//метка выйгрыша по времени
        [SerializeField] [Range(1, 30)] private int _minutesLiveForWin = 1; //количество времени 
        [SerializeField] private bool _isWinOnObstaclesDestroy = false;//метка выйгрыша по уничтожению врагов
        [SerializeField] private int _destroyObstaclesFOrWin = 10;//количество врагов

        [Header("Spaceship")]
        [SerializeField] private int _healthSpaceship = 3;//текущии жизни корабля
        [SerializeField] private int _maxHealthSpaceship = 3;//максимально доступные жизни игрока
        [SerializeField] private int _maxTotalHealthSpaceship = 6;//максимум до увеличения жизней
        [SerializeField] private float _speedSpaceship = 1f;//скорость корабля

        [Header("Obstacles")]
        [SerializeField] [Range(1f, 100f)] private float _frequencySpawnObstacles = 1f;//частота появления препядствий
        [SerializeField] private float _speedObstacles = 1f;//скорость препядствий

        [Header("Bullet")]
        [SerializeField] private float _bulletSpeed = 1f;//скорость пуль
        [SerializeField] private float _reloadTime = 0.1f;//время перезарядки
        [SerializeField] private bool _isInfinityFire = true;//метка стрельбы

        public int HealthSpaceship {get => _healthSpaceship; }
        public int MaxHealthSpaceship { get => _maxHealthSpaceship; }
        public int MaxTotalHealthSpaceship { get => _maxTotalHealthSpaceship; }
        public float SpeedSpaceship { get => _speedSpaceship; }

        public float FrequencySpawnObstacles { get => _frequencySpawnObstacles; }
        public float SpeedObstacles { get => _speedObstacles; }

        public float BulletSpeed { get => _bulletSpeed; }
        public float ReloadTime { get => _reloadTime; }
        public bool IsInfinityFire { get => _isInfinityFire; }

        public bool IsWinOnTimeLived { get => _isWinOnTimeLived; }
        public int MinutesLiveForWin { get => _minutesLiveForWin; }
        public bool IsWinOnObstaclesDestroy { get => _isWinOnObstaclesDestroy; }
        public int DestroyObstaclesFOrWin { get => _destroyObstaclesFOrWin; }
    }
}
