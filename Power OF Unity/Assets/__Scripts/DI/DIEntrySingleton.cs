using System;

/// <summary>
/// Фабрика которая производит singleton (не статический, конечно, просто экземпляр с флагом) и кэширует его.
/// </summary>
/// <remarks>
/// Наследует DIEntry<T>.
/// Фабрика не создает экземпляры мгновенно, а только, когда вы запрашиваете классы, вызывая Resolve().
/// </remarks>
public sealed class DIEntrySingleton<T> : DIEntry<T>
    {
        private T _instance;
        
        public DIEntrySingleton(DIContainer diContainer, Func<DIContainer, T> factory) : base(diContainer, factory) { }
        
        public override T Resolve()
        {
            if (_instance == null)
            {
                _instance = Factory(DiContainer); // Выполняет переданный делегат и возвращает тип Т
        }

            return _instance;
        }
    }
