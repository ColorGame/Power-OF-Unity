using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Slider> _sliders;  

    [Header("Parameters")]
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _offset = 3f;


    private void Update()
    {
        for (int i = 0; i < _sliders.Count; i++)
        {
            _sliders[i].value = (Mathf.Sin((Time.time * _speed) + (i * _offset)) * 0.5f) + 0.5f;
        }
    }
}
