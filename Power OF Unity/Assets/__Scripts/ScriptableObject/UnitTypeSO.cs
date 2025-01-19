using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ScriptableObject, ��������� ������ �������������� ����� ��� ������. 
/// </summary>
/// <remarks>
/// abstract - ������ ������� ��������� ������� ������.
/// �� �������� ��, ������ �����, ������ ������������ ��������, ����� ��� "UnitFriendSO"  "UnitEnemySO".
/// </remarks>

public abstract class UnitTypeSO : ScriptableObject//, ISerializationCallbackReceiver//ISerializationCallbackReceiver ��������� ��� ��������� �������� ������� ��� ������������ � ��������������.����� ������������ ��� �������� ID ��� ������������
                                                   //������������� ���������� ������� ������ ��������� ������� (� ��������� ����������� ���������) � �����. �������������� ��� ������� �������� ������������ � �� ������,
                                                   //������� ��������� � ������, �� ����� ������������ ��������� ������� � ������������ ���� ������ � ������ �����.
{
    /* [Header("���.����. UUID ��� ����/����.\n�������� ��� ����, ���� �� ������ ����-�� �����.")]
     [SerializeField] private string _unitID = null;*/
    [SerializeField] private string _name;   // ��� �����
    [Range(0, 100)]
    [SerializeField] private int _basicHealth; // ��������
    [Range(0, 100)]
    [SerializeField] private int _basicActionPoints; // ���� ��������
    [Range(0, 100)]
    [SerializeField] private int _basicPower; // ���� ��������
    [Range(0, 100)]
    [SerializeField] private int _basicAccuracy; // ��������
    [Header("��������� ��������.\n���������� � ����� �����, �������� ���� � ������ ������")]
    [SerializeField] private int _basicMoveDistance = Constant.MOVE_DISTSNCE_MAX;
    [Header("������ ���� ����� �� ���������")]
    [SerializeField] protected Transform _unitCorePrefab;

    public string GetName() { return _name; }
    public int GetBasicHealth() { return _basicHealth; }
    public int GetBasicActionPoints() { return _basicActionPoints; }
    public int GetBasicPower() { return _basicPower; }
    public int GetBasicAccuracy() { return _basicAccuracy; }
    public int GetBasicMoveDistance() { return _basicMoveDistance; }
    public Transform GetUnitCorePrefab() { return _unitCorePrefab; }

    [ContextMenu("��������� ��������")]
    private void SetRandomValue()
    {
        _basicHealth = UnityEngine.Random.Range(0, 100);
        _basicActionPoints = UnityEngine.Random.Range(0, 100);
        _basicPower = UnityEngine.Random.Range(0, 100);
        _basicAccuracy = UnityEngine.Random.Range(0, 100);

    }
   
    protected virtual void AutoCompletion()
    {
        SetRandomValue();
    }

   

    /* /// <summary>
     /// �������� ��������� SO �� ��� UUID.
     /// </summary>
     public static UnitTypeSO GetFromID(string itemID)
     {

     }*/




    /*  void ISerializationCallbackReceiver.OnBeforeSerialize()
      {
          // ������������ � ��������� ����� UUID, ���� �� ���� ��� ��� ������ ������ �������.
          if (string.IsNullOrWhiteSpace(_unitID))
          {
              _unitID = Guid.NewGuid().ToString();
          }
      }

      void ISerializationCallbackReceiver.OnAfterDeserialize()
      {
          // ��������� ISerializationCallbackReceiver, �� ��� �� ����� ������ � ���� ������.
      }*/
}