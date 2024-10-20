using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JumpAnim : MonoBehaviour
{
    public AnimationCurve AnimCurve;
    

    bool jump;
    float currentTime = 0;
    float maxTime = 2; // ����� ���������� � ������
    float maxHeight = 5;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
            currentTime = 0;
        }
        if (jump) // ��� ����������� �����������
        {
            currentTime += Time.deltaTime;
            if (currentTime < maxTime)
            {
                float mormTime = currentTime / maxTime;
                float positionY = AnimCurve.Evaluate(mormTime)* maxHeight;

                transform.position += new Vector3(transform.position.x, positionY, transform.position.y); //���������� ������� � ������ ������������ ������
            }
            else
            {
                jump = false;
               
            }
        }
    }
}
