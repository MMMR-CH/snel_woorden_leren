using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SWL
{
    public static class SWL_Debug
    {
        public static bool isDebug => Application.isEditor || Debug.isDebugBuild;

        public static void Log(string message)
        {
            if (!isDebug) return;
            Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            if (!isDebug) return;
            Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            if (!isDebug) return;
            Debug.LogError(message);
        }
    }
}
