using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Кнопка ДЕЙСТВИЯ. Обрабатываем логику нажатия на кнопку.
/// </summary>
/// <remarks>
/// Прикриплена к кнопке
/// </remarks>
public class ActionButtonUI : MonoBehaviour
{   
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedButtonVisualUI; // Будем включать и выкл. GameObject что бы скрыть или показать рамку кнопки // В инспекторе надо закинуть рамку
    [SerializeField] private Transform _placedObjectVisual;         
   
    private UnitActionSystem _unitActionSystem;
    private PlacedObjectTypeSO _placedObjectTypeSO;

    public void SetMoveAction(MoveAction moveAction, UnitActionSystem unitActionSystem) 
    {     
        _unitActionSystem = unitActionSystem;        
        _button.onClick.AddListener(() =>
        {
            _unitActionSystem.SetSelectedAction(moveAction); //Установить Выбранное Действие
        });
    }

    public void SetBaseActionAndPlacedObjectTypeSO(BaseAction baseAction,PlacedObjectTypeSO placedObjectTypeSO, UnitActionSystem unitActionSystem)
    {
        _placedObjectTypeSO = placedObjectTypeSO;
        _unitActionSystem = unitActionSystem;
       
       /* _placedObjectImage.sprite = placedObjectTypeSO.GetVisual2D();
        _placedObjectImage.rectTransform.localScale = Vector3.one * placedObjectTypeSO.GetScaleImageButton();*/
        
        //Добавим событие при нажатии на нашу кнопку
        _button.onClick.AddListener(() =>
        {
            baseAction.SetPlacedObjectTypeSO(placedObjectTypeSO);
            _unitActionSystem.SetSelectedAction(baseAction); //Установить Выбранное Действие
        });

        InteractableEnable();
    }



    public void UpdateSelectedVisual() // (Обновление визуала) Включение и выключение визуализации выбора.(вызывается событием при выборе кнопки базового действия)
    {
        BaseAction selectedBaseAction = _unitActionSystem.GetSelectedAction(); // Получим выбраное действие
                                                                               //_selectedButtonVisualUI.SetActive(_placedObject == selectedBaseAction.GetPlacedObjectTypeSO());   // Включить рамку если выбранное действие совподает с действием которое мы назначили на нашу кнопку // Если не совподает то получим false и рамка отключиться

        if (_placedObjectTypeSO == selectedBaseAction.GetPlacedObjectTypeSO())
            _button.Select();
        // Можно поменять цвет кнопки при активации кнопки
        /*if (selectedBaseAction == _baseAction) // Если кнопка активна
        {
            _button.image.color = _color; // оставим как есть
        }
        else
        {
            _button.image.color = new Color(_color.r, _color.g, _color.b, 0.5f); // сделаем ее полупрозрачной
        }*/
    }


    //способ скрыть кнопки когда занят действием
    public void InteractableEnable() // Включить взаимодействие
    {
        _button.interactable = true;
       
        UpdateSelectedVisual(); // Обновим отображение рамки кнопки в зависимости от активированного действия
    }

    public void InteractableDesabled() // Отключить взаимодействие // Кнопка становиться не активная и меняет цвет(Настраивается в инспекторе color  Desabled)
    {
        _button.interactable = false;
        _selectedButtonVisualUI.SetActive(false); //Отключим рамку

        /*Color textColor = _textColor; // Сохраним в локальную переменную цвет текста
        textColor.a = 0.1f; // Изменим значение альфа канала
        _textMeshPro.color = textColor; // Изменим текущий цвет текса (сдел прозрачным)*/

    }

    public void HandleStateButton(bool isBusy) // Обработать состояние кнопки
    {
        if (isBusy) // Если занят
        {
            InteractableDesabled(); // Отключить взаимодействие
        }
        else
        {
            InteractableEnable(); // Включить взаимодействие
        }
    }
}
