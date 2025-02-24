using UnityEngine;
/// <summary>
/// Цель для настройки анимации.<br/>
/// Висит на ОРУЖИИ, содержит параметры расположения и поворота левой руки на прекладе
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
