using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.AndryKram
{
    /// <summary>
    /// Кнопка списка уровней в стартовом меню
    /// </summary>
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private Button _button;        //кнопка
        [SerializeField] private Text _levelName;       //текст названия уровня
        [SerializeField] private Text _levelLabel;      //текст сообщения к уровню
        [SerializeField] private Text _levelScoreLabel; //текст количество очков набраных на уровне

        public delegate void OnButtonClick(int index);//делегат передающий int
        private OnButtonClick onButtonClickEvent;//делегат на нажатие кнопки

        private int _levelIndex;//индекс уровня на который указывает кнопка

        /// <summary>
        /// Установка информации об уровне на кнопку
        /// </summary>
        /// <param name="index">Номер уровня</param>
        /// <param name="label">Сообщение к уровню</param>
        /// <param name="score">колличество очков</param>
        public void SetLevelButton(int index, string label, int score)
        {
            //Установка названия
            _levelName.text = "LEVEL " + (index + 1).ToString();
            //Создание анонимного метода при нажатии кнопки на вызов делегата с передачей индекса уровня
            _levelIndex = index;
            _button.onClick.AddListener(delegate { onButtonClickEvent?.Invoke(_levelIndex); });

            //Установка данных об уровне
            _levelLabel.text = label;
            if(score >= 0) _levelScoreLabel.text = "SCORE: " + score;
            else _levelScoreLabel.text = "";
        }

        /// <summary>
        /// Подключение слушателей к вызову нажатия кнопки
        /// </summary>
        /// <param name="buttonClickListener"></param>
        public void AddListener(OnButtonClick buttonClickListener)
        {
            onButtonClickEvent += buttonClickListener;
        }

        /// <summary>
        /// Отключение слушателя от вызова нажатия кнопки
        /// </summary>
        /// <param name="buttonClickListener"></param>
        public void RemoveListener(OnButtonClick buttonClickListener)
        {
            onButtonClickEvent -= buttonClickListener;
        }
    }
}
