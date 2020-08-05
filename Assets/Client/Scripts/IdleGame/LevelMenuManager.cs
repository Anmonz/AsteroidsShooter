using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace com.AndryKram
{
    /// <summary>
    /// управляет GUI уровня
    /// </summary>
    public class LevelMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _idleGameMenu;//объект родителя GUI игры
        [SerializeField] private GameObject _openGameMenu;//объект родителя GUI всплывающей панели

        [SerializeField] private Text _textScore;//текст количества очков
        [SerializeField] private Text _textTime;//текст прошедшего времени
        [SerializeField] private Button _pauseButton;//кнопка паузы

        [SerializeField] private AudioSource _audioChangePanel;//звук смены панелей
        [SerializeField] private AudioSource _audioWinPanel;//звук выйгрыша
        [SerializeField] private AudioSource _audioFailPanel;//звукпроигрыша

        public UnityEvent OnClickButtonPause { get => _pauseButton.onClick; } //событие нажатия кнопки паузы

        public delegate void OnEventMenu();//делегат
        private OnEventMenu _onClickButtonToMenu;//вызов выхода в меню
        private OnEventMenu _onClickButtonRestart;//вызов перезапуска уровня
        private OnEventMenu _onClickButtonContinue;//вызов продолжения игры

        private GameObject _currentMenu;//текущий открытая GUI
        private LevelMenuPanel _openGameMenuPanel;//панель уровня
        private int _currentScore;//текущее количество очков
        private MenusIdleGame _codeCurrentMenu;//код текущей панели

        /// <summary>
        /// инициализирует меню менеджер
        /// </summary>
        private void Awake()
        {
            //установка текущим меню игровое
            _currentMenu = _idleGameMenu;
            _codeCurrentMenu = MenusIdleGame.idle;

            //подключение свободной панели
            _openGameMenuPanel = _openGameMenu.GetComponent<LevelMenuPanel>();
            _openGameMenuPanel.OnClickButtonToMenu.AddListener(OnClickButtonToMenuOpenPanel);
            _openGameMenuPanel.OnClickButtonRestart.AddListener(OnClickButtonRestartOpenPanel);
        }

        /// <summary>
        /// коды возможных панелей GUI уровня
        /// </summary>
        public enum MenusIdleGame { idle, win, fall, pause}

        /// <summary>
        /// Смена панели на другую
        /// </summary>
        /// <param name="menu"></param>
        public void ChangeMenuTo(MenusIdleGame menu)
        {
            //скрытие текущей панели
            _currentMenu.SetActive(false);
            _codeCurrentMenu = menu;

            //переключение на новую панель и ее настройка
            switch (menu)
            {
                case MenusIdleGame.idle:
                    _currentMenu = _idleGameMenu;
                    break;
                case MenusIdleGame.win:
                    _audioWinPanel.Play();
                    _openGameMenuPanel.InitMenuPanel("YOU WIN", true, _currentScore,0);
                    _currentMenu = _openGameMenu;
                    break;
                case MenusIdleGame.fall:
                    _audioFailPanel.Play();
                    _openGameMenuPanel.InitMenuPanel("YOU DIED", true, _currentScore, 1);
                    _currentMenu = _openGameMenu;
                    break;
                case MenusIdleGame.pause:
                    _openGameMenuPanel.InitMenuPanel("PAUSE", false, _currentScore, 2);
                    _currentMenu = _openGameMenu;
                    break;
            }
            //включение новой панели
            _audioChangePanel.Play();
            _currentMenu.SetActive(true);
        }

        /// <summary>
        /// Обновление GUI уровня (очков и времения)
        /// </summary>
        /// <param name="score"></param>
        /// <param name="time"></param>
        public void UpdateGUI(int score, int time)
        {
            _currentScore = score;
            _textScore.text = score.ToString();
            _textTime.text = time / 60 + ":" + time % 60;
        }

        /// <summary>
        /// регистрирует слушателя делегата выхода в меню
        /// </summary>
        public void RegisterOnClickButtonToMenu(OnEventMenu onEvent) => _onClickButtonToMenu += onEvent;
        /// <summary>
        /// регистрирует слушателя делегата перезапуска уровня
        /// </summary>
        public void RegisterOnClickButtonRestart(OnEventMenu onEvent) => _onClickButtonRestart += onEvent;
        /// <summary>
        /// регистрирует слушателя делегата продолжения уровня
        /// </summary>
        public void RegisterOnClickButtonContinue(OnEventMenu onEvent) => _onClickButtonContinue += onEvent;
        /// <summary>
        /// отключает слушателя делегата выхода в меню
        /// </summary>
        public void UnregisterOnClickButtonToMenu(OnEventMenu onEvent) => _onClickButtonToMenu -= onEvent;
        /// <summary>
        /// отключает слушателя делегата перезапуска уровня
        /// </summary>
        public void UnregisterOnClickButtonRestart(OnEventMenu onEvent) => _onClickButtonRestart -= onEvent;
        /// <summary>
        /// отключает слушателя делегата продолжения уровня
        /// </summary>
        public void UnregisterOnClickButtonContinue(OnEventMenu onEvent) => _onClickButtonContinue -= onEvent;

        /// <summary>
        /// Обрабатывает нажатие кнопки выхода в меню
        /// </summary>
        private void OnClickButtonToMenuOpenPanel()
        {
            //вызов выхода в меню
            _onClickButtonToMenu?.Invoke();
        }

        /// <summary>
        /// обрабатывает нажатие кнопки перезапуска
        /// </summary>
        private void OnClickButtonRestartOpenPanel()
        {
            //при паузе вызов паузы
            if (_codeCurrentMenu == MenusIdleGame.pause)
                _onClickButtonContinue?.Invoke();
            else//иначе вызов перезапуска
                _onClickButtonRestart?.Invoke();
        }
    }
}
