using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

//https://www.youtube.com/watch?v=E3rCrz0q02U&t=50s

public class SheetProcessor  // Обрабатывает исходный текст
{
    /// <summary>
    /// Разделитель ячеек в исходном тексте.
    /// </summary>
    private const string CELL_SEPORATOR = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"; // Положительный прогноз ((?= ... )) гарантирует, что перед запятой есть четное количество кавычек для разделения (т. е. Либо они встречаются парами, либо их нет вообще). [^\"]* соответствует символам, не заключенным в кавычки.

    /// <summary>
    /// Разделитель значений внутри одной ячейки
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
    /// Преобразуем сырой текст в двумерный массив строк<br/>
    /// a1,b1,c1,d1<br/>
    /// a2,b2,c2,d2<br/>
    /// a3,b3,c3,d3 
    /// </summary>
    /// <returns>Вернем массив с индексами - [номер строки, номер столбца]</returns>
    public static string[,] ProcessData(string csvRawData)
    {
        SheetProcessor sheetProcessor = new SheetProcessor(); // Т.к. это статический метод то сначала создадим экземляр данного класса а потом вызывим у него нужный метод
        char lineEnding = sheetProcessor.GetPlatformSpecificLineEnd(); // Получить конец строки для конкретной платформы

        string[] rows = csvRawData.Split(lineEnding); // Разделим на массив строк 
        string[] cells = Regex.Split( rows[0],CELL_SEPORATOR); // Разделим первую строку[0] на массив ячеек с помощью CELL_SEPORATOR и получим массив ячеек из которого узнаем число столбцов

        string[,] gridDateArray = new string[rows.Length, cells.Length]; // Инициализируем двумерный массив определенного размера   

        for (int rowNamber = 0; rowNamber < rows.Length; rowNamber++) // Переберем строки
        {
            cells = Regex.Split(rows[rowNamber],CELL_SEPORATOR); // Разделим строку на массив ячеек с помощью CELL_SEPORATOR 

            for (int columnNumber = 0; columnNumber < cells.Length; columnNumber++) // Перберем ячейки в данной строке
            {
                gridDateArray[rowNamber, columnNumber] = cells[columnNumber]; // Сохраним данные ячейки в двумерном массиве где [rowNamber,columnNumber] это будут индексы массива.
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
            Debug.Log("Не удается разобрать int, неправильный текст");
        }

        return result;
    }

    public static T ParseEnum<T>(string value)
    {
        object result;
        if (!Enum.TryParse(typeof(T), value, true, out result))
        {
            Debug.Log($"Не удается преобразовать {value} в Enum");
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

    private char GetPlatformSpecificLineEnd() // Платформеная спецификация
    {
        char lineEnding = '\n'; // Для Windows & Android
#if UNITY_IOS
        lineEnding = '\r'; // Для IOS
#endif
        return lineEnding;
    }
}