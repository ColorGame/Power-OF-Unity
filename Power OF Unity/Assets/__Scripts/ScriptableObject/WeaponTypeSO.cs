using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/WeaponType")]
public class WeaponTypeSO : PlacedObjectTypeSO // ������ - ������ ���� SO (��������� ����� ������������ �������)
{
   


   
    public int damage; // �������� �����
    public int numberShotInOneAction; // ���������� ��������� �� ���� ��������
    public float delayShot; //�������� ����� ����������
    
    public override string GetToolTip()
    {
        return""+ GetName();
    }

  


}
