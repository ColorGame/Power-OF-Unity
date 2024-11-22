using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������������ ���������, �������� ����
/// </summary>
/// <remarks>
/// ����������� �����.
/// </remarks>
public abstract class ToggleVisibleAnimatioMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Animator _animator; //�������� ��� ����

    protected GameInput _gameInput;
    protected HashAnimationName _hashAnimationName;
    protected int _animationOpen;
    protected int _animationClose;

    private float _animationTimer;
    private int _stateHashNameAnimation;
    private bool _animationStart = false;
    private bool _toggleBool = false;

    //��������, ������� ��� �������� ����
    private Action _closeDelegate;   // ������� ��������, ��� �������� ���������������� ����������� ����
    private Action _unloadDelegate;  // ������� �������� � ��������


    private void Awake()
    {
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
    private void GameInput_OnMenuAlternateToggleVisible(object sender, EventArgs e)
    {
        ToggleVisible();
    }
    /// <summary>
    /// ������������� ��������� ����. 
    /// </summary>
    /// <remarks>
    /// ���� ���� ����������� ��� �������� �� �������� withAnimation = false
    /// </remarks>
    public void ToggleVisible(bool withAnimation = true) // ������������� ��������� ���� ��������� (����� �������� ����� ��������� ������� OptionsButton)
    {  
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
    /// <summary>
    /// ���������� �������, ������� ���������� ��� �������� ����
    /// </summary>
    /// <remarks>
    /// ������ ��� �������� �� �������������� ����� ����.
    /// </remarks>
    public void SetCloseDelegate(Action closeDelegate)
    {
        _closeDelegate = closeDelegate;
    }
    /// <summary>
    /// ���������� ������� �������� � ��������
    /// </summary>
    public void SetUnloadDelegate(Action unloadDelegate)
    {
        _unloadDelegate = unloadDelegate;
    }

    private void ShowMenu()
    {
        _canvas.enabled = true;
        SubscribeAlternativeToggleVisible();
        _toggleBool = true;
    }
    /// <summary>
    /// ������ ����. ������� �������
    /// </summary>
    private void HideMenuCallDelegate()
    {
        _canvas.enabled = false;
        _toggleBool = false;
        UnsubscribeAlternativeToggleVisible();
        CallDelegateAndClear();
    }

    private void StartAnimation()
    {
        _animator.enabled = true;
        _animator.CrossFade(_stateHashNameAnimation, 0);
        _animationTimer = _animator.GetCurrentAnimatorStateInfo(0).length;
        _animationStart = true;
    }
    /// <summary>
    /// ���� ���� �� ������� ������� ������� ��� � �������.
    /// </summary>
    private void CallDelegateAndClear()
    {
        if(_closeDelegate != null)
        {
            _closeDelegate();
            _closeDelegate=null;
        }
        if (_unloadDelegate != null)
        {
            _unloadDelegate();
            _unloadDelegate = null;
        }
    }

    private void OnDestroy()
    {
        UnsubscribeAlternativeToggleVisible();
        if (_unloadDelegate != null)
        {
            _unloadDelegate();
            _unloadDelegate = null;
        }
    }

}
