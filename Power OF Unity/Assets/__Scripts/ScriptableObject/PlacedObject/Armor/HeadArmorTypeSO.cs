using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HeadArmorType")]
public class HeadArmorTypeSO : PlacedObjectTypeArmorSO
{
    [Header("������ ����������� !!BODY ARMOR")]
    [SerializeField] private List<PlacedObjectType> _compatibleBodyArmorList;

    private HashSet<PlacedObjectType> _compatibleArmorHashList;

    /// <summary>
    /// �������� - ������ ����������� �����
    /// </summary>
    public HashSet<PlacedObjectType> GetCompatibleArmorList()
    {
        if (_compatibleArmorHashList == null)
        {
            _compatibleArmorHashList = new HashSet<PlacedObjectType>(_compatibleBodyArmorList);
        }
        return _compatibleArmorHashList; 
    }
}
