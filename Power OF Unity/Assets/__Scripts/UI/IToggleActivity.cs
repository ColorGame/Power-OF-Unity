using System.Net.NetworkInformation;
using UnityEngine;

/// <summary>
/// Переключение активности объекта
/// </summary>
public interface IToggleActivity 
{  
  void SetActive(bool active);
}
