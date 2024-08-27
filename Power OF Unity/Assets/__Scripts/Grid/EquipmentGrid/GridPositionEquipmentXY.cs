using System;

public struct GridPositionEquipmentXY : IEquatable<GridPositionEquipmentXY> //Интерфейс Equatable - Приравниваемый    //Наряду с классами структуры представляют еще один способ создания собственных типов данных в C#. Более того многие примитивные типы, например, int, double и т.д.,
                                                          //по сути являются структурами. При работе с структурой мы передаем копии значений а не ссылку.
                                                          //Мы создаем свою структуру т.к. не можем воспользоваться стандартным Vector2Int. Он работает с X Y а нам нужно X Z (можно было бы сделать преобразование ХУ в XZ туда и обратно, но это добовляет много строк кода и хуже воспринимается)
{
    public int x;
    public int z;
    public EquipmentSlot gridName;

    public GridPositionEquipmentXY(int x, int z, EquipmentSlot gridName) // вспомогательный конструктор
    {
        this.x = x;
        this.z = z;
        this.gridName = gridName;
    }


    public override string ToString() // Переопределим ToString(). Хотим увидеть в отладке Debug.Log внутренее состояние X Z и этаж
    {
        return $"x: {x}; z: {z}; _floor: {gridName}";
    }

    public static bool operator ==(GridPositionEquipmentXY a, GridPositionEquipmentXY b) // Расширение для булевых операций сравнения
    {
        return a.x == b.x && a.z == b.z && a.gridName == b.gridName;
    }

    public static bool operator !=(GridPositionEquipmentXY a, GridPositionEquipmentXY b) // Расширение для булевых операций сравнения
    {
        return !(a == b);
    }

    public static GridPositionEquipmentXY operator +(GridPositionEquipmentXY a, GridPositionEquipmentXY b) // Расширение для суммы
    {
        return new GridPositionEquipmentXY(a.x + b.x, a.z + b.z, a.gridName);
    }

    public static GridPositionEquipmentXY operator -(GridPositionEquipmentXY a, GridPositionEquipmentXY b) // Расширение для разности
    {
        return new GridPositionEquipmentXY(a.x - b.x, a.z - b.z, a.gridName);
    }

    public override bool Equals(object obj) // Автоматически сгенерированное расширение Переопределение равенства
    {
        return obj is GridPositionEquipmentXY position &&
               x == position.x &&
               z == position.z &&
               gridName == position.gridName;
    }

    public override int GetHashCode() // Автоматически сгенерированное расширение
    {
        return HashCode.Combine(x, z, gridName);
    }

    public bool Equals(GridPositionEquipmentXY other) // Реализация интерфейса равенства
    {
        return this == other;
    }
}
