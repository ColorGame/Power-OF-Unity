using UnityEngine;
/// <summary>
/// ������ ������� ��� ���.
/// ����� // ������ � ����� ���� Cover ���������� ������ �� ������� ���� 1,4�. (��� ������� ������������������ �� Smoke)
/// </summary>
/// <remarks>
/// ���� ������ ���� 1,4� (��� ������ ����� �������� � �������� �����), �� �� ����������� OBSTACLES-������������, �.�. ������ �� ��������������� (�������� ������ �� 0%). 
/// CoverFull -1,4� 
/// CoverHalf -0,7� 
/// </remarks>
public class CoverSmokeObject : MonoBehaviour// 
{
    public enum CoverSmokeType
    {
        None,       //����
        CoverHalf,  //������� �� ��������
        CoverFull,  //������� ������
        SmokeHalf,  //��� �� ��������
        SmokeFull   //��� ������
    }

    [SerializeField] private CoverSmokeType _coverSmokeType;
    [SerializeField] private float _penaltyAccuracy;  // ����� ������������ ������� ��������������� ��� �� �� ����� ���� �������� ������� ������
    private LayerMask _coverLayerMask; // ����� ���� ������� (�������� � ����������)
    private LayerMask _smokeLayerMask; // ����� ���� ���� (�������� � ����������)
    
    private void Start()
    {
        //������� ��������������� ���������.
        //��������� ���. ��� �� ����� �������� ������ ���������(������ �����), ������� ������� ��� ������

        _coverLayerMask = LayerMask.GetMask("Cover");
        _smokeLayerMask = LayerMask.GetMask("Smoke");

        float raycastOffsetDistance = 1.5f; // ��������� �������� ����
        float raycastDistance = 0.5f; // ��������� �������� ����

        if (Physics.Raycast(transform.position + Vector3.up * raycastOffsetDistance, Vector3.down, raycastDistance, _coverLayerMask)) // ��������� ����� � ������ 1,5 ���� �� 0,5� ������ �� ����� �over 
        {
            // ���� �� ������ �� ���  CoverFull-������� ������
            _coverSmokeType = CoverSmokeType.CoverFull;            
        }
        else
        {
            // � ��������� ������ ��� CoverHalf-������� �� ��������
            _coverSmokeType = CoverSmokeType.CoverHalf;           
        }

        if (Physics.Raycast(transform.position + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, _smokeLayerMask)) // ��������� ����� �� ��� ���� �� ����� Smoke 
        {
            // ���� �� ������ �� ���  SmokeFull-��� ������, ��� ������ �� ������ ������ ����� ������ ����� ������ GrenadeSmokeDeactivation
            _coverSmokeType = CoverSmokeType.SmokeFull;
        }      

        _penaltyAccuracy = GetPenaltyFromEnumAccuracy(_coverSmokeType); // ��������� ������� ������ � ����������� �� �������������� ����
    }

    public float GetPenaltyFromEnumAccuracy(CoverSmokeType coverSmokeType) // ������� ����� ������������ � ����������� �� ��������� ������� //����� ���������//
    {
        switch (coverSmokeType)
        {
            case CoverSmokeType.None: // 
                _penaltyAccuracy = 0;
                break;

            case CoverSmokeType.CoverHalf: //������� �� ��������
                _penaltyAccuracy = 0.2f;
                break;

            case CoverSmokeType.CoverFull: //������� ������
                _penaltyAccuracy = 0.6f;
                break;

            case CoverSmokeType.SmokeHalf: //��� �� ��������
                _penaltyAccuracy = 0.25f;
                break;

            case CoverSmokeType.SmokeFull: //��� ������
                _penaltyAccuracy = 0.5f;
                break;
        }
        return _penaltyAccuracy;
    }

    public float GetPenaltyAccuracy() // ������� ����� ������������
    {
        return _penaltyAccuracy;
    }
    public CoverSmokeType GetCoverSmokeType()
    {
        return _coverSmokeType;
    }

    public void SetCoverSmokeType(CoverSmokeType coverType)
    {
        _coverSmokeType = coverType;
    }
}


