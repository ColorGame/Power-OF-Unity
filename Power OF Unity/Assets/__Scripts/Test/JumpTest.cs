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



    //�����// ��� ������ �����
    private float _timerJumpNormalized; // ��������������� ������ ������ 
    private float _timerJump; // ������ ������ �������
    private float _maxTimerJump; // ������������ ������ ������ 
    private Vector3 _startPosition; // ��������� �������
    private Vector3 _targetPosition;
   
    private float _totalDistance;   //��� ���������. ��������� �� ���� (����� �������� � �����). ��� ����������� �������� ���� ���, � � Update() ��� ���������� �������� ��������� �� ���� ����� �������� �� _totalDistance ��������� �� ���� ��� moveStep (Vector3.Distance-��������� �����)
    private bool _sratJump;

    void Start()
    {       
           _sratJump = false;
        _totalDistance = Vector3.Distance(transform.position, _target1.position);  //�������� ��������� ����� �������� � ����� 
        _maxTimerJump = _totalDistance / _moveSpeed; // �������� ����� ������ ������� = ��������� ������� �� ��������
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
            _totalDistance = Vector3.Distance(_start1.position, _target1.position);  //�������� ��������� ����� �������� � ����� 
            _maxTimerJump = _totalDistance / _moveSpeed; // �������� ����� ������ ������� = ��������� ������� �� ��������
            _timerJump = _maxTimerJump;
            _startPosition = _start1.position;
            _targetPosition = _target1.position;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _totalDistance = Vector3.Distance(_start2.position, _target2.position);  //�������� ��������� ����� �������� � ����� 
            _maxTimerJump = _totalDistance / _moveSpeed; // �������� ����� ������ ������� = ��������� ������� �� ��������
            _timerJump = _maxTimerJump;
            _startPosition = _start2.position;
            _targetPosition = _target2.position;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _totalDistance = Vector3.Distance(_start3.position, _target3.position);  //�������� ��������� ����� �������� � ����� 
            _maxTimerJump = _totalDistance / _moveSpeed; // �������� ����� ������ ������� = ��������� ������� �� ��������
            _timerJump = _maxTimerJump;
            _startPosition = _start3.position;
            _targetPosition = _target3.position;
        }

        if (_sratJump)
        {
            _animator.SetTrigger("JumpDown");
            _timerJump -= Time.deltaTime; // �������� ������ ������ �������

            _timerJumpNormalized = 1 - _timerJump / _maxTimerJump; // ��������  ��������������� ����� ������ ������� (� ������ ������ _timerJump=_maxTimerJump ������ 1-1=0 )

            Vector3 bezier1 = _startPosition + Vector3.up * _hightJump;
            Vector3 bezier2 = new Vector3(_targetPosition.x, bezier1.y, _targetPosition.z);

            // ������� ����� �� ������ ����� � ������ ������ �������
            Vector3 positionBezier = Bezier.GetPoint(
                _startPosition,
                bezier1,
                bezier2,
                _targetPosition,
                _timerJumpNormalized
                );

            transform.position = positionBezier; // ���������� ������ � ��� �����

            if (_timerJump <= 0) // �� ��������� ������� ������ �������...
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
