using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram
{
    /// <summary>
    /// Управление перемещением корабля
    /// </summary>
    public class SpaceshipController : MonoBehaviour
    {
        [SerializeField] private Joystick _joystickMover;    //джойстик управления
        [SerializeField] private Animator _animatorSpaceship;//аниматор корабля
        [SerializeField] private Transform _spaceshipPlayer; //Трансформ корабля
        [SerializeField] private float _spaceshipSpeed = 1f; //скорость корабля
        [SerializeField] private float _spaceshipSize = 1f;  //размер корабля

        private Vector3 _moveVector;    //вектор перемещения корабля
        private Vector3 _leftAngle;     //левая нижняя точка игрового поля 
        private Vector3 _rightAngle;    //правая верхняя точка игрового поля 

        private bool _isUpdateController = false;//метка выполнения обновления корабля

        /// <summary>
        /// Установка скорости корабля
        /// </summary>
        /// <param name="spaceshipSpeed"></param>
        public void SetSpaceshipSpeed(float spaceshipSpeed)
        {
            this._spaceshipSpeed = spaceshipSpeed;
            StartController();
        }

        /// <summary>
        /// Включение перемещения корабля
        /// </summary>
        public void StartController()
        {
            _isUpdateController = true;
        }

        /// <summary>
        /// Отключение перемещения корабля
        /// </summary>
        public void StopController()
        {
            _isUpdateController = false;
        }

        /// <summary>
        /// Устанавливает точки игрового поля
        /// подключается к делегату джойстика
        /// </summary>
        private void Start()
        {
            _leftAngle = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.position.y));
            _rightAngle = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.y));

            _joystickMover.JoystickOnDragDelegate += JoystickOnDrag;
        }
        
        /// <summary>
        /// Выполняется при перемещении джойстика
        /// устанавливает вектор перемещения
        /// </summary>
        public void JoystickOnDrag()
        {
            if (!_isUpdateController) return;

            _moveVector = new Vector3(_joystickMover.Direction.x, 0f, _joystickMover.Direction.y).normalized;
            MoveSpaceship();
        }

        /// <summary>
        /// Передвигает корабль
        /// </summary>
        private void MoveSpaceship()
        {
            ChangeAnimation();

            var newPosition = transform.position + _moveVector * _spaceshipSpeed * Time.deltaTime;
            //проверка края игрового поля
            if (newPosition.x > _leftAngle.x + _spaceshipSize && newPosition.z > _leftAngle.z + _spaceshipSize
                && newPosition.x < _rightAngle.x - _spaceshipSize && newPosition.z < _rightAngle.z - _spaceshipSize)
            {
                _spaceshipPlayer.position = newPosition;
            }
        }

        /// <summary>
        /// Изменение анимации
        /// </summary>
        private void ChangeAnimation()
        {
            _animatorSpaceship.SetFloat("MoveX",_moveVector.x);
        }

#if UNITY_EDITOR
        //Установка вектора через клавишы клавиатуры для Editor'а
        private void Update()
        {
            if (!_isUpdateController) return;

            _moveVector = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                _moveVector += Vector3.forward;
            }

            if(Input.GetKey(KeyCode.S))
            {
                _moveVector += Vector3.back;
            }

            if (Input.GetKey(KeyCode.A))
            {
                _moveVector += Vector3.left;
            }

            if(Input.GetKey(KeyCode.D))
            {
                _moveVector += Vector3.right;
            }

            _moveVector = _moveVector.normalized;

            MoveSpaceship();
        }

#endif
    }
}
