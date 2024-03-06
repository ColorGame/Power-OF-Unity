using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteAlways] // ��������� ������ ��������� ���������� �������, ��� � ������ ���������������, ��� � ��� ��������������.
public class JsonSaveableEntity : MonoBehaviour // ����������� ��������. �������� �� ������ ������� ����� ���������
{

    [SerializeField] string uniqueIdentifier = ""; // �����������

    // ������������ ���������
    static Dictionary<string, JsonSaveableEntity> globalLookup = new Dictionary<string, JsonSaveableEntity>(); // ���������� ����� (���� -�����������, �������� - ����������� ������)// ����� ��� ������������ ������������� ID. ������� ����� ����� ��������� ����� ������� �.�. ��� ������

    public string GetUniqueIdentifier()
    {
        return uniqueIdentifier;
    }

    public JToken CaptureAsJtoken() // �������� ������ ��� Jtoken
    {
        JObject state = new JObject(); // JObject - ��� ����������� �����-���������, ������� ��������� ��� ������� ��������� ������� ����� / ��������. ����� ������������ ����� ������. �������� - ��� JTokens.
        IDictionary<string, JToken> stateDict = state; //������������  Dictionary<string, JToken> ����� � JObject.  ��� state � stateDict  ��������� �� ���� � ��� �� ������.
        foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>()) // ������� � ��������� ��� ������������� ���������� IJsonSaveable
        {
            // ������� �����
            JToken token = jsonSaveable.CaptureAsJToken();
            // ������� ��� ������
            string component = jsonSaveable.GetType().ToString();
            Debug.Log($"{name} Capture {component} = {token.ToString()}");
            // �������� � ������� ��� ����� ���� ���� ��������
            stateDict[jsonSaveable.GetType().ToString()] = token;
        }
        return state;
    }

    public void RestoreFromJToken(JToken s) // ������������ �� JToken
    {
        // ������� ����� ������, ��������� �� ������ �������� JSON.
        JObject state = s.ToObject<JObject>();
        //������������  Dictionary<string, JToken> ����� � JObject.
        IDictionary<string, JToken> stateDict = state;
        // ������� � ��������� ��� ������������� ���������� IJsonSaveable
        foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>())
        {
            // ������� ��� ������
            string component = jsonSaveable.GetType().ToString();
            // ���� ����� ���� ���� � ������� ��
            if (stateDict.ContainsKey(component))
            {
                Debug.Log($"{name} Restore {component} =>{stateDict[component].ToString()}");
                jsonSaveable.RestoreFromJToken(stateDict[component]);
            }
        }
    }

#if UNITY_EDITOR // ����� ���� ��������� Build ���� ���� ��� ������������ �.�. � ���� ��� ������� � ������������ ���� SerializedObject
    private void Update() // � ������ �������������� � Update ���c���� ��������������� ������ ������ ��� �������� ������� � ����� (�������� ����������� �� �����)
    {
        // ���������� ���� �� � ������ ���������������
        if (Application.IsPlaying(gameObject)) return; //Application.IsPlaying - ���������� true ��� ������ � ����� ���������� ������������� ��� ��� ������ � ��������� � ������ ��������������� (������ ��� ������).
                                                       // ���������� ���� ��� ������ �� � ����� (�������� � �������)
        if (string.IsNullOrEmpty(gameObject.scene.path)) return; //string.IsNullOrEmpty -���������, ������������� �� ��������� ������ �������� ������� null ��� ������ ������� ("").
                                                                 // ������� ���� ������ ��������������� �����
        SerializedObject serializedObject = new SerializedObject(this);
        // ������ ��������������� ���� � ������ �����������
        SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier"); // SerializedProperty(��������������� �������������) � �������� ������������ ��� ������ ��� ��������� �������� ��������. 

        // ���� ����������� ���� ��� ����  ||  �� �� ���������� (�����������). //��� ����������� �������� � �����, �� �� �������,  �� ID ����������� ������� � ��������� �� ������������
        if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
        {
            property.stringValue = System.Guid.NewGuid().ToString(); // ����������� ��������������� ����� � ��������� � ������
            serializedObject.ApplyModifiedProperties(); // �������� ��������� ������� � ������ �������
        }

        globalLookup[property.stringValue] = this; // �������� � ������� ���� ��������, ��� ����������� ������ �� ������������ ID
    }
#endif

    private bool IsUnique(string candidate)
    {
        if (!globalLookup.ContainsKey(candidate)) return true; // ���� ����� ��� � ������� ������ �� ��������

        if (globalLookup[candidate] == this) return true; // ������� ������ ���� �������� ������� ��������� �� ������������ - ��� � ���� ��� ��������

        if (globalLookup[candidate] == null) // ���� ��� ���� ID ��� �������� (�� ������)
        {
            globalLookup.Remove(candidate); // ������ ���� � ������ ������
            return true;
        }

        if (globalLookup[candidate].GetUniqueIdentifier() != candidate) // ���� ����� ID ���� �������� � ������� ���������� �� �������� ID ��
        {
            globalLookup.Remove(candidate); // ������ ���� � ������ ������
            return true;
        }

        return false;
    }
}