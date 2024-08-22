using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BookerManager : MonoBehaviour
{
    [Header("Booker Prefab")]
    [SerializeField] private Booker secretBooker;
    [SerializeField] private BookerContainer bookerContainer;

    public Dictionary<Vector2, Booker> BookerDict;
    public List<Booker> allSubwayBookers;
    
    private GameDataSO _gameDataSo;

    public Booker SecretBookerInstance => secretBooker;
    
    public static BookerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        else
            Instance = this;

    }

    private void Start()
    {
        BookerDict = new Dictionary<Vector2, Booker>();
    }

    public void InitializeLevel()
    {
        GenerateBooker();
    }

    public void SetGameData(GameDataSO gameData) { _gameDataSo = gameData; }

    private void GenerateBooker()
    {
        foreach (var t in _gameDataSo.charactersColorMap)
        {
            if (CreateSubwayBookers(t)) continue;
            
            if(t.gameColors == GameColors.DEFAULT) continue;
            
            //Booker Instantiation
            var initialPos = new Vector3(t.x, 0.9f, t.y);
            Booker newBooker;
            
            //Secret booker
            if (t.isSecret)
            {
                newBooker = Instantiate(secretBooker, initialPos, Quaternion.identity, transform);
                newBooker.IsSecret = true;
            }
            else // Default booker
            {
                newBooker = Instantiate(bookerContainer.bookerCatalog[t.gameColors], 
                    initialPos, Quaternion.identity, transform);

                if (t.isReserved)
                    newBooker.ReservationHat.SetActive(true);

            }
            
            BookerDict.Add(new Vector2(t.x, t.y),newBooker);

            GridManager.Instance.Tiles[new Vector2(t.x,t.y)].hasObstacle = true;
            
            newBooker.Attributes.bookerColor = t.gameColors;
            newBooker.IsReserved = t.isReserved;
            
            
            newBooker.OnReach += LineManager.Instance.TryBookForBooker;
            newBooker.OnExit += LineManager.Instance.HandleBookerExit;
            newBooker.OnClick += SecretBooker;
            
            
        }

        AddBookerAtSubwayDirection(GridManager.Instance.subwayStations);
        
    }

    private bool CreateSubwayBookers(CellData t)
    {
        if (!t.isSubway) return false;

        var index = 0;
        foreach (var color in t.bookersInSubway)
        {
            var subwayBooker = Instantiate(bookerContainer.bookerCatalog[color], t.subwayStation.transform);
            
            subwayBooker.gameObject.SetActive(false);
            t.subwayStation.StationBookerList.Add(subwayBooker);
            subwayBooker.Attributes.bookerColor = color;
            subwayBooker.IsReserved = t.reservedSubwayBookers[index];
                    
            subwayBooker.OnReach += LineManager.Instance.TryBookForBooker;
            subwayBooker.OnExit += LineManager.Instance.HandleBookerExit;
            subwayBooker.OnClick += SecretBooker;
            subwayBooker.OnClick += t.subwayStation.CallBooker;

            index++;
        }

        return true;

    }

    private void AddBookerAtSubwayDirection(List<SubwayStation> subwayStations)
    {
        foreach (var station in subwayStations)
        {
            //Register booker at the direction of subway. 
            var pos = station.transform.position.Convert3DTo2D() + station.Direction.Convert3DTo2D();
            BookerDict.TryGetValue(pos, out var booker);
            
            if (!booker) continue;
            
            booker.OnClick += station.CallBooker;
        }
    }

    public void ClearBookers()
    {
        if(BookerDict == null) return;
        
        var destroyeObj = new List<Booker>();
        
        foreach (var booker in BookerDict.Values)
        {
            destroyeObj.Add(booker);
        }
        
        var len = destroyeObj.Count;
        
        for (var i = 0; i < len; i++)
        {
            Destroy(destroyeObj[i].gameObject);
        }
        BookerDict?.Clear();

        if (allSubwayBookers == null) return;
        
        var length = allSubwayBookers.Count;
        for (var i = 0; i < length; i++)
        {
            Destroy(allSubwayBookers[i].gameObject);
        }
        
        allSubwayBookers?.Clear();
    }
    
    private void SecretBooker(Booker booker)
    {
        booker.OnExit -= SecretBooker;
        
        var position = booker.transform.position;
        var bookerPos = new Vector2(position.x, position.z);
        
        if(BookerDict.TryGetValue(bookerPos + Vector2.up, out var neighbour))
        {
            if (neighbour.IsSecret)
            {
                neighbour.IsSecret = false;
                neighbour.rendererBooker.sharedMaterial = bookerContainer.bookerCatalog[neighbour.Attributes.bookerColor]
                    .rendererBooker.sharedMaterial;
            }
        }
        
        if(BookerDict.TryGetValue(bookerPos + Vector2.down, out neighbour))
        {
            if (neighbour.IsSecret)
            {
                neighbour.IsSecret = false;
                neighbour.rendererBooker.sharedMaterial = bookerContainer.bookerCatalog[neighbour.Attributes.bookerColor]
                    .rendererBooker.sharedMaterial;
            }
        }
        
        if(BookerDict.TryGetValue(bookerPos + Vector2.right, out neighbour))
        {
            if (neighbour.IsSecret)
            {
                neighbour.IsSecret = false;
                neighbour.rendererBooker.sharedMaterial = bookerContainer.bookerCatalog[neighbour.Attributes.bookerColor]
                    .rendererBooker.sharedMaterial;
            }
        }
        
        if(BookerDict.TryGetValue(bookerPos + Vector2.left, out neighbour))
        {
            if (neighbour.IsSecret)
            {
                neighbour.IsSecret = false;
                neighbour.rendererBooker.sharedMaterial = bookerContainer.bookerCatalog[neighbour.Attributes.bookerColor]
                    .rendererBooker.sharedMaterial;
            }
        }
        
    }

}