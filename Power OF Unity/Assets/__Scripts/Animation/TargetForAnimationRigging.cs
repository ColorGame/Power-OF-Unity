using UnityEngine;
/// <summary>
/// ���� ��� ��������� ��������.<br/>
/// ����� �� ������, �������� ��������� ������������ � �������� ����� ���� �� ��������
/// </summary>
public class TargetForAnimationRigging : MonoBehaviour
{
    [SerializeField]private Transform _targetForHandIdleAnimation;

    private void Awake()
    {
        if (_targetForHandIdleAnimation== null)
            _targetForHandIdleAnimation =  transform.Find("TargetForHandIdleAnimation");
    }
    public Transform GetTargetForHandIdleAnimation() {  return _targetForHandIdleAnimation; }
}
