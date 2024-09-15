using System;
using UnityEngine;

/// <summary>
/// ������������ ���������, �������� ����
/// </summary>
/// <remarks>
/// ����������� �����.
/// </remarks>
public abstract class ToggleVisibleAnimatioMenuUI : MonoBehaviour
{
    protected GameInput _gameInput;
    protected HashAnimationName _hashAnimationName;
    protected int _animationOpen;
    protected int _animationClose;

    private Animator _animator; //�������� ��� ����
    private float _animationTimer;
    private int _stateHashNameAnimation;
    private bool _animationStart = false;
    private bool _toggleBool = false;

    private Action _menuClosed; // ������� ����� �������� ��� �������� ����

    private Canvas _canvas;

    private void Awake()
    {
        if (TryGetComponent(out Animator animator))
        {
            _animator = animator;
        }
        _canvas = GetComponentInParent<Canvas>(true);
       
        HideMenuCallDelegate();
    }
       
    protected abstract void SetAnimationOpenClose();


    private void Update()
    {
        if (!_animationStart)
            return;

        _animationTimer -= Time.deltaTime; // �������� ������

        if (_animationTimer <= 0)
        {
            _animator.enabled = false; // �������� �������� ��� �� ������� ���������� � ���
            _animationStart = false;
            if (_stateHashNameAnimation == _animationClose) // ���� ������ �������� ���������� ��
                HideMenuCallDelegate();
        }
    }

    /// <summary>
    /// ����������� �� �������������� ������������ ��������� ���� (������ ��� ESC)
    /// </summary>
    protected void SubscribeAlternativeToggleVisible()
    {
        if (_gameInput != null) // ��� �������� _gameInput ��� �� �������������� 
            _gameInput.OnMenuAlternate += GameInput_OnMenuAlternateToggleVisible;
    }
    /// <summary>
    /// ���������� �� ��������������� ������������ ��������� ����
    /// </summary>
    protected void UnsubscribeAlternativeToggleVisible()
    {
        if (_gameInput != null) // ��� �������� _gameInput ��� �� �������������� 
            _gameInput.OnMenuAlternate -= GameInput_OnMenuAlternateToggleVisible;
    }
    private void GameInput_OnMenuAlternateToggleVisible(object sender, System.EventArgs e)
    {
        ToggleVisible();
    }
    /// <summary>
    /// ������������� ��������� ����. ��������� �������, ������� ������� � �������, ��� �������� ����� ���� (������ ��� �������� �� �������������� ����� ����)
    /// </summary>
    /// <remarks>
    /// ���� ���������� ������� = null �� �� �� ������������ ����������. ���� ���� ����������� ��� �������� �� �������� withAnimation = false
    /// </remarks>
    public void ToggleVisible(Action menuClosed = null, bool withAnimation = true) // ������������� ��������� ���� ��������� (����� �������� ����� ��������� ������� OptionsButton)
    {
        if (menuClosed != null)
            _menuClosed = menuClosed;

        if (!_toggleBool) // ���� �� �������(���������) �� ���������� 
        {
            ShowMenu();

            if (withAnimation && _animator != null) // ���� ���� �������� �� �������� � �������� ��
            {
                _stateHashNameAnimation = _animationOpen;
                StartAnimation();
            }
        }
        else
        {
            if (withAnimation && _animator != null) // ���� ���� �������� �� �������� � �������� ��
            {
                _stateHashNameAnimation = _animationClose;
                StartAnimation();
            }
            else // � ��������� ������ ������ ������ ����
            {
                HideMenuCallDelegate();
            }                
        }
    }

    private void ShowMenu()
    {      
        _canvas.enabled = true;
        SubscribeAlternativeToggleVisible();
        _toggleBool=true;       
    }
    /// <summary>
    /// ������ ����. ���� ���� �� ������� ������� ������� ��� � �������.
    /// </summary>
    private void HideMenuCallDelegate()
    {        
        _canvas.enabled = false;
        UnsubscribeAlternativeToggleVisible();
        _toggleBool = false;
      
        if (_menuClosed != null)
        {
            _menuClosed();
            _menuClosed = null;
        }
    }

    private void StartAnimation()
    {
        _animator.enabled = true;
        _animator.CrossFade(_stateHashNameAnimation, 0);
        _animationTimer = _animator.GetCurrentAnimatorStateInfo(0).length;
        _animationStart = true;
    }

    private void OnDestroy()
    {
        UnsubscribeAlternativeToggleVisible();
    }

}
