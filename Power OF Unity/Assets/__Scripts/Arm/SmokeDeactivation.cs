using UnityEngine;
/// <summary>
/// ����������� ���� �� ������� ����� ���������� �����
/// </summary>
/// <remarks>
/// ������ ������ �� ������� ������ ����
/// </remarks>
public class SmokeDeactivation : MonoBehaviour 
{

    private int _startTurnNumber; // ����� ������� (����) ��� ������ 
    private int _currentTurnNumber; // ������� ����� ������� (����) 
    private TurnSystem _turnSystem;

    private ParticleSystem _particleSystem;
    private CoverSmokeObject _coverSmokeObject; // ������ �������
    private float _rateOverTime = 50;// ��������, � ������� ���������� ��������� ����� ������� � �������� ������� (�� ��������� 200).
    private int _countTurnLowerEffect = 4; // ���������� ����� ��� ���������� �������
    private int _countTurnDestroyObjacte = 6; // ���������� ����� ��� ����������� �������.

    public void Init(TurnSystem turnSystem)
    {
        _turnSystem = turnSystem;

        _particleSystem = GetComponent<ParticleSystem>();
        _coverSmokeObject = GetComponent<CoverSmokeObject>();

        _coverSmokeObject.SetCoverSmokeType(CoverSmokeObject.CoverSmokeType.SmokeFull); // ��������� ������� ���� ������
        _startTurnNumber = _turnSystem.GetTurnNumber(); // ������� ����� ����

        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // ������. �� ������� ��� �������
    }   

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        _currentTurnNumber = _turnSystem.GetTurnNumber(); // ������� ������� ����� ����;

        if (_currentTurnNumber - _startTurnNumber == _countTurnLowerEffect)
        {
            // �� 4 ���� ������������� � ������ ������ �� 50%
            var emission =  _particleSystem.emission;
            emission.rateOverTime = _rateOverTime; // �������� ���������� ����������� ������
            _coverSmokeObject.SetCoverSmokeType(CoverSmokeObject.CoverSmokeType.SmokeHalf); // ��������� ������� ���� �� ��������
        }

        if (_currentTurnNumber - _startTurnNumber == _countTurnDestroyObjacte)
        {
            // �� 6 ���� ��������� ���
            Destroy(gameObject); 
        }
    }

    private void OnDestroy()
    {
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
}
