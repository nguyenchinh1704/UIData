// #define FLEXALON_LOG

using UnityEngine;

namespace Flexalon
{
    internal class FlexalonLog
    {
        public static void Log(string message)
        {
            #if FLEXALON_LOG
                Debug.Log(message);
            #endif
        }

        public static void Log(string message, FlexalonNode node)
        {
            #if FLEXALON_LOG
                Debug.Log(message + " (" + node?.GameObject?.name + ")");
            #endif
        }

        public static void Log<T>(string message, FlexalonNode node, T value)
        {
            #if FLEXALON_LOG
                Debug.Log(message + " (" + node?.GameObject?.name + "): " + value);
            #endif
        }
    }
}