using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace com.AndryKram
{
    /// <summary>
    /// Управляет стартовым меню
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private LevelsScriptableObject _levelsScriptableObject;//информация обо всех уровнях
        [SerializeField] private float _fadeTime = 0.5f;//время фейда при переключении уровня

        [Header("Start Menu")]
        [SerializeField] private GameObject _startMenu;//начальная панель
        [SerializeField] private Button _btnPlayGame;//кнопка запуска игры

        [Header("Levels Menu")]
        [SerializeField] private GameObject _levelsMenu;//панель уровней
        [SerializeField] private Transform _levelsListContent;//родитель списка уровней
        [SerializeField] private GameObject _winLevelBtnPrefab;//префаб кнопки с пройденным уровнем
        [SerializeField] private GameObject _openLevelBtnPrefab;//префаб открытого уровня
        [SerializeField] private GameObject _lockLevelBtnPrefab;//префаб закрытого уровня

        [Header("Audios")]
        [SerializeField] private AudioSource _audioChangePanel;//звук смены панелей
        [SerializeField] private AudioSource _audioBtnClick;//звук нажатия на кнопку

        /// <summary>
        /// подгрузка списка уровней и подключения кнопок
        /// </summary>
        private void Awake()
        {
            //подключение к прослушиванию нажатия кнопки на переключение в меню уровней анонимным методом
            _btnPlayGame.onClick.AddListener(delegate () { _startMenu.SetActive(false); _levelsMenu.SetActive(true); _audioChangePanel.Play(); });
            
            //получение количества уровней
            int countLevels = _levelsScriptableObject.GetCountLevels();
            int opneLevel = 0;
            
            //получение количество открытых уровней
            if (PlayerPrefs.HasKey("OpenLevelNumber"))
            {
                opneLevel = PlayerPrefs.GetInt("OpenLevelNumber");
            }
            else
            {
                PlayerPrefs.SetInt("OpenLevelNumber", 0); 
            }
             
            //заполнение списка уровней
            for(int i = 0; i < countLevels; i++)
            {
                
                if(i < opneLevel)//уровень пройден
                {
                    //создание кнопки и ее установка
                    var button = Instantiate(_winLevelBtnPrefab, _levelsListContent).GetComponent<LevelButton>();
                    button.SetLevelButton(i, GetLevelLabel(i), PlayerPrefs.GetInt("ScoreLevel" + i));
                    button.AddListener(OpenLevel);
                }
                else if(i == opneLevel) //уровень октрыт
                {
                    //создание кнопки и ее установка
                    var button = Instantiate(_openLevelBtnPrefab, _levelsListContent).GetComponent<LevelButton>();

                    if(PlayerPrefs.HasKey("ScoreLevel" + i))
                        button.SetLevelButton(i, GetLevelLabel(i), PlayerPrefs.GetInt("ScoreLevel" + i));
                    else
                        button.SetLevelButton(i, GetLevelLabel(i), 0);

                    button.AddListener(OpenLevel);
                }
                else //уровень закрыт
                {
                    //создание кнопки и ее установка
                    Instantiate(_lockLevelBtnPrefab, _levelsListContent).GetComponent<LevelButton>().SetLevelButton(i, GetLevelLabel(i),-1);
                }
            }
        }

        /// <summary>
        /// Составляет информацию о прохождении уровня
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetLevelLabel(int index)
        {
            var level = _levelsScriptableObject.GetLevel(index);
            //метка выйгрыша по уничтожению 
            if(level.IsWinOnObstaclesDestroy)
            {
                var label = "Destroy " + level.DestroyObstaclesFOrWin.ToString() + " obstacles";
                //метка выйгрыша по времени
                if (level.IsWinOnTimeLived)
                {
                    label += " and survive " + level.MinutesLiveForWin.ToString() + " minutes";
                }
                return label;
            }
            else if(level.IsWinOnTimeLived)//метка выйгрыша по времени
            {
                return "Survive " + level.MinutesLiveForWin.ToString() + " minutes";
            }
            else //нет меток
            {
                return "Free fun";
            }
        }

        /// <summary>
        /// Загружает выбранный уровенб
        /// </summary>
        /// <param name="index"></param>
        private void OpenLevel(int index)
        {
            _audioBtnClick.Play();
            PlayerPrefs.SetInt("CurrentLevelNumber", index);
            Initiate.Fade("IdleGame", Color.black, _fadeTime);
        }
    }
}
