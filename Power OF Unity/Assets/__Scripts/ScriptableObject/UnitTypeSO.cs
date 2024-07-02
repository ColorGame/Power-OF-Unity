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

public abstract class UnitTypeSO : ScriptableObject, ISerializationCallbackReceiver//ISerializationCallbackReceiver ��������� ��� ��������� �������� ������� ��� ������������ � ��������������.����� ������������ ��� �������� ID ��� ������������
                                                                                   //������������� ���������� ������� ������ ��������� ������� (� ��������� ����������� ���������) � �����. �������������� ��� ������� �������� ������������ � �� ������,
                                                                                   //������� ��������� � ������, �� ����� ������������ ��������� ������� � ������������ ���� ������ � ������ �����.
{
    [Header("���.����. UUID ��� ����/����.\n�������� ��� ����, ���� �� ������ ����-�� �����.")]
    [SerializeField] private string _unitID = null;
    [SerializeField] private string _name;   // ��� �����
    [SerializeField] private int _basicHealth; // ��������
    [SerializeField] private int _basicActionPoints = Constant.ACTION_POINTS_MAX; // ���� ��������
    [Header("��������� ��������.\n���������� � ����� �����, �������� ���� � ������ ������")]
    [SerializeField] private int _basicMoveDistance = Constant.MOVE_DISTSNCE_MAX;
    [Header("������ ���� ����� �� ���������")]
    [SerializeField] private Transform _unitCorePrefab;  

    public string GetName() { return _name; }
    public int GetBasicHealth() { return _basicHealth; }
    public int GetBasicActionPoints() { return _basicActionPoints; }
    public int GetBasicMoveDistance() { return _basicMoveDistance; }
    public Transform GetUnitCorePrefab() { return _unitCorePrefab; }
        
    /// <summary>
    /// �������� ������ ���������� ������� �������� ��� ����� �����
    /// </summary>
    public BaseAction[] GetValidBaseActionArray()
    {
        return _unitCorePrefab.GetComponents<BaseAction>();
    }


    /* /// <summary>
     /// �������� ��������� SO �� ��� UUID.
     /// </summary>
     public static UnitTypeSO GetFromID(string itemID)
     {

     }*/


   

    void ISerializationCallbackReceiver.OnBeforeSerialize()
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
    }
}