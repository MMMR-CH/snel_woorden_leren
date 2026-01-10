using UnityEditor;

[InitializeOnLoad]
public class AutoKeystore
{
    static AutoKeystore()
    {
        PlayerSettings.Android.keystorePass = "snel_leren_48";
        PlayerSettings.Android.keyaliasPass = "snel_leren_48";
    }
}