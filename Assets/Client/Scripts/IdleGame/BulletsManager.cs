using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram
{
    /// <summary>
    /// Управление пулом пуль
    /// </summary>
    public class BulletsManager : MonoBehaviour
    {
        [SerializeField] private GameObject _spaceshipPlayer;       //корабль  
        [SerializeField] private List<Transform> _gunsPositions;    //список позиций для спавна пуль
        [SerializeField] private GameObject _bulletPrefab;          //префаб пули  
        [SerializeField] private float _bulletSpeed = 1f;           //скорость пуль
        [SerializeField] private float _reloadTime = 0.1f;          //время перезарядки
        [SerializeField] private bool _isInfinityFire = true;       //метка стрельбы бесконечно
        [SerializeField] private int _sizePoolBullets = 20;         //размер пула пуль
        [SerializeField] private bool _growSizePoolBullets = false; //метка увеличения размера пула
        [SerializeField] private AudioSource _audioGunShoot;        //звук выстрела

        private bool _isDoUpdateCycle = false;//метка цикла спавна и перемещения пуль

        private List<Bullet> _poolBullets;//пул пуль
        private int _currentGun = 0;//текущее место спавна пуль

        private float _distanceDestroyBulletTopScreen = 10f;//дистанция уничтожения пуль
        private float _offsetHeightDestroyLine = -0.1f;//смещение от верха экрана для дистанции уничтожения пуль

        private float _timeOnShoot = 0f;//время до нового выстрела

        public delegate void OnDestroyBullet();//делегат события уничтожения пуль
        private OnDestroyBullet _onDestroyBulletEvent;//экземпляр делегата события уничтожения пуль

        /// <summary>
        /// Добавить к событию уничтожения пуль слушателя
        /// </summary>
        public void AddListenerOnDestroyBullet(OnDestroyBullet onDestroyBullet)
        {
            _onDestroyBulletEvent += onDestroyBullet;
        }


        /// <summary>
        /// Убрать у событию уничтожения пуль слушателя
        /// </summary>
        public void RemoveListenerOnDestroyBullet(OnDestroyBullet onDestroyBullet)
        {
            _onDestroyBulletEvent -= onDestroyBullet;
        }

        /// <summary>
        /// Запуск цикла перемещения и спавна пуль
        /// Отображение живых пуль
        /// </summary>
        public void StartUpdate()
        {
            foreach(Bullet bullet in _poolBullets)
            {
                if (!bullet.isHide) bullet.transformBullet.gameObject.SetActive(true);
            }
            _isDoUpdateCycle = true;
        }

        /// <summary>
        /// Остановка цикла перемещения и спавна пуль
        /// Скрытие живых пуль
        /// </summary>
        public void StopUpdate()
        {
            foreach (Bullet bullet in _poolBullets)
            {
                if (!bullet.isHide) bullet.transformBullet.gameObject.SetActive(false);
            }
            _isDoUpdateCycle = false;
        }

        /// <summary>
        /// Инициализация менеджера управления пулом пуль
        /// </summary>
        /// <param name="bulletSpeed">скорость пуль</param>
        /// <param name="reloadTime">время перезарядки</param>
        /// <param name="isInfinityFire">метка постоянной стрельбы</param>
        public void InitBulletsManager(float bulletSpeed, float reloadTime, bool isInfinityFire)
        {
            this._bulletSpeed = bulletSpeed;
            this._reloadTime = reloadTime;
            this._isInfinityFire = isInfinityFire;

            if (_spaceshipPlayer == null) _spaceshipPlayer = this.gameObject;
            if (_gunsPositions.Count == 0) _gunsPositions.Add(_spaceshipPlayer.transform);

            //Вычисление растояния для уничтожения пуль от края экрана
            _distanceDestroyBulletTopScreen = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height, Camera.main.transform.position.y)).z + _offsetHeightDestroyLine;

            //создание или обновление пула
            if (_poolBullets == null)
            {
                _poolBullets = new List<Bullet>();
            }
            else
            {
                foreach(Bullet bullet in _poolBullets)
                {
                    bullet.isHide = true;
                    bullet.transformBullet.gameObject.SetActive(false);
                }
            }

            StartUpdate();
        }

        /// <summary>
        /// Постоянное перемещение пуль и создание новых
        /// </summary>
        private void Update()
        {
            if (!_isDoUpdateCycle) return;

            if (!_isInfinityFire) return;

            if (_timeOnShoot <= 0f)
            {
                _audioGunShoot.Play();
                SpawnBullets();
                _timeOnShoot = _reloadTime;
            }
            else
            {
                _timeOnShoot -= Time.deltaTime;
            }

            foreach (Bullet bullet in _poolBullets)
            {
                if (!bullet.isHide)
                {
                    bullet.transformBullet.position += bullet.transformBullet.forward * _bulletSpeed * Time.deltaTime;
                    if (bullet.transformBullet.position.z >= _distanceDestroyBulletTopScreen)
                    {
                        bullet.transformBullet.gameObject.SetActive(false);
                        bullet.isHide = true;
                    }
                }
            }
        }

        /// <summary>
        /// Спавн свободных пуль на точка спавна или создание нового
        /// </summary>
        private void SpawnBullets()
        {
            //выбор точки спавна
            NextGun();

            //проверка свободных пуль
            foreach (Bullet bullet in _poolBullets)
            {
                if (bullet.isHide)
                {
                    bullet.transformBullet.position = this?._gunsPositions[_currentGun].position ?? _gunsPositions[0].position;
                    bullet.isHide = false;
                    bullet.transformBullet.gameObject.SetActive(true);
                    return;
                }
            }

            //Если свободные пуле не найдены и пул не заполнен создание новой пули
            if (_growSizePoolBullets || _poolBullets.Count < _sizePoolBullets) CreateBullets();
        }

        /// <summary>
        /// Создает новую пулю в пуле
        /// </summary>
        private void CreateBullets()
        {
            GameObject bullet = Instantiate(this?._bulletPrefab,
                                  this?._gunsPositions[_currentGun].position ?? _gunsPositions[0].position,
                                  Quaternion.identity, this.transform);

            Bullet newBullet = new Bullet() { transformBullet = bullet.transform, isHide = false };

            //создание анонимного метода на делегате столкновения объектов
            bullet.AddComponent<CollisionEnterEvent>().OnCollisionEnterDelegateEvent += delegate () { HideBullet(newBullet); };

            _poolBullets.Add(newBullet);
        }

        /// <summary>
        /// Обработка делегата столкновения с объектом
        /// </summary>
        /// <param name="bullet"></param>
        private void HideBullet(Bullet bullet)
        {
            //вызво делегата уничтожения пули
            _onDestroyBulletEvent?.Invoke();

            //освобождение пули для пула
            bullet.isHide = true;
            bullet.transformBullet.gameObject.SetActive(false);
        }

        /// <summary>
        /// Выбор следующего места спавна пуль
        /// </summary>
        private void NextGun()
        {
            _currentGun++;
            if (_currentGun >= _gunsPositions.Count) _currentGun = 0;
        }

        /// <summary>
        /// Вспомогательный класс для хранения объектов в пуле
        /// </summary>
        private class Bullet
        {
            public Transform transformBullet;
            public bool isHide;
        }
    }
}
