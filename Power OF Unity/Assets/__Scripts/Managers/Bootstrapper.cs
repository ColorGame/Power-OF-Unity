using UnityEngine;

public static class Bootstrapper // Загрузчик
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // Используйте этот атрибут для получения обратного вызова при запуске среды выполнения и загрузке первой сцены.
                                                                               // Используйте различные параметры для RuntimeInitializeLoadType, чтобы контролировать, когда метод вызывается в последовательности запуска.
                                                                               // BeforeSceneLoad. Здесь загружаются объекты сцены, но функция Awake() еще не была вызвана. Здесь все объекты считаются неактивными.
   
    
    public static void Execute()
    {
        Object bootstrapEntryPoint = Object.Instantiate(Resources.Load("CoreEntryPoint"));// Создать префаб CoreEntryPoint
      //  Object.DontDestroyOnLoad(bootstrapEntryPoint); // и не уничтожать при загрузке другой сцены
    }

    /*public static void Execute()
    {
        Object.DontDestroyOnLoad(Addressables.InstatiateAsync("CoreEntryPoint").WaitForCompletion()); // Создать префаб CoreEntryPoint и не уничтожать при загрузке другой сцены
    }*/
}