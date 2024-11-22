using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Переключение ВИДИМОСТИ, АНИМАЦИИ меню
/// </summary>
/// <remarks>
/// Абстрактный класс.
/// </remarks>
public abstract class ToggleVisibleAnimatioMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Animator _animator; //Аниматор для меню

    protected GameInput _gameInput;
    protected HashAnimationName _hashAnimationName;
    protected int _animationOpen;
    protected int _animationClose;

    private float _animationTimer;
    private int _stateHashNameAnimation;
    private bool _animationStart = false;
    private bool _toggleBool = false;

    //Делегаты, вызывим при закрытии меню
    private Action _closeDelegate;   // Делегат подписки, для возврата функциональности предыдущему меню
    private Action _unloadDelegate;  // Делегат выгрузки и удаления


    private void Awake()
    {
        HideMenuCallDelegate();
    }

    protected abstract void SetAnimationOpenClose();


    private void Update()
    {
        if (!_animationStart)
            return;

        _animationTimer -= Time.deltaTime; // Запустим таймер

        if (_animationTimer <= 0)
        {
            _animator.enabled = false; // Отключим анимацию что бы вернуть управление в КОД
            _animationStart = false;
            if (_stateHashNameAnimation == _animationClose) // Если сейчас анимация закрывания то
                HideMenuCallDelegate();
        }
    }

    /// <summary>
    /// Подписаться на альтернативное переключение видимости меню (обычно это ESC)
    /// </summary>
    protected void SubscribeAlternativeToggleVisible()
    {
        if (_gameInput != null) // При загрузки _gameInput еще не иницилизирован 
            _gameInput.OnMenuAlternate += GameInput_OnMenuAlternateToggleVisible;
    }
    /// <summary>
    /// Отписаться от альтернативного переключение видимости меню
    /// </summary>
    protected void UnsubscribeAlternativeToggleVisible()
    {
        if (_gameInput != null) // При загрузки _gameInput еще не иницилизирован 
            _gameInput.OnMenuAlternate -= GameInput_OnMenuAlternateToggleVisible;
    }
    private void GameInput_OnMenuAlternateToggleVisible(object sender, EventArgs e)
    {
        ToggleVisible();
    }
    /// <summary>
    /// Переключатель видимости меню. 
    /// </summary>
    /// <remarks>
    /// Если надо переключить без анимации то передаем withAnimation = false
    /// </remarks>
    public void ToggleVisible(bool withAnimation = true) // Переключатель видимости меню НАСТРОЙКИ (будем вызывать через инспектор кнопкой OptionsButton)
    {  
        if (!_toggleBool) // Если не активны(выключена) то активируем 
        {
            ShowMenu();

            if (withAnimation && _animator != null) // Если есть анимация то настроим и запустим ее
            {
                _stateHashNameAnimation = _animationOpen;
                StartAnimation();
            }
        }
        else
        {
            if (withAnimation && _animator != null) // Если есть анимация то настроим и запустим ее
            {
                _stateHashNameAnimation = _animationClose;
                StartAnimation();
            }
            else // В противном случае просто скроем меню
            {
                HideMenuCallDelegate();
            }
        }
    }
    /// <summary>
    /// Установить делегат, который вызовиться при закрытии меню
    /// </summary>
    /// <remarks>
    /// Обычно это подписка на альтернативный вызов меню.
    /// </remarks>
    public void SetCloseDelegate(Action closeDelegate)
    {
        _closeDelegate = closeDelegate;
    }
    /// <summary>
    /// Установить делегат выгрузки и удаления
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
    /// Скроем меню. Вызовим делегат
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
    /// Если есть не нулевой делегат вызовим его и очистим.
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
