using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("WARNINGS")]
    [SerializeField] [TextArea] public string warnings;

    [Header("Tile Width and Height")]
    [SerializeField] [Range(0, 100)] public int width;
    [SerializeField] [Range(0, 100)] public int height;
    
    [Header("TILES")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private ObstacleSelector obstacleSelector;
    [SerializeField] private ObstacleMatCatalog obstacleMatCatalog;
    
    [SerializeField] private List<CellData> cellDataList;

    [Header("BookerList")]
    [SerializeField] private List<Booker> bookerList;
    
    [Header("Vehicle list")] 
    [SerializeField] private List<GameColors> vehicleColorList;
    [SerializeField] private List<ReservationVehicleAttributes> vehicleReservationList;
    
    [Header("SubwayList")] 
    [SerializeField] public List<SubwayStation> subwayStations;
    public Dictionary<CellData, SubwayStation> stationCellDict;

    
    [Header("Level Catalog SO")]
    [SerializeField] private LevelCatalog levelCatalog;
    
    private GameDataSO _gameDataSo;
    private PathFinding _pathFinding;

    private GameObject subwayParent;
    //private bool isSec
    
    [Header("EDITOR")]
    public Transform createdCharacters;
    public BookerContainer bookerContainer;
    public SubwayStation subwayPrefab;
    
    public GameColors gameColors;

    public Dictionary<GameColors, int> colorCounterDict;

    public List<CellData> CellDataList => cellDataList;
    public List<Booker> BookerList => bookerList;
    
    
    public Dictionary<Vector2, Tile> Tiles { get; set; }
    public Dictionary<Vector2, GameColors> EditorBookersColorsMap { get; set; }
    

    public List<GameColors> VehicleColorList
    {
        get => vehicleColorList;
        set => vehicleColorList = value;
    }

    public List<ReservationVehicleAttributes> VehicleReservationList
    {
        get => vehicleReservationList;
        set => vehicleReservationList = value;
    }


    public static GridManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        else
            Instance = this;
    }

    private void Start()
    {
        subwayParent = new GameObject
        {
            name = "SubwayParent",
            transform =
            {
                rotation = Quaternion.identity
            }
        };
    }

    public void SetGameData(GameDataSO gameData) { _gameDataSo = gameData; }

    public void InitializeLevel()
    {
        width = _gameDataSo.width;
        height = _gameDataSo.height;
        cellDataList = _gameDataSo.charactersColorMap;
        GenerateGridsInGame();
        GenerateSubwayStations();
    }

    public void CheckSubwayObstacle()
    {
        foreach (var station in subwayStations)
        {
            var target = (station.transform.position + station.Direction).Convert3DTo2D();

            if (!Tiles.TryGetValue(target, out var targetTile)) continue;
            
            if (targetTile.hasObstacle) continue;
            
            station.MoveFirstBooker();
        }
    }
    public void GenerateGridsInGame()
    {
        Tiles = new Dictionary<Vector2, Tile>();
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                CreateTileInGame(x, y);
            }
        }

        foreach (var cell in _gameDataSo.charactersColorMap.Where(cell => cell.hasObstacle && !cell.isSubway))
        {
            if(!Tiles.TryGetValue(new Vector2(cell.x, cell.y), out var tile)) continue;
            
            var obstacleCode = obstacleSelector.CalculateObstacle(Tiles, tile);
            tile.tileRenderer.sharedMaterial = obstacleMatCatalog.ObstacleSelector(obstacleCode);
        }
    }

    private void CreateTileInGame(int x, int y)
    {
        var tile = Instantiate(tilePrefab, transform);
        
        tile.transform.position = new Vector3(x, 0, y);
        tile.x = x;
        tile.y = y;
        Tiles.Add(new Vector2(x, y), tile);
        
        tile.isSubway = _gameDataSo.charactersColorMap[height * x + y].isSubway;
        tile.hasObstacle = tile.isSubway || _gameDataSo.charactersColorMap[height * x + y].hasObstacle;
        
    }
    
    private void GenerateSubwayStations()
    {
        var cdList = _gameDataSo.charactersColorMap;
        
        foreach (var cd in cdList.Where(cd => cd.isSubway))
        {
            var subway = Instantiate(subwayPrefab, subwayParent.transform);
            
            subway.transform.position = new Vector3(cd.x, 0, cd.y);

            subway.Direction = cd.subwayDirection.Set3DDirection();

            var targetRotation = Quaternion.LookRotation(subway.Direction);
            subway.MeshRoot.rotation = targetRotation;
            
            cd.subwayStation = subway;

            cd.hasObstacle = true;
            
            Tiles[new Vector2(cd.x, cd.y)].gameObject.SetActive(true);
            
            subwayStations.Add(subway);

        }
    }

    public void ClearGrids()
    {
        if(Application.isPlaying) return;

        // Create a list to hold the objects to destroy
        List<GameObject> objectsToDestroy = new List<GameObject>();
        
        foreach (Transform t in createdCharacters)
        {
            objectsToDestroy.Add(t.gameObject);
        }
        
        foreach (GameObject obj in objectsToDestroy)
        {
            DestroyImmediate(obj);
        }

        for (int i = 0; i < subwayStations?.Count; i++)
        {
            if(!subwayStations[i]) continue;
            
            DestroyImmediate(subwayStations[i].gameObject);
        }
        
        cellDataList.Clear();
        vehicleColorList.Clear();
        vehicleReservationList.Clear();
        bookerList.Clear();

        subwayStations?.Clear();

        warnings = "";
    }


    public void GenerateGridsInEditor()
    {
        Tiles = new Dictionary<Vector2, Tile>();

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                CreateTileInEditor(x, y);
            }
        }
    }

    

    private void CreateTileInEditor(int i, int j)
    {
        if(Application.isPlaying) return;

#if UNITY_EDITOR
        var tileGo = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(tilePrefab.gameObject);
        tileGo.transform.SetParent(transform);
        tileGo.transform.position = new Vector3(i, 0, j);
        tileGo.transform.rotation = Quaternion.identity;
        var tile = tileGo.GetComponent<Tile>();
        tile.x = i;
        tile.y = j;
        Tiles.Add(new Vector2(i, j), tile);


#else
        var spawnedTile = Instantiate(tilePrefab, new Vector3(i, 0, j), Quaternion.identity, transform);
        spawnedTile.x = i;
        spawnedTile.y = j;

        Tiles.Add(new Vector2(i, j), spawnedTile);
#endif
    }

    public void SaveData()
    {
        if(Application.isPlaying) return;

#if UNITY_EDITOR
        //Instantiate gameData
        _gameDataSo = ScriptableObject.CreateInstance<GameDataSO>();
        
        string name = "Level_New" + ".asset";
        //Save Game Data To Assets/GameData/Level_X.asset
        AssetDatabase.CreateAsset(_gameDataSo, "Assets/GameData/LevelData/" + name);
        

        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = _gameDataSo;
        
        SaveInGameData();
        levelCatalog.gameLevels.Add(_gameDataSo);
        EditorUtility.SetDirty(levelCatalog);
#endif
    }

    private void SaveInGameData()
    {
        if(Application.isPlaying) return;
#if UNITY_EDITOR
        _gameDataSo.charactersColorMap = new List<CellData>();
        foreach (var cd in cellDataList)
        {
            _gameDataSo.charactersColorMap.Add(cd);
        }

        _gameDataSo.busColorList = new List<GameColors>();
        foreach (var bc in vehicleColorList)
        {
            _gameDataSo.busColorList.Add(bc);
        }

        _gameDataSo.vehicleReservationList = new List<ReservationVehicleAttributes>();
        foreach (var r in vehicleReservationList)
        {
            _gameDataSo.vehicleReservationList.Add(r);
        }

        _gameDataSo.width = width;
        _gameDataSo.height = height;
        

        EditorUtility.SetDirty(gameObject);
        EditorUtility.SetDirty(_gameDataSo);
        AssetDatabase.SaveAssetIfDirty(_gameDataSo);

#endif
    }
}

[Serializable]
public struct ReservationVehicleAttributes
{
    public bool isReserved;
    [Range(1,3)] public short reserveRequire;

    public ReservationVehicleAttributes(bool isReserved, short reserveRequire)
    {
        this.isReserved = isReserved;
        this.reserveRequire = reserveRequire;
    }
}
