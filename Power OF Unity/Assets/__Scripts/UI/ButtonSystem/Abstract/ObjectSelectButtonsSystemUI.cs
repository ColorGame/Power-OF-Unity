using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ �������
/// </summary>
public abstract class ObjectSelectButtonsSystemUI : MonoBehaviour, IToggleActivity
{
    protected Transform[] _containerArray; // ������ �����������
    protected Transform _activeContainer;
    protected Image[] _buttonSelectedImageArray; // ������ ����������� ��� ��������� ������ ������      
    protected ScrollRect _scrollRect; //��������� ��������� ������
    protected Canvas _canvas;
    protected TooltipUI _tooltipUI;
  

    protected virtual void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);       
    }

    protected void Setup()
    {
        ClearAllContainer();
        SetDelegateContainerSelectionButton();
    }

    /// <summary>
    /// ���������� ������� ������ ������ ����������
    /// </summary>
    protected abstract void SetDelegateContainerSelectionButton();

    public abstract void SetActive(bool active);

    protected void ShowSelectedButton(Image typeButtonSelectedImage) // �������� ���������� ������
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // ��������� ������ 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// ���� ��� ���������� ��� ����������� �� ������� ���
        }
    }

    /// <summary>
    ///  �������� � �������� ��������� (� �������� �������� ������ ��������� ������)
    /// </summary>
    protected void ShowAndUpdateContainer(Transform selectContainer) 
    {
        ClearActiveButtonContainer(); // ������� ���������� �������� ���������       

        foreach (Transform container in _containerArray) // ��������� ������ �����������
        {
            if (container == selectContainer) // ���� ��� ���������� ��� ���������
            {
                _activeContainer = selectContainer; // �������� ����� �������� ���������
                selectContainer.gameObject.SetActive(true); // ������� ���
                CreateSelectButtonsSystemInActiveContainer();
                _scrollRect.content = (RectTransform)selectContainer; // ��������� ���� ��������� ��� ������� ��� ���������
                _scrollRect.verticalScrollbar.value = 1; // ���������� ��������� ������ � ����.
            }
            else // � ��������� ������
            {
                container.gameObject.SetActive(false); // ��������
            }
        }
    }

    /// <summary>
    /// ������� �������� ��������� c ��������
    /// </summary>
    protected virtual void ClearActiveButtonContainer()
    {
        if (_activeContainer != null)
        {
            foreach (Transform selectPlacedButton in _activeContainer)
            {
                Destroy(selectPlacedButton.gameObject); // ������ ������� ������ ������������� � Transform
            }
            _activeContainer = null;
        }
    }
    /// <summary>
    /// �������� ��� ����������
    /// </summary>
    protected void ClearAllContainer()
    {
        foreach (Transform container in _containerArray)
        {
            foreach (Transform selectPlacedButton in container)
            {
                Destroy(selectPlacedButton.gameObject);
            }
        }
    }

    /// <summary>
    ///  ������� ������� ������ ������(��������) � �������� ����������
    /// </summary>
    protected abstract void CreateSelectButtonsSystemInActiveContainer();
}


