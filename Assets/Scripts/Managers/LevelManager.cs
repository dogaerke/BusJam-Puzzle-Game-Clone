using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelCatalog levelCatalog;

    private const string Key = "Level_";
    [SerializeField] private int currentLevel;
    
    public LevelCatalog Catalog => levelCatalog;

    public int CurrentLevel => currentLevel;

    public static LevelManager Instance { get; private set; }
    private void Awake()
    {
        
        if (Instance != null && Instance != this)
            Destroy(this);

        else
            Instance = this;
        
        currentLevel = GetLevel();
    }
    
    public  void SaveLevel() { PlayerPrefs.SetInt(Key, currentLevel); }

    public void IncreaseLevel()
    {
        
        if (currentLevel < levelCatalog.gameLevels.Count - 1)
        {
            currentLevel++;
        }
        else
            Debug.Log("All levels are finished.");
        SaveLevel();
    }

    private  int GetLevel()
    {
        return PlayerPrefs.HasKey(Key) ? PlayerPrefs.GetInt(Key) : 0;
    }
}