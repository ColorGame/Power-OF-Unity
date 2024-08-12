using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Переключение ВИДИМОСТИ, АНИМАЦИИ, ПОДПИСКИ меню
/// </summary>
/// <remarks>
/// Абстрактный класс.
/// </remarks>
public abstract class ToggleVisibleAnimatioSubscribeMenuUI : MonoBehaviour
{
    [Header("Фон который блокирует\nвзаимодействие с задним фоном")]
    [SerializeField] private Image _backgroundBlockRaycast;

    protected GameInput _gameInput;
    protected HashAnimationName _animBase = new HashAnimationName();
    protected int _animationOpen;
    protected int _animationClose;

    private Animator _animator; //Аниматор для меню
    private float _animationTimer;
    private int _stateHashNameAnimation;
    private bool _animationStart = false;
    private bool _toggleBool = false;

    private Action _menuClosed; // Делегат будем вызывать при закрытии меню


    private void Awake()
    {
        if (TryGetComponent(out Animator animator))
        {
            _animator = animator;
        }

        SetAnimationOpenClose();
        HideMenuCallDelegate();
    }

    protected abstract void SetAnimationOpenClose();

    private void OnEnable()
    {
        SubscribeAlternativeToggleVisible();
    }
    private void OnDisable()
    {
        UnsubscribeAlternativeToggleVisible();
    }

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
    private void GameInput_OnMenuAlternateToggleVisible(object sender, System.EventArgs e)
    {
        ToggleVisible();
    }
    /// <summary>
    /// Переключатель видимости меню. Принимает делегат, который вызовим и обнулим, при закрытии этого меню (обычно это подписка на альтернативный вызов меню)
    /// </summary>
    /// <remarks>
    /// Если передается делегат = null то он не переписывает предыдущий. Если надо переключить без анимации то передаем withAnimation = false
    /// </remarks>
    public void ToggleVisible(Action menuClosed = null, bool withAnimation = true) // Переключатель видимости меню НАСТРОЙКИ (будем вызывать через инспектор кнопкой OptionsButton)
    {
        if (menuClosed != null)
            _menuClosed = menuClosed;

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
        _toggleBool = !_toggleBool;
    }

    private void ShowMenu()
    {
        gameObject.SetActive(true);
        _backgroundBlockRaycast.enabled = true;
    }
    /// <summary>
    /// Скроем меню. Если есть не нулевой делегат вызовим его и обнулим.
    /// </summary>
    private void HideMenuCallDelegate()
    {
        gameObject.SetActive(false);
        _backgroundBlockRaycast.enabled = false;
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

}
