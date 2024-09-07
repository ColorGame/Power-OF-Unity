using UnityEngine;

public enum MarketState
{
    Buy,
    Sell
}

/// <summary>
/// Рынок (экипировки и ресурсов) 
/// </summary>
public class MarketUI : MonoBehaviour
{

    /// <summary>
    /// Сумма всех покупок (спишется со счета)
    /// </summary>
    private uint _sumAllBuy;
    /// <summary>
    /// Сумма всех продаж (зачислиться на сет)
    /// </summary>
    private uint _sumAllSell;

    private WarehouseManager _warehouseManager;

    /// <summary>
    /// Попробуем добавить количество покупок
    /// </summary>
    public bool TryAddBuyCount(PlacedObjectTypeSO placedObjectTypeSO)
    {
        // теккущая сумма + цена объекта (который хотим добавить) <= всех наших сбережений (если истина то у нас достаточно средств)
        return _sumAllBuy + placedObjectTypeSO.GetPriceBuy() <= _warehouseManager.GetCountCoin();
    }
}
