using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ �������
/// </summary>
public abstract class ObjectSelectButtonsSystemUI : MonoBehaviour, IToggleActivity
{
    protected Transform[] _containerButtonArray; // ������ �����������
    protected RectTransform _activeContainer;
    protected Image[] _buttonSelectedImageArray; // ������ ����������� ��� ��������� ������ ������      
    protected ScrollRect _scrollRect; //��������� ��������� ������
    protected Canvas _canvas;
    protected TooltipUI _tooltipUI;
    protected bool _isActive = true;
    protected bool _firstStart = true;


    protected virtual void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);
    }

    protected virtual void Setup()
    {
        // ClearAllContainer();
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
    ///  �������� (� �������� �������� ������ ��������� ������)<br/>
    ///  ��� ������ ������� ���������� ���������� ���� ����������� (��������)
    /// </summary>
    protected void ShowContainer(RectTransform buttonContainer)
    {
        if (_firstStart)
        {
            _firstStart = false;
            UpdateAllContainer(); // ��� ������ ������� ������� ��� ����������
            Show(buttonContainer);
        }
        else
        {
            Show(buttonContainer);
        }
    }

    private void Show(RectTransform buttonContainer)
    {
        foreach (Transform container in _containerButtonArray) // ��������� ������ �����������
        {
            if (container == buttonContainer) // ���� ��� ���������� ��� ���������
            {
                _activeContainer = buttonContainer; // �������� ����� �������� ���������
                buttonContainer.gameObject.SetActive(true); // ������� ���
                _scrollRect.content = buttonContainer; // ��������� ���� ��������� ��� ������� ��� ���������
                _scrollRect.verticalScrollbar.value = 1; // ���������� ��������� ������ � ����.
            }
            else // � ��������� ������
            {
                container.gameObject.SetActive(false); // ��������
            }
        }
    }
    /// <summary>
    /// ������� ��� ����������<br/>
    /// ������ � �������� ������
    /// </summary>
    protected void UpdateAllContainer()
    {
        ClearAllContainer();
        CreateSelectButtonsSystemInAllContainer();
    }

    protected void UpdateContainer(RectTransform buttonContainer)
    {
        ClearButtonContainer(buttonContainer);
        CreateSelectButtonsSystemInContainer(buttonContainer);
    }

    /// <summary>
    ///  ������ ��� ���������� c ��������
    /// </summary>
    protected void HideAllContainerArray()
    {
        foreach (Transform container in _containerButtonArray)
        {
            container.gameObject.SetActive(false);
        }
    }

    /* /// <summary>
     ///  �������� � �������� ��������� (� �������� �������� ������ ��������� ������)
     /// </summary>
     protected void ShowAndUpdateContainer(RectTransform buttonContainer)
     {
         ClearActiveButtonContainer(); // ������� ���������� �������� ���������       

         foreach (Transform container in _containerButtonArray) // ��������� ������ �����������
         {
             if (container == buttonContainer) // ���� ��� ���������� ��� ���������
             {
                 _activeContainer = buttonContainer; // �������� ����� �������� ���������
                 buttonContainer.gameObject.SetActive(true); // ������� ���
                 CreateSelectButtonsSystemInActiveContainer();
                 _scrollRect.content = buttonContainer; // ��������� ���� ��������� ��� ������� ��� ���������
                 _scrollRect.verticalScrollbar.value = 1; // ���������� ��������� ������ � ����.
             }
             else // � ��������� ������
             {
                 container.gameObject.SetActive(false); // ��������
             }
         }
     }*/

    /*    /// <summary>
        /// ������� �������� ��������� c ��������
        /// </summary>
        protected virtual void ClearActiveButtonContainer()
        {
            if (_activeContainer != null)
            {
               // ClearButtonContainer(_activeContainer);
                _activeContainer = null;
            }
        }*/
    /// <summary>
    /// �������� ���������� ��������� � ��������
    /// </summary>
    private void ClearButtonContainer(RectTransform buttonContainer)
    {
        foreach (Transform buttonTransform in buttonContainer)
        {
            Destroy(buttonTransform.gameObject); // ������ ������� ������ ������������� � Transform
        }
    }
    /// <summary>
    /// �������� ��� ����������
    /// </summary>
    private  void ClearAllContainer()
    {
        foreach (Transform container in _containerButtonArray)
        {
            foreach (Transform selectPlacedButton in container)
            {
                Destroy(selectPlacedButton.gameObject);
            }
        }
    }

    /*/// <summary>
    ///  ������� ������� ������ ������(��������) � �������� ����������
    /// </summary>
    protected abstract void CreateSelectButtonsSystemInActiveContainer();*/

    /// <summary>
    ///  ������� ������� ������ ������(��������) � ���������� ���������� ����������
    /// </summary>
    protected abstract void CreateSelectButtonsSystemInContainer(RectTransform buttonContainer);
    /// <summary>
    ///  ������� ������� ������ ������(��������) �� ���� �����������
    /// </summary>
    protected abstract void CreateSelectButtonsSystemInAllContainer();
}


