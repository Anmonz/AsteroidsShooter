using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.AndryKram
{
    /// <summary>
    /// Управление текущим игровым уровнем
    /// </summary>
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelsScriptableObject _levelsData;        //информация о всех уровнях игры
        [SerializeField] private SpaceshipController _spaceshipController;  //управление кораблем
        [SerializeField] private ObstaclesManager _obstaclesManager;        //Управление спавном препядствий
        [SerializeField] private BulletsManager _bulletsManager;            //Управление спавном пуль
        [SerializeField] private HealthComponent _healthComponentSpaceship; //Управление жизнями корабля
        [SerializeField] private LevelMenuManager _levelMenuManager;        //Управление GUI уровня

        [SerializeField] private float _fadeTime = 0.5f;                    //время фейда при загрузке другой сцены

        private LevelScriptableObject _currentLevelData;//информация о текущем уровне

        private bool _isWinOnTimeLived = false;//метка условия победы по времени
        private int _minutesLiveForWin = 1;//минут для победы
        private bool _isWinOnObstaclesDestroy = false;//метка условия победы по уничтоженым препятствиям
        private int _destroyObstaclesFOrWin = 10;//препятствий для победы

        private float _gameTime;//время игры
        private int _currentDestroyedObstacles = 0;//текущее количество уничтоженных препятствий
        private int _currentScore;//текущее количество очков

        private bool _isStopGame = true;//метка остановки игры

        private bool _isEndWinTime = false;//метка выполнения условия победы по времени
        private bool _isEndWinDestroyed = false;//метка выполнения условия победы по уничтоженым препятствиям

        /// <summary>
        /// проверяет наличие данных о текущем уровне
        /// есть - инициализирует уровень
        /// нет - выходит в меню игры
        /// </summary>
        private void Start()
        {
            if (PlayerPrefs.HasKey("CurrentLevelNumber"))
                _currentLevelData = _levelsData.GetLevel(PlayerPrefs.GetInt("CurrentLevelNumber"));

            if (_currentLevelData != null)
            {
                InitLevel();
                AddListeners();
            }
            else
            {
                Initiate.Fade("StartMenu", Color.black, _fadeTime);
            }
        }

        /// <summary>
        /// обновляет время игры
        /// обновляет GUI очков и времени
        /// проверяет победу по времени
        /// </summary>
        private void Update()
        {
            if (!_isStopGame)
            {
                _gameTime += Time.deltaTime;
                _levelMenuManager.UpdateGUI(_currentScore = Convert.ToInt32(_gameTime) + _currentDestroyedObstacles*10, (int)_gameTime);
                if (_isWinOnTimeLived && !_isEndWinTime) CheckWinState();
            }
        }

        /// <summary>
        /// обработка остановки приложения
        /// вызов паузы
        /// </summary>
        /// <param name="pause"></param>
        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                OnGamePause();
            }
        }

        /// <summary>
        /// Инициализирует уровень
        /// </summary>
        private void InitLevel()
        {
            //количества жизней
            _healthComponentSpaceship.InitHealthComponent(_currentLevelData.HealthSpaceship, _currentLevelData.MaxHealthSpaceship, _currentLevelData.MaxTotalHealthSpaceship);
            //данные о корабле
            _spaceshipController.SetSpaceshipSpeed(_currentLevelData.SpeedSpaceship);
            //данные о менеджере препятствий
            _obstaclesManager.InitObstaclesManager(_currentLevelData.FrequencySpawnObstacles, _currentLevelData.SpeedObstacles);
            //данные о менеджере пуль
            _bulletsManager.InitBulletsManager(_currentLevelData.BulletSpeed, _currentLevelData.ReloadTime, _currentLevelData.IsInfinityFire);

            //условия победы
            _isWinOnTimeLived = _currentLevelData.IsWinOnTimeLived;
            _minutesLiveForWin = _currentLevelData.MinutesLiveForWin;

            _isWinOnObstaclesDestroy = _currentLevelData.IsWinOnObstaclesDestroy;
            _destroyObstaclesFOrWin = _currentLevelData.DestroyObstaclesFOrWin;

            //вспомогательные данные
            _gameTime = 0;
            _currentDestroyedObstacles = 0;
            _currentScore = 0;

            _isStopGame = false;

            _isEndWinTime = false;
            _isEndWinDestroyed = false;
        }

        /// <summary>
        /// Подключается к делегатам и событиям 
        /// </summary>
        private void AddListeners()
        {
            //окончание жизней
            _healthComponentSpaceship.OnZeroHealthEvent.AddListener(OnEndLifeSpaceship);
            //Уничтожение пуль
            _bulletsManager.AddListenerOnDestroyBullet(OnDestroyObstacles);

            //нажатие кнопок на GUI
            _levelMenuManager.OnClickButtonPause.AddListener(OnGamePause);
            _levelMenuManager.RegisterOnClickButtonContinue(OnGameStart);
            _levelMenuManager.RegisterOnClickButtonRestart(OnGameRestart);
            _levelMenuManager.RegisterOnClickButtonToMenu(OnGameLoadMainMenu);
        }

        /// <summary>
        /// Подсчитывет количество уничтоженных препятствий
        /// </summary>
        private void OnDestroyObstacles()
        {
            if (!_isStopGame)
            {
                _currentDestroyedObstacles++;
                if (_isWinOnObstaclesDestroy && !_isEndWinDestroyed) CheckWinState();
            }
        }
        
        /// <summary>
        /// Проверяет выполнение условий победы
        /// </summary>
        private void CheckWinState()
        {
            //Оба условия
            if (_isWinOnTimeLived && _isWinOnObstaclesDestroy)
            {
                if (_gameTime >= _minutesLiveForWin * 60)
                {
                    _isEndWinTime = true;
                }
                else return;

                if (_currentDestroyedObstacles >= _destroyObstaclesFOrWin)
                {
                    _isEndWinDestroyed = true;
                }
                else return;
            }//По времени
            else if (_isWinOnTimeLived)
            {
                if (_gameTime >= _minutesLiveForWin * 60)
                {
                    _isEndWinTime = true;
                }
                else return;
            }//по уничтоженным препятствиям
            else if (_isWinOnObstaclesDestroy)
            {
                if (_currentDestroyedObstacles >= _destroyObstaclesFOrWin)
                {
                    _isEndWinDestroyed = true;
                }
                else return;
            }
            else return;

            //запись открытия нового уровня
            if (PlayerPrefs.GetInt("OpenLevelNumber") < PlayerPrefs.GetInt("CurrentLevelNumber") + 1)
                PlayerPrefs.SetInt("OpenLevelNumber", PlayerPrefs.GetInt("CurrentLevelNumber")+1);
            
            //остановка игры и вызова панели победы
            StopGame();
            _levelMenuManager.ChangeMenuTo(LevelMenuManager.MenusIdleGame.win);
        }

        /// <summary>
        /// Остановка игры
        /// </summary>
        private void StopGame()
        {
            _isStopGame = true;
            //остановка контроллеров
            _spaceshipController.StopController();
            _obstaclesManager.StopUpdate();
            _bulletsManager.StopUpdate();

            SaveStateGame();
        }
 
        /// <summary>
        /// Запуск игры
        /// </summary>
        private void StartGame()
        {
            _isStopGame = false;

            _spaceshipController.StartController();
            _obstaclesManager.StartUpdate();
            _bulletsManager.StartUpdate();
        }

        /// <summary>
        /// Сохранение данных уровня
        /// </summary>
        private void SaveStateGame()
        {
            //запись колличества очков
            if (PlayerPrefs.HasKey("ScoreLevel" + PlayerPrefs.GetInt("CurrentLevelNumber")))
            {
                if (_currentScore > PlayerPrefs.GetInt("ScoreLevel" + PlayerPrefs.GetInt("CurrentLevelNumber")))
                {
                    PlayerPrefs.SetInt("ScoreLevel" + PlayerPrefs.GetInt("CurrentLevelNumber"), _currentScore);
                }
            }
            else 
                PlayerPrefs.SetInt("ScoreLevel" + PlayerPrefs.GetInt("CurrentLevelNumber"), _currentScore);
            
            //сохранение
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Обрабатывает событие окончания жизней
        /// останавливает игру и вызывает панель проигрыша
        /// </summary>
        private void OnEndLifeSpaceship()
        {
            StopGame();
            _levelMenuManager.ChangeMenuTo(LevelMenuManager.MenusIdleGame.fall);
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки паузы
        /// останавливает игры и вызывает панель паузы
        /// </summary>
        private void OnGamePause()
        {
            StopGame();
            _levelMenuManager.ChangeMenuTo(LevelMenuManager.MenusIdleGame.pause);
        }

        /// <summary>
        /// Обрабатывает событие снятия паузы
        /// запускает игру и вызывает GUI уровня
        /// </summary>
        private void OnGameStart()
        {
            StartGame();
            _levelMenuManager.ChangeMenuTo(LevelMenuManager.MenusIdleGame.idle);
        }

        /// <summary>
        /// Обрабатывает событие рестарта уровня
        /// инициализирует и запускает игру и вызывает GUI уровня
        /// </summary>
        private void OnGameRestart()
        {
            InitLevel();
            StartGame();
            _levelMenuManager.ChangeMenuTo(LevelMenuManager.MenusIdleGame.idle);
        }

        /// <summary>
        /// Обрабатывает событие выхода в меню
        /// загружает стартовую сцену
        /// </summary>
        private void OnGameLoadMainMenu()
        {
            SaveStateGame();
            Initiate.Fade("StartMenu", Color.black, _fadeTime);
        }

    }
}
