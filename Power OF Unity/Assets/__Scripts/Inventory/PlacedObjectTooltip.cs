/// <summary>
/// Всплывающая подсказка о размещенном объекте.
/// </summary>
public struct PlacedObjectTooltip
{
    public string name;         //имя
    public string description;  //Описание
    public string details;      //Подробности
    public string sideEffects;  //Побочные эффекты

    public PlacedObjectTooltip(string name, string description, string details, string sideEffects)
    {
        this.name = name;
        this.description = description;
        this.details = details;
        this.sideEffects = sideEffects;

    }
}