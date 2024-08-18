using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

//https://www.youtube.com/watch?v=E3rCrz0q02U&t=50s

public class SheetProcessor  // ������������ �������� �����
{
    /// <summary>
    /// ����������� ����� � �������� ������.
    /// </summary>
    private const string CELL_SEPORATOR = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"; // ������������� ������� ((?= ... )) �����������, ��� ����� ������� ���� ������ ���������� ������� ��� ���������� (�. �. ���� ��� ����������� ������, ���� �� ��� ������). [^\"]* ������������� ��������, �� ����������� � �������.

    /// <summary>
    /// ����������� �������� ������ ����� ������
    /// </summary>
    private const char IN_CELL_SEPORATOR = ';';

    private Dictionary<string, Color> _colors = new Dictionary<string, Color>()
    {
        {"white", Color.white},
        {"black", Color.black},
        {"yellow", Color.yellow},
        {"red", Color.red},
        {"green", Color.green},
        {"blue", Color.blue},
    };


    /// <summary>
    /// ����������� ����� ����� � ��������� ������ �����<br/>
    /// a1,b1,c1,d1<br/>
    /// a2,b2,c2,d2<br/>
    /// a3,b3,c3,d3 
    /// </summary>
    /// <returns>������ ������ � ��������� - [����� ������, ����� �������]</returns>
    public static string[,] ProcessData(string csvRawData)
    {
        SheetProcessor sheetProcessor = new SheetProcessor(); // �.�. ��� ����������� ����� �� ������� �������� �������� ������� ������ � ����� ������� � ���� ������ �����
        char lineEnding = sheetProcessor.GetPlatformSpecificLineEnd(); // �������� ����� ������ ��� ���������� ���������

        string[] rows = csvRawData.Split(lineEnding); // �������� �� ������ ����� 
        string[] cells = Regex.Split( rows[0],CELL_SEPORATOR); // �������� ������ ������[0] �� ������ ����� � ������� CELL_SEPORATOR � ������� ������ ����� �� �������� ������ ����� ��������

        string[,] gridDateArray = new string[rows.Length, cells.Length]; // �������������� ��������� ������ ������������� �������   

        for (int rowNamber = 0; rowNamber < rows.Length; rowNamber++) // ��������� ������
        {
            cells = Regex.Split(rows[rowNamber],CELL_SEPORATOR); // �������� ������ �� ������ ����� � ������� CELL_SEPORATOR 

            for (int columnNumber = 0; columnNumber < cells.Length; columnNumber++) // �������� ������ � ������ ������
            {
                gridDateArray[rowNamber, columnNumber] = cells[columnNumber]; // �������� ������ ������ � ��������� ������� ��� [rowNamber,columnNumber] ��� ����� ������� �������.
            }
        }
        return gridDateArray;
    }

   

    private Color ParseColor(string color)
    {
        color = color.Trim();
        Color result = default;
        if (_colors.ContainsKey(color))
        {
            result = _colors[color];
        }

        return result;
    }

    private Vector3 ParseVector3(string s)
    {
        string[] vectorComponents = s.Split(IN_CELL_SEPORATOR);
        if (vectorComponents.Length < 3)
        {
            Debug.Log("Can't parse Vector3. Wrong text format");
            return default;
        }

        float x = ParseFloat(vectorComponents[0]);
        float y = ParseFloat(vectorComponents[1]);
        float z = ParseFloat(vectorComponents[2]);
        return new Vector3(x, y, z);
    }

    public static int ParseInt(string s)
    {
        int result = -1;
        if (!int.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out result))
        {
            Debug.Log("�� ������� ��������� int, ������������ �����");
        }

        return result;
    }

    public static T ParseEnum<T>(string value)
    {
        object result;
        if (!Enum.TryParse(typeof(T), value, true, out result))
        {
            Debug.Log($"�� ������� ������������� {value} � Enum");
            result = PlacedObjectType.None;
        }

        return (T)result;
    }

    private float ParseFloat(string s)
    {
        float result = -1;
        if (!float.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out result))
        {
            Debug.Log("Can't pars float,wrong text ");
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    public static IEnumerable<string> SplitIgnoringQuotes(string input, char delimiter)
    {
        bool insideQuotes = false;
        StringBuilder currentToken = new StringBuilder();

        foreach (char c in input)
        {
            if (c == delimiter && !insideQuotes)
            {
                yield return currentToken.ToString();
                currentToken.Clear();
            }
            else
            {
                currentToken.Append(c);

                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
            }
            yield return currentToken.ToString();
        }
    }

    private char GetPlatformSpecificLineEnd() // ������������ ������������
    {
        char lineEnding = '\n'; // ��� Windows & Android
#if UNITY_IOS
        lineEnding = '\r'; // ��� IOS
#endif
        return lineEnding;
    }
}