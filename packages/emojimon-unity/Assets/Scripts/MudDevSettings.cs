#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[FilePath("Playmint/MudDevSettings.cfg", FilePathAttribute.Location.PreferencesFolder)]
public class MudDevSettings : ScriptableSingleton<MudDevSettings>
{
    [SerializeField]
    public string NodePath = "/path/to/node";

    [SerializeField]
    public string PrivateKey = "0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80";

    public void SaveSettings()
    {
        Save(true);
    }
}

#endif
