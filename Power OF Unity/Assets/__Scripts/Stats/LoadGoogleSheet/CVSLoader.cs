using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Загрузка текста с GOOGLE SHEET
/// </summary>
/// <remarks>
/// Таблица должна иметь открытый доступ!!!
/// </remarks>
public class CVSLoader 
{
    /// <summary>
    /// Загрузим текст с Гугл таблицы. После успешной загрузки запустим ДЕЛЕГАТ
    /// </summary>
    /// <param name="textList"> Список для заполнения текстом, который нада передать делегату </param>
    /// <param name="onSheetLoadedAction"> ДЕЛЕГАТ </param>

    public static IEnumerator DownloadRawCvsTable(string sheetId, string pageId, Action<string> onSheetLoadedAction)
    {
        string actualUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={pageId}";
        using (UnityWebRequest request = UnityWebRequest.Get(actualUrl)) // using- позволяет безопасно работать с фаловым потоком, и предотвратить утечку памяти(не надо каждый раз закрывать поток что бы не перепольнить память)
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ||
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Успешная загрузка");
                // Debug.Log(request.downloadHandler.text);

                onSheetLoadedAction(request.downloadHandler.text);
            }

        }
        yield return null;
    }

  /*  public static IEnumerator DownloadRawCvsTable(string sheetId, Dictionary<TablePage, int> pageTableID, Action<string, TablePage> onSheetLoadedAction)
    {
        foreach (TablePage page in Enum.GetValues(typeof(TablePage)))
        {
            string actualUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={pageTableID[page]}";
            using (UnityWebRequest request = UnityWebRequest.Get(actualUrl)) // using- позволяет безопасно работать с фаловым потоком, и предотвратить утечку памяти(не надо каждый раз закрывать поток что бы не перепольнить память)
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    Debug.Log($"Успешная загрузка {page}");
                    Debug.Log(request.downloadHandler.text);

                    onSheetLoadedAction (request.downloadHandler.text, page);//(STOP) Запустим другую кроутину 
                }
            }
        }
        yield return null;
    }*/

}