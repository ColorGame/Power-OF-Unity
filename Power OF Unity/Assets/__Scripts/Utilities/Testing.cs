using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Transform _gridDebugObjectPrefab; // ������ ������� ����� //������������ ��� ������ ��������� � ����� ��������� ������ CreateDebugObject
    [SerializeField] private Unit _unit;
    private GridSystemXZ<GridObjectUnitXZ> _gridSystem;

    private void Start()
    {


        //���� CreateDebugObject ������������ ��������� ����� � ������ ������
        /*_gridSystemXYList = new GridSystemXZ(10, 10, 2f); // �������� ����� 10 �� 10 � �������� 2 �������
        _gridSystemXYList.CreateDebugObject(_gridDebugObjectPrefab); // �������� ��� ������ � ������ ������

        Debug.Log(new GridPositionXZ(5, 7)); // ��������� ��� ���������� GridPositionXZ*/
    }

    private void Update()
    {
        //���� 
        //Debug.Log(_gridSystemXYList.GetGridPosition(MouseOnGameGrid.GetTransformPosition())); // ������� ��������� ����� ����� ��� ������

        //���� ���������� �������� ������� ��� ��������




        
        if(Input.GetKeyDown(KeyCode.T))
        {
            //��� ��������� ����� ���������� �� (0,0) � ����� ��������� ����
            /*GridPositionXZ _mouseGridPosition = _levelGrid.GetGridPosition(MouseOnGameGrid.GetTransformPosition());

            GridPositionXZ startGridPosition = new GridPositionXZ(0, 0);

            List<GridPositionXZ> gridPositionList = PathfindingMonkey.Instance.FindPath(startGridPosition, _mouseGridPosition);

            for (int i = 0; i < gridPositionList.Count -1 ; i++)
            {
                Debug.DrawLine(
                    _levelGrid.GetWorldPositionCenter�ornerCell(gridPositionList[i]),
                    _levelGrid.GetWorldPositionCenter�ornerCell(gridPositionList[i + 1]),
                    Color.white,
                    10f
                    );
            }*/

            
        }      
    }
}

