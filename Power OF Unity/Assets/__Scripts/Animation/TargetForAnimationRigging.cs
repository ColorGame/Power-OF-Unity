using UnityEngine;
/// <summary>
/// ���� ��� ��������� ��������
/// </summary>
public class TargetForAnimationRigging : MonoBehaviour
{
    [SerializeField]private Transform _targetForHandIdleAnimation;

    private void Awake()
    {
        if (_targetForHandIdleAnimation)
            _targetForHandIdleAnimation =  transform.Find("TargetForHandIdleAnimation");
    }
    public Transform GetTargetForHandIdleAnimation() {  return _targetForHandIdleAnimation; }
}
