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

    public JsonSavingSystem() { } // Конструктор что бы отследить количество созданных new T() ОН ДОЛЖЕН БЫТЬ ОДИН

    /// <summary>
    /// Загрузит последнюю сохраненную сцену и восстановит состояние.
    /// Это должно выполняться как сопрограмма.
    /// </summary>
    /// <param name="saveFile">Сохраненный файл, к которому нужно обратиться для загрузки.</param>
    public IEnumerator LoadLastScene(string saveFile)
    {
        JObject state = LoadJsonFromFile(saveFile);
        IDictionary<string, JToken> stateDict = state;
        int buildIndex = SceneManager.GetActiveScene().buildIndex; // Полуим индекс активной сцены по умолчанию
        if (stateDict.ContainsKey("lastSceneBuildIndex")) // Если в сохраненом файле есть ключ - "lastSceneBuildIndex"
        {
            buildIndex = (int)stateDict["lastSceneBuildIndex"]; //Изменим Индекс сцены  на индекс последней сохраненой сцены
        }
        yield return SceneManager.LoadSceneAsync(buildIndex); // Загрузим эту сцену ассинхроно (это происходит между Awake и Start)
        RestoreFromToken(state); // Восстановим сохраненное состояние этой сцены
    }

    /// <summary>
    /// Сохраните текущую сцену в предоставленный файл сохранения.
    /// </summary>
    public void Save(string saveFile)
    {
        JObject state = LoadJsonFromFile(saveFile);
        CaptureAsToken(state);
        SaveFileAsJSon(saveFile, state);
    }

    /// <summary>
    /// Удалите состояние в данном файле сохранения.
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
        if (!File.Exists(path)) // Определяет, существует ли указанный файл.
        {
            // Если нет то создадим новый пустой Json фаил для сохранения
            return new JObject();
        }

        using (var textReader = File.OpenText(path)) //using- позволяет безопасно работать с фаловым потоком, и предотвратить утечку памяти(не надо каждый раз закрывать поток что бы не перепольнить память)
        {
            using (var reader = new JsonTextReader(textReader))
            {
                reader.FloatParseHandling = FloatParseHandling.Double; // Обработка синтаксического анализа с плавающей точкой - .Двойной

                return JObject.Load(reader);
            }
        }

    }

    private void SaveFileAsJSon(string saveFile, JObject state)
    {
        string path = GetPathFromSaveFile(saveFile); // Получим путь сохраненого файла
        Debug.Log("Saving to " + path);
        using (var textWriter = File.CreateText(path)) //перезапишем полученый файл в нужный формат // using- позволяет безопасно работать с фаловым потоком, и предотвратить утечку памяти(не надо каждый раз закрывать поток что бы не перепольнить память)
        {
            using (var writer = new JsonTextWriter(textWriter)) // Создадим JsonTextWriter из преданного текста
            {
                writer.Formatting = Formatting.Indented; //задодим нужный формат. Возвращает или задает значение, указывающее, как должен быть отформатирован вывод текста в формате JSON.
                state.WriteTo(writer); // Запишем в токен наш созданный JsonTextWriter
            }
        }
    }

    /// <summary>
    ///  Захватим объект как Token
    /// </summary>    
    private void CaptureAsToken(JObject state) 
    {
        IDictionary<string, JToken> stateDict = state;  //сопоставляет  Dictionary<string, JToken> прямо с JObject.
                                                        // Найдем и переберем все сущности которые должны сохраняться
        foreach (JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())
        {
            // Сохраним в словаре (КЛЮЧ- Индефикатор, ЗНАЧЕНИЕ - Jtoken сохраняемого объекта)
            stateDict[saveable.GetUniqueIdentifier()] = saveable.CaptureAsJtoken();
        }
        // Для  ключа lastSceneBuildIndex сохраним индекс последней активной СОХРАНЕНОЙ сцены
        stateDict["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
    }

    private IEnumerable<T> FindObjectsOfType<T>()
    {
        throw new NotImplementedException();
    }

    private void RestoreFromToken(JObject state) // Восстановление из токена (для избежания гонки помни что этот метод вызывается мжду Awakw и Start)
    {
        IDictionary<string, JToken> stateDict = state;  //сопоставляет  Dictionary<string, JToken> прямо с JObject.
                                                        // Найдем и переберем все сущности которые должны сохраняться
        foreach (JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())
        {
            // Получим ID объекта
            string id = saveable.GetUniqueIdentifier();
            // Если этот ID есть в переданном нам словаре
            if (stateDict.ContainsKey(id))
            {
                // Восстановим наш объект по этому номеру
                saveable.RestoreFromJToken(stateDict[id]);
            }
        }
    }


    private string GetPathFromSaveFile(string saveFile)
    {
        return Path.Combine(Application.persistentDataPath, saveFile + EXTENSION);
    }
}