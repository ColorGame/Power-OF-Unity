using UnityEngine;
/// <summary>
/// Веревка Юнита
/// </summary>
public class Rope : MonoBehaviour
{
    private RopeRanderer _ropeRanderer;

    private void Awake()
    {
        _ropeRanderer = GetComponentInChildren<RopeRanderer>();
    }

    private void Start()
    {
        HideRope();
    }

    public void ShowRope()
    {
        _ropeRanderer.enabled = true;
    }

    public void HideRope()
    {
        _ropeRanderer.enabled = false;
    }

    public RopeRanderer GetRopeRanderer()
    {
        return _ropeRanderer;
    }
}
