using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// https://gitlab.com/Mnemoth42/RPG/-/wikis/home


public class JsonSavingSystem //: MonoBehaviour
{
    private const string EXTENSION = ".json";

    public JsonSavingSystem() { } // ����������� ��� �� ��������� ���������� ��������� new T() �� ������ ���� ����

    /// <summary>
    /// �������� ��������� ����������� ����� � ����������� ���������.
    /// ��� ������ ����������� ��� �����������.
    /// </summary>
    /// <param name="saveFile">����������� ����, � �������� ����� ���������� ��� ��������.</param>
    public IEnumerator LoadLastScene(string saveFile)
    {
        JObject state = LoadJsonFromFile(saveFile);
        IDictionary<string, JToken> stateDict = state;
        int buildIndex = SceneManager.GetActiveScene().buildIndex; // ������ ������ �������� ����� �� ���������
        if (stateDict.ContainsKey("lastSceneBuildIndex")) // ���� � ���������� ����� ���� ���� - "lastSceneBuildIndex"
        {
            buildIndex = (int)stateDict["lastSceneBuildIndex"]; //������� ������ �����  �� ������ ��������� ���������� �����
        }
        yield return SceneManager.LoadSceneAsync(buildIndex); // �������� ��� ����� ���������� (��� ���������� ����� Awake � Start)
        RestoreFromToken(state); // ����������� ����������� ��������� ���� �����
    }

    /// <summary>
    /// ��������� ������� ����� � ��������������� ���� ����������.
    /// </summary>
    public void Save(string saveFile)
    {
        JObject state = LoadJsonFromFile(saveFile);
        CaptureAsToken(state);
        SaveFileAsJSon(saveFile, state);
    }

    /// <summary>
    /// ������� ��������� � ������ ����� ����������.
    /// </summary>
    public void Delete(string saveFile)
    {
        File.Delete(GetPathFromSaveFile(saveFile));
    }

    public void Load(string saveFile)
    {
        RestoreFromToken(LoadJsonFromFile(saveFile));
    }

    public IEnumerable<string> ListSaves()
    {
        foreach (string path in Directory.EnumerateFiles(Application.persistentDataPath))
        {
            if (Path.GetExtension(path) == EXTENSION)
            {
                yield return Path.GetFileNameWithoutExtension(path);
            }
        }
    }

    // PRIVATE

    private JObject LoadJsonFromFile(string saveFile)
    {
        string path = GetPathFromSaveFile(saveFile);
        if (!File.Exists(path)) // ����������, ���������� �� ��������� ����.
        {
            // ���� ��� �� �������� ����� ������ Json ���� ��� ����������
            return new JObject();
        }

        using (var textReader = File.OpenText(path)) //using- ��������� ��������� �������� � ������� �������, � ������������� ������ ������(�� ���� ������ ��� ��������� ����� ��� �� �� ������������ ������)
        {
            using (var reader = new JsonTextReader(textReader))
            {
                reader.FloatParseHandling = FloatParseHandling.Double; // ��������� ��������������� ������� � ��������� ������ - .�������

                return JObject.Load(reader);
            }
        }

    }

    private void SaveFileAsJSon(string saveFile, JObject state)
    {
        string path = GetPathFromSaveFile(saveFile); // ������� ���� ����������� �����
        Debug.Log("Saving to " + path);
        using (var textWriter = File.CreateText(path)) //����������� ��������� ���� � ������ ������ // using- ��������� ��������� �������� � ������� �������, � ������������� ������ ������(�� ���� ������ ��� ��������� ����� ��� �� �� ������������ ������)
        {
            using (var writer = new JsonTextWriter(textWriter)) // �������� JsonTextWriter �� ���������� ������
            {
                writer.Formatting = Formatting.Indented; //������� ������ ������. ���������� ��� ������ ��������, �����������, ��� ������ ���� �������������� ����� ������ � ������� JSON.
                state.WriteTo(writer); // ������� � ����� ��� ��������� JsonTextWriter
            }
        }
    }

    /// <summary>
    ///  �������� ������ ��� Token
    /// </summary>    
    private void CaptureAsToken(JObject state) 
    {
        IDictionary<string, JToken> stateDict = state;  //������������  Dictionary<string, JToken> ����� � JObject.
                                                        // ������ � ��������� ��� �������� ������� ������ �����������
        foreach (JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())
        {
            // �������� � ������� (����- �����������, �������� - Jtoken ������������ �������)
            stateDict[saveable.GetUniqueIdentifier()] = saveable.CaptureAsJtoken();
        }
        // ���  ����� lastSceneBuildIndex �������� ������ ��������� �������� ���������� �����
        stateDict["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
    }

    private IEnumerable<T> FindObjectsOfType<T>()
    {
        throw new NotImplementedException();
    }

    private void RestoreFromToken(JObject state) // �������������� �� ������ (��� ��������� ����� ����� ��� ���� ����� ���������� ���� Awakw � Start)
    {
        IDictionary<string, JToken> stateDict = state;  //������������  Dictionary<string, JToken> ����� � JObject.
                                                        // ������ � ��������� ��� �������� ������� ������ �����������
        foreach (JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())
        {
            // ������� ID �������
            string id = saveable.GetUniqueIdentifier();
            // ���� ���� ID ���� � ���������� ��� �������
            if (stateDict.ContainsKey(id))
            {
                // ����������� ��� ������ �� ����� ������
                saveable.RestoreFromJToken(stateDict[id]);
            }
        }
    }


    private string GetPathFromSaveFile(string saveFile)
    {
        return Path.Combine(Application.persistentDataPath, saveFile + EXTENSION);
    }
}