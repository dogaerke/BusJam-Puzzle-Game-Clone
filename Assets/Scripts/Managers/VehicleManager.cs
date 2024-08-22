using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class VehicleManager : MonoBehaviour
{
    [SerializeField] private BusContainer busContainer;
    [SerializeField] private PlaneContainer planeContainer;
    [SerializeField] private ShipContainer shipContainer;
    [SerializeField] private TrainContainer trainContainer;
    
    private List<Vehicle> _vehicleList;
    private List<Vehicle> _removedVehicles;
    
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject greenQuad;
    [SerializeField] private Material cloudMaterial;
    [SerializeField] private Material roadMaterial;
    [SerializeField] private Material planeMaterial;
    [SerializeField] private Material seaMaterial;
    [SerializeField] private Material railMaterial;
    
    [Header("Vehicle Points")]
    [SerializeField] private Transform secondVehiclePoint;
    [SerializeField] private Transform vehicleStopPoint;
    [SerializeField] private Transform vehicleExitPoint;
    [SerializeField] private GameObject cloud;
    [SerializeField] private GameObject greyQuad;


    private Vector3 _defaultRoadScale;
    private GameObject _cloud1;
    private GameObject _cloud2;
    private GameObject _cloud3;
    private GameObject _greyQuad;
    
    private GameDataSO _gameDataSo;
    public static VehicleManager Instance { get; private set; }

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
        _vehicleList = new List<Vehicle>();
        _removedVehicles = new List<Vehicle>();
        
        road.gameObject.SetActive(false);
        greenQuad.gameObject.SetActive(false);
        
        _defaultRoadScale = road.transform.localScale;
    }

    public void InitializeLevel()
    {
        var position = vehicleStopPoint.position;
        position.x = GridManager.Instance.width / 2;
        position.z = GridManager.Instance.height + 3;
        
        if (GridManager.Instance.width % 2 == 0)
            position.x -= 0.5f;
        
        vehicleStopPoint.position = position;

        road.transform.position = position;
        road.gameObject.SetActive(true);
        
        position.z = GridManager.Instance.height - 8;
        greenQuad.gameObject.SetActive(true);
        greenQuad.transform.position = position;
        
        var secondPos = secondVehiclePoint.position;
        secondPos.x = GridManager.Instance.width / 2 - 6f;
        secondPos.z = GridManager.Instance.height + 3;
        
        if (GridManager.Instance.width % 2 == 1)
            secondPos.x += 0.5f;
        
        secondVehiclePoint.position = secondPos; 

        position = vehicleExitPoint.position;
        position.z = GridManager.Instance.height + 3;
        vehicleExitPoint.position = position;

        if (LevelManager.Instance.CurrentLevel >= 30 && LevelManager.Instance.CurrentLevel < 40)
        {
            _cloud1 = Instantiate(cloud, transform);
            _cloud2 = Instantiate(cloud, transform);
            _cloud3 = Instantiate(cloud, transform);
        }  
        else if (LevelManager.Instance.CurrentLevel >= 20 && LevelManager.Instance.CurrentLevel < 30)
        {
            _greyQuad = Instantiate(greyQuad, transform);
            
        }

        
        GenerateVehicle();

        _vehicleList[0].MoveVehicleToLine();
        if (_vehicleList.Count > 1)
            _vehicleList[1].transform.DOMove(secondVehiclePoint.position, 1f).SetEase(Ease.OutQuart);
        

    }

    public void SetGameData(GameDataSO gameData) { _gameDataSo = gameData; }

    private void GenerateVehicle()
    {
        
        foreach (var t in _gameDataSo.busColorList)
        {
            Vehicle vehicle;
            if(LevelManager.Instance.CurrentLevel >= 10 && LevelManager.Instance.CurrentLevel < 20)
            {
                vehicle = Instantiate(trainContainer.trainCatalog[t], transform);
                road.transform.localScale = _defaultRoadScale;
                road.GetComponent<MeshRenderer>().material  = railMaterial;
                
            }
            else if(LevelManager.Instance.CurrentLevel >= 20 && LevelManager.Instance.CurrentLevel < 30)
            {
                vehicle = Instantiate(shipContainer.shipCatalog[t], transform);
                var scale = road.transform.localScale;
                scale.x = 17;
                scale.y = 9.5f;
                road.transform.localScale = scale;
                var position = vehicleStopPoint.position;
                var pos = position;
                pos.z += 3;
                road.transform.position = pos;
                
                var pos3 = position;
                pos3.z -= +1.5f;
                _greyQuad.transform.position = pos3;
                
                road.GetComponent<MeshRenderer>().material  = seaMaterial;
            }
            else if(LevelManager.Instance.CurrentLevel >= 30 && LevelManager.Instance.CurrentLevel < 40)
            {
                vehicle = Instantiate(planeContainer.planeCatalog[t], transform);
                road.transform.localScale = _defaultRoadScale;
                road.GetComponent<MeshRenderer>().material = planeMaterial;

                var position = vehicleStopPoint.position;
                
                var pos = position;
                pos.z += 2.1f;
                pos.x -= 1;
                _cloud1.transform.position = pos;
                
                var pos1 = position;
                pos1.z += 3.65f;
                pos1.x -= 2.7f;
                _cloud2.transform.position = pos1;
                
                var pos2 = position;
                pos2.z += 2.8f;
                pos2.x += 2.3f;
                _cloud3.transform.position = pos2;

            }
            else
            {
                vehicle = Instantiate(busContainer.busCatalog[t], transform);
                road.transform.localScale = _defaultRoadScale;
                road.GetComponent<MeshRenderer>().material  = roadMaterial;

            }

            //Z component of instantiated bus.
            var vector3 = vehicle.transform.position; 
            vector3.z = GridManager.Instance.height + 3;
            vehicle.transform.position = vector3;
            
            vehicle.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            
            vehicle.StopPoint = vehicleStopPoint;
            vehicle.ExitPoint = vehicleExitPoint;
            
            vehicle.OnReach += LineManager.Instance.HandleOnVehicleReach;
            vehicle.OnExit += LineManager.Instance.HandleOnVehicleExit;
            vehicle.OnExit += HandleVehicleOnExit;
            
            vehicle.Attributes.VehicleColor = t;
            _vehicleList.Add(vehicle);
        }

        foreach (var veh in _vehicleList)
        {
            veh.BookersSittingInVehicle[0].SetActive(false);
            veh.BookersSittingInVehicle[1].SetActive(false);
            veh.BookersSittingInVehicle[2].SetActive(false);
        }

        for (var i = 0; i < _gameDataSo.vehicleReservationList.Count ; i++)
        {
            _vehicleList[i].isReserveRequired = _gameDataSo.vehicleReservationList[i].isReserved;
            _vehicleList[i].reservationInfo = _gameDataSo.vehicleReservationList[i].reserveRequire;
        }

        foreach (var veh in _vehicleList.Where(veh => veh.isReserveRequired))
        {
            veh.ReservationFlags[veh.reservationInfo - 1].SetActive(true);
        }
        
        var v = _vehicleList.LastOrDefault();
        if(v)
            v.OnEnd += LastBusEndsGame;
    }

    public void HandleVehicleOnExit(Vehicle vehicle)
    {
        vehicle.OnExit -= HandleVehicleOnExit;
        _vehicleList.Remove(vehicle);
        _removedVehicles.Add(vehicle);
        if (_vehicleList.Count > 0)
        {
            _vehicleList[0].MoveVehicleToLine();
            if (_vehicleList.Count > 1)
                _vehicleList[1].transform.DOMove(secondVehiclePoint.position, 1f).SetEase(Ease.OutQuart);
        }
        
        
    }

    public void LastBusEndsGame(Vehicle vehicle)
    {
        vehicle.OnEnd -= LastBusEndsGame;
        UIManager.Instance.GameEndsWithWin();
    }

    public void ClearVehicle()
    {
        if(_removedVehicles == null) return;

        var length = _removedVehicles.Count;
        for (var i = 0; i < length; i++)
        {
            Destroy(_removedVehicles[i].gameObject);
        }
        
        _removedVehicles?.Clear();
        
        if(_removedVehicles == null) return;

        length = _vehicleList.Count;
        for (var i = 0; i < length; i++)
        {
            Destroy(_vehicleList[i].gameObject);
        }
        if(_cloud1)
            Destroy(_cloud1);
        if(_cloud2)
            Destroy(_cloud2);
        if(_cloud2)
            Destroy(_cloud3);
        if(_greyQuad)
            Destroy(_greyQuad);
        
        _vehicleList?.Clear();
        
    }
    
}