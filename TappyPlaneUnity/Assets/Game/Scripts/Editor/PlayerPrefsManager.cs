using UnityEditor;
using UnityEngine;

public class PlayerPrefsEditorHelper
{
    [MenuItem("Development/Delete Player Prefs")]
    static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}