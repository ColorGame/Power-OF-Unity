using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// �������� ������ � GOOGLE SHEET
/// </summary>
/// <remarks>
/// ������� ������ ����� �������� ������!!!
/// </remarks>
public class CVSLoader 
{
    /// <summary>
    /// �������� ����� � ���� �������. ����� �������� �������� �������� �������
    /// </summary>
    /// <param name="textList"> ������ ��� ���������� �������, ������� ���� �������� �������� </param>
    /// <param name="onSheetLoadedAction"> ������� </param>

    public static IEnumerator DownloadRawCvsTable(string sheetId, string pageId, Action<string> onSheetLoadedAction)
    {
        string actualUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={pageId}";
        using (UnityWebRequest request = UnityWebRequest.Get(actualUrl)) // using- ��������� ��������� �������� � ������� �������, � ������������� ������ ������(�� ���� ������ ��� ��������� ����� ��� �� �� ������������ ������)
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ||
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("�������� ��������");
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
            using (UnityWebRequest request = UnityWebRequest.Get(actualUrl)) // using- ��������� ��������� �������� � ������� �������, � ������������� ������ ������(�� ���� ������ ��� ��������� ����� ��� �� �� ������������ ������)
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    Debug.Log($"�������� �������� {page}");
                    Debug.Log(request.downloadHandler.text);

                    onSheetLoadedAction (request.downloadHandler.text, page);//(STOP) �������� ������ �������� 
                }
            }
        }
        yield return null;
    }*/

}