
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManagerController : MonoBehaviour
{
    [SerializeField] private LevelCatalog levelCatalog;
    [SerializeField] private TMP_Text levelCount;
    [SerializeField] private CountDown countDownSc;
    
    
    public static ManagerController Instance { get; private set; }

    public TMP_Text LevelCount
    {
        get => levelCount;
        set => levelCount = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        levelCount.SetText((LevelManager.Instance.CurrentLevel + 1).ToString());
    }

    public void LoadGameData()
    {
        if (countDownSc.HeartLeft <= 0)
        {
            Debug.Log("No Heart Left");
        }
        ClearLevel();
        
        var level = levelCatalog.gameLevels[LevelManager.Instance.CurrentLevel];
        LevelCount.SetText((LevelManager.Instance.CurrentLevel + 1).ToString());
        
        GridManager.Instance.SetGameData(level);
        BookerManager.Instance.SetGameData(level);
        VehicleManager.Instance.SetGameData(level);
        SelectionManager.Instance.ClickedBookerCount = 0;
        
        InitializeLevels();
    }

    private void InitializeLevels()
    {
        GridManager.Instance.InitializeLevel();
        BookerManager.Instance.InitializeLevel();
        VehicleManager.Instance.InitializeLevel();
        LineManager.Instance.Initialize();
        GridManager.Instance.CheckSubwayObstacle();
        CameraController.Instance.IntegrateProperties();
    }

    private void ClearLevel()
    {
        LineManager.Instance.ClearLevel();
        VehicleManager.Instance.ClearVehicle();
        BookerManager.Instance.ClearBookers();
        ClearTiles();
        
    }

    private static void ClearTiles()
    {
        if (GridManager.Instance.Tiles == null) return;
        var destroyedObj = new List<Tile>();

        foreach (var tile in GridManager.Instance.Tiles.Values)
        {
            destroyedObj.Add(tile);
        }
        
        var length = destroyedObj.Count;
        
        for (int i = 0; i < length; i++)
        {
            Destroy(destroyedObj[i].gameObject);
        }

        var subwayList = GridManager.Instance.subwayStations;
        for (int i = 0; i < subwayList.Count; i++)
        {
            Destroy(subwayList[i].gameObject);
        }
        subwayList.Clear();
        
        GridManager.Instance.Tiles?.Clear();
    }
}