using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace com.AndryKram
{
    /// <summary>
    /// Управляет летящими препядствиями
    /// </summary>
    public class ObstaclesManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _obstaclesPrefabs;//список префабов препядствий
        [SerializeField] private GameObject _explosionPrefab;//перфаб взрыва
        [SerializeField] [Range(1f, 100f)] private float _frequencySpawnObstacles = 1f;//частота появления препядствий
        [SerializeField] private float _speedObstacles = 1f;//скорость препядствий
        [SerializeField] private int _sizePoolObstacles = 20;//размер пула препядствий
        [SerializeField] private bool _growSizePoolObstacles = false;//метка увеличения размера пула препядствий
        [SerializeField] private bool _isJobsActivate = false;
        [SerializeField] private float _offsetHeightSpawnLine = 2f;//смещение линии спавна объектов на величину
        [SerializeField] private float _offsetWeightSpawnLine = 1f;//уменьшение линии спавна объектов на величину

        private bool _isDoUpdateCycle = false;//метка выполнения цикла спавна и перемещения объектов

        private List<Obstacle> _poolObstacles;//пул препядствий

        private Vector3 _leftTopAngleGameSpace;//левый верхний угол экрана
        private Vector3 _rightTopAngleGameSpace;//правый верхний угол экана

        private float _distanceDestroyObstacleDownScreen;//растояние после экрана для уничтожения объектов

        private float _timeSpawnObstacles = 0f;//время до спавна объектов

        /// <summary>
        /// Запускает цикл перемещения и спавна объекта
        /// </summary>
        public void StartUpdate()
        {
            _isDoUpdateCycle = true;
            foreach(Obstacle obstacle in _poolObstacles)
            {
                if (!obstacle.isHide) obstacle.transformObject.gameObject.SetActive(true);
            }
        }
        /// <summary>
        /// Останавливает цикл перемещения и спавна объекта
        /// </summary>
        public void StopUpdate()
        {
            _isDoUpdateCycle = false;
            foreach (Obstacle obstacle in _poolObstacles)
            {
                if (!obstacle.isHide) obstacle.transformObject.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Инициализация менеджера контроля препядствий
        /// </summary>
        /// <param name="frequencySpawnObstacles">частота спавна</param>
        /// <param name="speedObstacles">скорость перемещения</param>
        public void InitObstaclesManager(float frequencySpawnObstacles, float speedObstacles)
        {
            this._frequencySpawnObstacles = frequencySpawnObstacles;
            this._speedObstacles = speedObstacles;

            //вычисления углов экрана
            _leftTopAngleGameSpace = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height, Camera.main.transform.position.y)) + Vector3.forward * _offsetHeightSpawnLine + Vector3.right* _offsetWeightSpawnLine;
            _rightTopAngleGameSpace = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.y)) + Vector3.forward * _offsetHeightSpawnLine + Vector3.left*_offsetWeightSpawnLine;

            //вычисление дистанции отключения объектов через нижний край экрана
            _distanceDestroyObstacleDownScreen = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.position.y)).z - _offsetHeightSpawnLine*2;

            //Проверка на существование пула объектов
            if (_poolObstacles == null)
            {
                //создание пула
                _poolObstacles = new List<Obstacle>();
            }
            else
            {
                //обнуление пула
                foreach (Obstacle obstacle in _poolObstacles)
                {
                    obstacle.isHide = true;
                    obstacle.transformObject.gameObject.SetActive(false);
                }
            }

            //запуск цикла
            StartUpdate();
        }

        /// <summary>
        /// Цикл перемещения и создания объектов
        /// </summary>
        private void Update()
        {
            //проверка на выполнение цикла
            if(_isDoUpdateCycle)
            {
                //Проверка на создание нового объекта
                if(_timeSpawnObstacles <= 0f)
                {
                    SpawnObstacle();
                    _timeSpawnObstacles = 1f / _frequencySpawnObstacles;
                }
                else
                {
                    _timeSpawnObstacles -= Time.deltaTime;
                }

                //Проверка на использование Job System (не дает прироста, мало объектов)
                if (_isJobsActivate)
                {
                    NativeArray<bool> isHideObstaclesArray = new NativeArray<bool>(_poolObstacles.Count, Allocator.TempJob);
                    TransformAccessArray transformAccessArray = new TransformAccessArray(_poolObstacles.Count);

                    for (int i = 0; i < _poolObstacles.Count; i++)
                    {
                        isHideObstaclesArray[i] = _poolObstacles[i].isHide;
                        transformAccessArray.Add(_poolObstacles[i].transformObject);
                    }

                    ObstacleMoveParrallelJob obstacleMoveParrallelJob = new ObstacleMoveParrallelJob()
                    {
                        _deltaTime = Time.deltaTime,
                        _moveDirectionObstacles = this.transform.forward * _speedObstacles,
                        _isHideObstaclesArray = isHideObstaclesArray,
                        _destroyDistanceObstacles = _distanceDestroyObstacleDownScreen
                    };

                    JobHandle jobHandle = obstacleMoveParrallelJob.Schedule(transformAccessArray);
                    jobHandle.Complete();

                    for (int i = 0; i < _poolObstacles.Count; i++)
                    {
                        _poolObstacles[i].isHide = isHideObstaclesArray[i];
                    }

                    isHideObstaclesArray.Dispose();
                    transformAccessArray.Dispose();
                }
                else
                {
                    //перемещение объектов
                    foreach (Obstacle obstacle in _poolObstacles)
                    {
                        if (!obstacle.isHide)
                        {
                            obstacle.transformObject.position += this.transform.forward * _speedObstacles * Time.deltaTime;
                            if (obstacle.transformObject.position.z <= _distanceDestroyObstacleDownScreen)
                                obstacle.isHide = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Спавн новых и объектов
        /// </summary>
        private void SpawnObstacle()
        {
            foreach(Obstacle obstacle in _poolObstacles)
            {
                if(obstacle.isHide)
                {
                    obstacle.transformObject.position = Vector3.Lerp(_leftTopAngleGameSpace, _rightTopAngleGameSpace, UnityEngine.Random.Range(0f, 1f));
                    obstacle.isHide = false;
                    obstacle.transformObject.gameObject.SetActive(true);
                    return;
                }
            }

            if (_growSizePoolObstacles || _poolObstacles.Count < _sizePoolObstacles) CreateObstacle();
        }

        /// <summary>
        /// Создание нового объекта
        /// </summary>
        private void CreateObstacle()
        {
            //выбор случайного из списка, установка случайной точки на линии спавна
            GameObject obstacle = Instantiate(_obstaclesPrefabs[UnityEngine.Random.Range(0, _obstaclesPrefabs.Count)],
                                  Vector3.Lerp(_leftTopAngleGameSpace, _rightTopAngleGameSpace, UnityEngine.Random.Range(0f, 1f)),
                                  Quaternion.identity, this.transform);

            Obstacle newObstacle = new Obstacle() { transformObject = obstacle.transform, isHide = false };
            
            //создание и подключение к событию столкновения с объектом
            obstacle.AddComponent<CollisionEnterEvent>().OnCollisionEnterDelegateEvent += delegate () { HideObstacle(newObstacle); };

            _poolObstacles.Add(newObstacle);

        }

        /// <summary>
        /// Создание нового объекта взврыва в пуле
        /// </summary>
        /// <param name="position"></param>
        private void CreateExplosion(Vector3 position)
        {
            Instantiate(_explosionPrefab, position, Quaternion.identity, this.transform);
        }

        /// <summary>
        /// Обрабатывает событие столкновения с объектом
        /// скрывает объект пула и останавливает случайную скорость перемещения объекта 
        /// </summary>
        /// <param name="obstacle"></param>
        private void HideObstacle(Obstacle obstacle)
        {
            obstacle.isHide = true;
            obstacle.transformObject.gameObject.SetActive(false);
            CreateExplosion(obstacle.transformObject.position);
            obstacle.transformObject.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        /// <summary>
        /// Доп класс для хранения данных в пуле
        /// </summary>
        private class Obstacle
        {
            public Transform transformObject;
            public bool isHide;
        }

        private class Explosion
        {
            public Transform transformObject;
            public ParticleSystem particleSystem;
        }
    }

    /// <summary>
    /// Структура для Job System для выполнения перемещения объектов
    /// </summary>
    [BurstCompile]
    public struct ObstacleMoveParrallelJob : IJobParallelForTransform
    {
        public NativeArray<bool> _isHideObstaclesArray;//массив с метками скрытых объектво пула
        [ReadOnly] public float3 _moveDirectionObstacles;//вектор направления перемещения
        [ReadOnly] public float _destroyDistanceObstacles;//дистанция уничтожения объектов
        [ReadOnly] public float _deltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            if(!_isHideObstaclesArray[index])
            {
                transform.position += new Vector3(_moveDirectionObstacles.x, 0f, _moveDirectionObstacles.z) * _deltaTime;
                if (transform.position.z <= _destroyDistanceObstacles)
                    _isHideObstaclesArray[index] = true;
            }
        }
    }
}
