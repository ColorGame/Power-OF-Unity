using UnityEngine;

public class JumpTest : MonoBehaviour
{
    [SerializeField] private Transform _target1;
    [SerializeField] private Transform _target2;
    [SerializeField] private Transform _target3;
    [SerializeField] private Transform _start1;
    [SerializeField] private Transform _start2;
    [SerializeField] private Transform _start3;
    public int _moveSpeed;    
    [SerializeField] private float _hightJump;
    [SerializeField] private Animator _animator;



    //БЕЗЬЕ// Для кривой БИЗЬЕ
    private float _timerJumpNormalized; // Нормализованное Таймер полета 
    private float _timerJump; // Таймер полета гранаты
    private float _maxTimerJump; // Максимальное Таймер полета 
    private Vector3 _startPosition; // Стартовая позиция
    private Vector3 _targetPosition;
   
    private float _totalDistance;   //Вся дистанция. Дистанция до цели (между гранатой и целью). Для оптимизации вычислим один раз, а в Update() для вычисления текущего растояния до цели будем отнимать от _totalDistance проиденый за кадр шаг moveStep (Vector3.Distance-затратный метод)
    private bool _sratJump;

    void Start()
    {       
           _sratJump = false;
        _totalDistance = Vector3.Distance(transform.position, _target1.position);  //Вычислим дистанцию между гранатой и целью 
        _maxTimerJump = _totalDistance / _moveSpeed; // Вычислим время полета гранаты = растояние поделим на скорость
        _timerJump = _maxTimerJump;
        _startPosition = _start1.position;
        _targetPosition = _target1.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _sratJump = true;

           
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _totalDistance = Vector3.Distance(_start1.position, _target1.position);  //Вычислим дистанцию между гранатой и целью 
            _maxTimerJump = _totalDistance / _moveSpeed; // Вычислим время полета гранаты = растояние поделим на скорость
            _timerJump = _maxTimerJump;
            _startPosition = _start1.position;
            _targetPosition = _target1.position;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _totalDistance = Vector3.Distance(_start2.position, _target2.position);  //Вычислим дистанцию между гранатой и целью 
            _maxTimerJump = _totalDistance / _moveSpeed; // Вычислим время полета гранаты = растояние поделим на скорость
            _timerJump = _maxTimerJump;
            _startPosition = _start2.position;
            _targetPosition = _target2.position;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _totalDistance = Vector3.Distance(_start3.position, _target3.position);  //Вычислим дистанцию между гранатой и целью 
            _maxTimerJump = _totalDistance / _moveSpeed; // Вычислим время полета гранаты = растояние поделим на скорость
            _timerJump = _maxTimerJump;
            _startPosition = _start3.position;
            _targetPosition = _target3.position;
        }

        if (_sratJump)
        {
            _animator.SetTrigger("JumpDown");
            _timerJump -= Time.deltaTime; // запустим таймер полета гранаты

            _timerJumpNormalized = 1 - _timerJump / _maxTimerJump; // Вычислим  Нормализованное Время полета гранаты (в начале броска _timerJump=_maxTimerJump значит 1-1=0 )

            Vector3 bezier1 = _startPosition + Vector3.up * _hightJump;
            Vector3 bezier2 = new Vector3(_targetPosition.x, bezier1.y, _targetPosition.z);

            // Получим точку на кривой Безье в данный момент времени
            Vector3 positionBezier = Bezier.GetPoint(
                _startPosition,
                bezier1,
                bezier2,
                _targetPosition,
                _timerJumpNormalized
                );

            transform.position = positionBezier; // Переместим снаряд в эту точку

            if (_timerJump <= 0) // по истечении таймера полета гранаты...
            {
               _animator.SetTrigger("Land");
                _sratJump = false ;
                _timerJump = _maxTimerJump;
            }       

        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = _startPosition;
        }
    }
}
