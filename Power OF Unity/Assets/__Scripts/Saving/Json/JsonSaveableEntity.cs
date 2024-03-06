using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteAlways] // Позволяет всегда выполнять экземпляры скрипта, как в режиме воспроизведения, так и при редактировании.
public class JsonSaveableEntity : MonoBehaviour // Сохраняемая сущность. Повесить на объект который будем сохронять
{

    [SerializeField] string uniqueIdentifier = ""; // Индефикатор

    // КЭШИРОВАННОЕ СОСТОЯНИЕ
    static Dictionary<string, JsonSaveableEntity> globalLookup = new Dictionary<string, JsonSaveableEntity>(); // Глобальный поиск (ключ -Индификатор, значение - сохраняемый объект)// Нужен для отслеживания повторяющихся ID. Вкаждом кадре будем проверять ключи словоря т.к. это дешево

    public string GetUniqueIdentifier()
    {
        return uniqueIdentifier;
    }

    public JToken CaptureAsJtoken() // Захватим объект как Jtoken
    {
        JObject state = new JObject(); // JObject - это специальный класс-контейнер, который позволяет нам хранить коллекцию записей ключа / значения. Ключи представляют собой строки. Значение - это JTokens.
        IDictionary<string, JToken> stateDict = state; //сопоставляет  Dictionary<string, JToken> прямо с JObject.  Оба state и stateDict  указывают на один и тот же объект.
        foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>()) // Получим и переберем все Реализованные интерфейсы IJsonSaveable
        {
            // Получим токен
            JToken token = jsonSaveable.CaptureAsJToken();
            // Получим ТИП токена
            string component = jsonSaveable.GetType().ToString();
            Debug.Log($"{name} Capture {component} = {token.ToString()}");
            // Сохраним в словаре для этого типа наше значение
            stateDict[jsonSaveable.GetType().ToString()] = token;
        }
        return state;
    }

    public void RestoreFromJToken(JToken s) // Восстановить из JToken
    {
        // Получим Новый объект, созданный на основе значения JSON.
        JObject state = s.ToObject<JObject>();
        //сопоставляет  Dictionary<string, JToken> прямо с JObject.
        IDictionary<string, JToken> stateDict = state;
        // Получим и переберем все Реализованные интерфейсы IJsonSaveable
        foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>())
        {
            // Получим ТИП токена
            string component = jsonSaveable.GetType().ToString();
            // Если такой ключ есть в словаре то
            if (stateDict.ContainsKey(component))
            {
                Debug.Log($"{name} Restore {component} =>{stateDict[component].ToString()}");
                jsonSaveable.RestoreFromJToken(stateDict[component]);
            }
        }
    }

#if UNITY_EDITOR // Когда буду создавадь Build игры этот код игнорируется т.к. у игры нет доступа к пространству имен SerializedObject
    private void Update() // В режиме редактирования в Update приcвоим индификационные номера только тем объектам которые в сцене (префабам присваивать не будем)
    {
        // остановить если мы в режиме воспроизведения
        if (Application.IsPlaying(gameObject)) return; //Application.IsPlaying - Возвращает true при вызове в любом встроенном проигрывателе или при вызове в редакторе в режиме воспроизведения (только для чтения).
                                                       // остановить если наш объект НЕ в сцене (например в префабе)
        if (string.IsNullOrEmpty(gameObject.scene.path)) return; //string.IsNullOrEmpty -Указывает, действительно ли указанная строка является строкой null или пустой строкой ("").
                                                                 // Сделаем этот объект сериализованным полем
        SerializedObject serializedObject = new SerializedObject(this);
        // Найдем сериализованное поле с именем индефикатор
        SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier"); // SerializedProperty(сериализованная собственность) в основном используется для чтения или изменения значения свойства. 

        // Если индефикатор пуст или ноль  ||  Он не уникальный (повторяется). //При копирование объектов в сцене, не из префаба,  их ID повторяется поэтому и проверяем их уникальность
        if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
        {
            property.stringValue = System.Guid.NewGuid().ToString(); // Сгенерируем индификационный номер и переведем в строку
            serializedObject.ApplyModifiedProperties(); // Применим изменения свойств к нашему объекту
        }

        globalLookup[property.stringValue] = this; // Сохраним в Словарь нашу сущность, для дальнейшего поиска на уникальность ID
    }
#endif

    private bool IsUnique(string candidate)
    {
        if (!globalLookup.ContainsKey(candidate)) return true; // Если ключа нет в словаре значит он УНИКАЛЕН

        if (globalLookup[candidate] == this) return true; // Вернуть истину если кандитат который проверяем на уникальность - это и есть эта сущность

        if (globalLookup[candidate] == null) // Если под этим ID нет сущности (он удален)
        {
            globalLookup.Remove(candidate); // Удалим ключ и вернем истину
            return true;
        }

        if (globalLookup[candidate].GetUniqueIdentifier() != candidate) // Если вдруг ID этой сущности в словаре отличается от текущего ID то
        {
            globalLookup.Remove(candidate); // Удалим ключ и вернем истину
            return true;
        }

        return false;
    }
}