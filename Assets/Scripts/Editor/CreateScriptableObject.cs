using UnityEngine;
using UnityEditor;

public class CreateScriptableObject
{
    private static GameDataSO _gameDataSo;

    public static GameDataSO GameDataSo => _gameDataSo;

    public static void CreateAsset()
    {
        _gameDataSo = ScriptableObject.CreateInstance<GameDataSO>();

        string name = "Level_" +  ".asset";
        AssetDatabase.CreateAsset(_gameDataSo, "Assets/GameData/" + name);
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = _gameDataSo;
    }
    
    
}