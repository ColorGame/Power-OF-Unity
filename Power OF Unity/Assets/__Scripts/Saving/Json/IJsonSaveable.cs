using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Saving
{
    public interface IJsonSaveable
    {
        /// <summary>
        /// Переопределение для возврата токена, представляющего состояние IJsonSaveable
        /// </summary>
        /// <returns>A JToken</returns>
        JToken CaptureAsJToken();
        /// <summary>
        /// Восстановите состояние компонента, используя информацию, содержащуюся в токене.
        /// </summary>
        /// <param name="state">JToken объект, представляющий состояние модуля</param>
        void RestoreFromJToken(JToken state);
    }


    // Вспомогательный класс для преобразования Vector3 в Token и обратно
    public static class JsonStatics
    {

        public static JToken ToToken(this Vector3 vector)
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["x"] = vector.x;
            stateDict["y"] = vector.y;
            stateDict["z"] = vector.z;
            return state;
        }

        public static Vector3 ToVector3(this JToken state)
        {
            Vector3 vector = new Vector3();
            if (state is JObject jObject)
            {
                IDictionary<string, JToken> stateDict = jObject;

                if (stateDict.TryGetValue("x", out JToken x))
                {
                    vector.x = x.ToObject<float>();
                }

                if (stateDict.TryGetValue("y", out JToken y))
                {
                    vector.y = y.ToObject<float>();
                }

                if (stateDict.TryGetValue("z", out JToken z))
                {
                    vector.z = z.ToObject<float>();
                }
            }
            return vector;
        }
    }

}
