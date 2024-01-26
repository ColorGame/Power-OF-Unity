/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.IO;
using UnityEngine;

public static class SaveSystem // Простой класс без наследования монобех.
{
    private const string SAVE_EXTENSION = "txt"; // Расширение для сохранения

    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/"; // Папка сохранения - Выполняется во время ввполнения
    private static bool isInit = false; // Объект Инициализирован - по умолчанию нет

    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            // Проверьте, существует ли папка сохранения
            if (!Directory.Exists(SAVE_FOLDER))
            {
                // Создать папку сохранения
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }
    }

    public static void Save(string fileName, string saveString, bool overwrite) // overwrite - переписать фаил сохранения или нет
    {
        Init();
        string saveFileName = fileName;
        if (!overwrite)
        {
            // Убедитесь, что сохраненный номер уникален, чтобы он не перезаписывал предыдущий сохраненный файл
            int saveNumber = 1;
            while (File.Exists(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION)) //File.Exists - Определяет, существует ли указанный файл. Если существует то..
            {
                saveNumber++;
                saveFileName = fileName + "_" + saveNumber;
            }
            // сохраненное имя файла уникально
        }
        File.WriteAllText(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION, saveString);
    }

    public static string Load(string fileName)
    {
        Init();
        if (File.Exists(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION);
            return saveString;
        }
        else
        {
            return null;
        }
    }

    public static string LoadMostRecentFile() // Загрузить самый последний файл
    {
        Init();
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        // Получить все сохраненные файлы
        FileInfo[] saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
        // Циклически просматривайте все сохраненные файлы и определяйте самый последний из них
        FileInfo mostRecentFile = null; // самый последний файл
        foreach (FileInfo fileInfo in saveFiles)
        {
            if (mostRecentFile == null)
            {
                mostRecentFile = fileInfo;
            }
            else
            {
                if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime) // Сравним время последней записи
                {
                    mostRecentFile = fileInfo;
                }
            }
        }

        // Если есть файл сохранения, загрузите его, если нет, верните null
        if (mostRecentFile != null)
        {
            string saveString = File.ReadAllText(mostRecentFile.FullName);
            return saveString;
        }
        else
        {
            return null;
        }
    }

    public static void SaveObject(object saveObject)
    {
        SaveObject("save", saveObject, false);
    }

    public static void SaveObject(string fileName, object saveObject, bool overwrite)
    {
        Init();
        string json = JsonUtility.ToJson(saveObject);
        Save(fileName, json, overwrite);
    }

    public static TSaveObject LoadMostRecentObject<TSaveObject>() // Загрузить самый последний объект <Будем использовать джинерики> что бы загружать разные сущности
    {
        Init();
        string saveString = LoadMostRecentFile();
        if (saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }

    public static TSaveObject LoadObject<TSaveObject>(string fileName)
    {
        Init();
        string saveString = Load(fileName);
        if (saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }

}

