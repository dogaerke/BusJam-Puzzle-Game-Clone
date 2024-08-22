
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelCatalog", menuName = "ScriptableObjects/LevelCatalog", order = 2)]

[System.Serializable]
public class LevelCatalog : ScriptableObject
{
    public List<GameDataSO> gameLevels;

#if UNITY_EDITOR
    void SaveGameData()
    {
        AssetDatabase.SaveAssets();

    }
#endif
    
}