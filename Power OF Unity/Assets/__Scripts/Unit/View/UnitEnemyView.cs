public class UnitEnemyView : UnitView
{
    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
       //оставим пустым т.к. у врага не меняется броня головы
    }

    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        //оставим пустым т.к. у врага не меняется броня тела
    }
}
