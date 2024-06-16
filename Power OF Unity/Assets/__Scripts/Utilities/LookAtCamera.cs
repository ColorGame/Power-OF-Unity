
using UnityEngine;

public class LookAtCamera : MonoBehaviour // Шкала здоровья будет смотреть в сторону камеры
{
    //[SerializeField] private bool _invert; // для 1 МЕТОДА поворота (ставим галочку если надо инвертировать)

    private Transform _cameraTransform;
    private bool _updateTransformStarted;
    private CameraFollow _cameraFollow;

    public void SetupForSpawn(CameraFollow cameraFollow)
    {
        _cameraFollow = cameraFollow;

        _cameraFollow.OnCameraRotateStarted += CameraMove_OnCameraRotateZoomStarted;
        _cameraFollow.OnCameraRotateCompleted += CameraMove_OnCameraRotateZoomCompleted;
        
        _cameraFollow.OnCameraZoomStarted += CameraMove_OnCameraRotateZoomStarted;
        _cameraFollow.OnCameraZoomCompleted += CameraMove_OnCameraRotateZoomCompleted;
    }

    private void Awake()
    {
        _cameraTransform = Camera.main.transform; // Кэшируем трансформ камеры (особой необходимости нет но всеже немного сэкономим ресурсов)
    }

    private void Start()
    {
        UpdateTransform();       
    }

    private void OnDestroy()
    {
        _cameraFollow.OnCameraRotateStarted -= CameraMove_OnCameraRotateZoomStarted;
        _cameraFollow.OnCameraRotateCompleted -= CameraMove_OnCameraRotateZoomCompleted;
        
        _cameraFollow.OnCameraZoomStarted -= CameraMove_OnCameraRotateZoomStarted;
        _cameraFollow.OnCameraZoomCompleted -= CameraMove_OnCameraRotateZoomCompleted;
    }
    private void CameraMove_OnCameraRotateZoomCompleted(object sender, System.EventArgs e) { _updateTransformStarted = false; }
    private void CameraMove_OnCameraRotateZoomStarted(object sender, float e) { _updateTransformStarted = true; }
    private void CameraMove_OnCameraRotateZoomStarted(object sender, System.EventArgs e) { _updateTransformStarted = true; }

    private void LateUpdate() // будем выполнять после всех Update чтобы сначала переместились юниты а потом повернулась UI
    {
        /*// 1 МЕТОД//{ // в данном методе поворот UI игрока зависит от положения на сцене и требует доп проверки
        if (_invert) // Если текст требует инвертирование
        {
            Vector3 directionCamera = (_cameraTransform.position - transform.position).normalized; // Нормализованый вектор направленный в сторону камеры
            transform.LookAt(transform.position + directionCamera*(-1)); // Передадим в аргумент - смотреть относито своей позиции в противоположную сторону вектора directionCamera (он будет как бы смотреть на камеру но жопой)
        }
        else
        {
            transform.LookAt(_cameraTransform);
        }// 1 МЕТОД//}*/

        // 2 ЛУЧШИЙ МЕТОД//{  в данном методе UI будет паралелен рамкам экрана

        if (_updateTransformStarted) // удем обнавлять положение только при движении камеры
            UpdateTransform();
    }

    private void UpdateTransform()
    {
        transform.forward = _cameraTransform.forward; // можно использовать rotetion, но это кватернион и они немного сложнее чем Vector3
    }
}
