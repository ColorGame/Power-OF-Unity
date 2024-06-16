using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/WeaponType")]
public class WeaponTypeSO : PlacedObjectTypeSO // ������ - ������ ���� SO (��������� ����� ������������ �������)
{

    [SerializeField] private int _shootDistance; // ��������� �������� 
    [SerializeField] private int damage; // �������� �����
    [SerializeField] private int numberShotInOneAction; // ���������� ��������� �� ���� ��������
    [SerializeField] private float delayShot; //�������� ����� ����������
    
   /* public override string GetToolTip()
    {
        return""+ GetName();
    }*/

  


}
