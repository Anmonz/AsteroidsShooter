using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace com.AndryKram
{
    /// <summary>
    /// Панель изменяемая под паузу/проигрыш/выйгрыш
    /// </summary>
    public class LevelMenuPanel : MonoBehaviour
    {
        [SerializeField] private Image _panel;//панель
        [SerializeField] private Text _nameMenu;//имя панели
        [SerializeField] private Text _scoreText;//количество очков
        [SerializeField] private Button _buttonToMenu;//кнопка выхода в меню
        [SerializeField] private Button _buttonRestart;//кнопка перезапуска
        [SerializeField] private List<ColorStylePanel> _styles;//список стилей стили

        /// <summary>
        /// OnClick на кнопке вызова меню
        /// </summary>
        public Button.ButtonClickedEvent OnClickButtonToMenu { get => _buttonToMenu.onClick; }
        /// <summary>
        /// OnClick на кнопке перезапуска
        /// </summary>
        public Button.ButtonClickedEvent OnClickButtonRestart { get => _buttonRestart.onClick; }

        /// <summary>
        /// Инициализация панели
        /// </summary>
        /// <param name="namePanel">Строка имени панели</param>
        /// <param name="isActivateScore">Метка включения очков</param>
        /// <param name="countScore">количество очков</param>
        /// <param name="indexStyle">индекс стиля</param>
        public void InitMenuPanel(string namePanel,bool isActivateScore, int countScore, int indexStyle)
        {
            //установка имени панели
            _nameMenu.text = namePanel;

            //включение записи о количестве очков
            if (isActivateScore)
            {
                _scoreText.enabled = true;
                _scoreText.text = "Score: " + countScore.ToString();
            }
            else
            {
                _scoreText.enabled = false;
            }
            //установка стиля 
            _nameMenu.color = _styles[indexStyle].textColor;
            _scoreText.color = _styles[indexStyle].textColor;
            _buttonToMenu.GetComponent<Image>().color = _styles[indexStyle].buttonColor;
            _buttonRestart.GetComponent<Image>().color = _styles[indexStyle].buttonColor;
            if(indexStyle == 2)
                _buttonRestart.GetComponentInChildren<Text>().text = "CONTINUE";
            else
                _buttonRestart.GetComponentInChildren<Text>().text = "RESTART";
            _panel.color = _styles[indexStyle].panelColor;
        }

        /// <summary>
        /// Вспомагательная структура стиля панели
        /// </summary>
        [Serializable]
        private struct ColorStylePanel
        {
            public Color textColor;
            public Color buttonColor;
            public Color panelColor;
        }
    }
}
