using UnityEngine;

public enum MarketState
{
    Buy,
    Sell
}

/// <summary>
/// ����� (���������� � ��������) 
/// </summary>
public class MarketUI : MonoBehaviour
{

    /// <summary>
    /// ����� ���� ������� (�������� �� �����)
    /// </summary>
    private uint _sumAllBuy;
    /// <summary>
    /// ����� ���� ������ (����������� �� ���)
    /// </summary>
    private uint _sumAllSell;

    private WarehouseManager _warehouseManager;

    /// <summary>
    /// ��������� �������� ���������� �������
    /// </summary>
    public bool TryAddBuyCount(PlacedObjectTypeSO placedObjectTypeSO)
    {
        // �������� ����� + ���� ������� (������� ����� ��������) <= ���� ����� ���������� (���� ������ �� � ��� ���������� �������)
        return _sumAllBuy + placedObjectTypeSO.GetPriceBuy() <= _warehouseManager.GetCountCoin();
    }
}
