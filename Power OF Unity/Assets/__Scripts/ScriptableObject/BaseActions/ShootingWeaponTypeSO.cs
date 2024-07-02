using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/ShootingWeaponType")]
public class ShootingWeaponTypeSO : PlacedObjectTypeSO //���������� ������ ������ - ������ ���� SO (��������� ����� ������������ �������)
{   
   
    [SerializeField] private int numberShotInOneAction = 3; // ���������� ��������� �� ���� ��������
    [SerializeField] private float delayShot= 0.2f; //�������� ����� ����������
    [SerializeField] private int maxShootDistance = 7;// ��������� �������� 
    [SerializeField] private int shootDamage = 6; // �������� �����
    [SerializeField] private float percentageShootDistanceIncrease = 0.5f;// ������� ���������� ��������� �������� 
    [SerializeField] private float percentageShootDamageIncrease = 0.5f;//������� ���������� ����� �� �������� 
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<ShootAction>();
    }

    /* public override string GetToolTip()
     {
         return""+ GetName();
     }*/

    public int GetNumberShotInOneAction() { return numberShotInOneAction; }
    public float GetDelayShot() {  return delayShot; }
    public int GetMaxShootDistance() { return maxShootDistance; }
    public float GetShootDamage() {  return shootDamage; }
    public float GetPercentageShootDistanceIncrease() {  return percentageShootDistanceIncrease; }
    public float GetPercentageShootDamageIncrease() {  return percentageShootDamageIncrease; }


}
