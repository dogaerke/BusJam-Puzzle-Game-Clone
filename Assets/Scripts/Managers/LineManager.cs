using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class LineManager : MonoBehaviour
{
    [Header("Line Transforms List")]
    [SerializeField] private List<Transform> linePlaces;
    
    [Header("Bookers List")]
    [SerializeField] private List<Booker> bookers;//Bookers that complete its path

    [SerializeField] private List<Booker> unlistedBookers;
    
    [SerializeField] private List<Booker> bookersThatWillBeRemoved;//Bookers that will be removed after bus complete its process.
    
    
    [Header("Bus That Arrived")]
    [SerializeField] private Vehicle arrivingVehicle;
    
    private Dictionary<Vector3, bool> lineChecker;
    private static readonly int Running = Animator.StringToHash("Running");
    public List<Booker> Bookers => bookers;
    public static LineManager Instance { get; private set; }

    public List<Transform> LinePlaces => linePlaces;

    private bool endGameControl;


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
        foreach (var l in linePlaces)
            l.gameObject.SetActive(false);
        
    }

    public void Initialize()
    {
        lineChecker = new Dictionary<Vector3, bool>();
        endGameControl = false;
        var count = 0;
        var width = GridManager.Instance.width;
        foreach (var l in linePlaces)
        {
            var vector3 = l.position;
            l.gameObject.SetActive(true);
            vector3.z = GridManager.Instance.height + 1;
            vector3.x = width / 2 - 2 + count++ * 1.0f;
            
            if (width % 2 == 0)
                vector3.x -= 0.5f;
            
            l.position = vector3;
            lineChecker.Add(l.position, true);
        }
        
    }

    public void TryBookForBooker(Booker booker)
    {
        booker.OnReach -= TryBookForBooker;
        
        if(endGameControl) return;
        
        if(arrivingVehicle)
        {
            if (arrivingVehicle.bookers.Count < 3 &&
                arrivingVehicle.Attributes.VehicleColor == booker.Attributes.bookerColor)
            {
                booker.MoveCharacterToBus(arrivingVehicle);
                return;
            }
        }
        
        AddBookerToLine(booker);
        
    }

    private IEnumerator Wait(float sec)
    {
        yield return new WaitForSeconds(sec);
    }
    public void AddBookerToLine(Booker booker)
    {
        bookers.Add(booker);

        foreach (var place in linePlaces)
        {
            if (!lineChecker.TryGetValue(place.position, out var empty)) continue;
            
            if(!empty) continue;
            
            var position = place.position;
            lineChecker[position] = false;
            booker.BookedPos = position;
            MoveAndLook(booker, position);
            return;
        }

        bookers.Remove(booker);
        unlistedBookers.Add(booker);
        booker.BookerAnim.SetBool(Running, false);
    }

    private void MoveAndLook(Booker booker, Vector3 position)
    {
        var targetPos = new Vector3(position.x, booker.transform.position.y, position.z);
        booker.transform.LookAt(targetPos);
            
        booker.transform.DOMove(position + Vector3.up, .5f).OnComplete(() =>
        {
            booker.transform.rotation = Quaternion.Euler(Vector3.zero);
            booker.BookerAnim.SetBool(Running, false);
            if(arrivingVehicle)
            {
                if (arrivingVehicle.bookers.Count < 2)
                {
                    EndGameController();
                }

                if (arrivingVehicle.bookers.Count == 2)
                {
                    StartCoroutine(Wait(1.0f));
                    EndGameController();
                }
            }
        }).SetEase(Ease.Linear);
    }

    private void EndGameController()
    {
        var flag = false;
        foreach (var lp in linePlaces)
        {
            if (lineChecker[lp.position])
            {
                flag = true;
                break;
            }
        }

        if (flag) return;
        
        PlayerInputHandler.Instance.gameObject.SetActive(false);
        UIManager.Instance.GameEndsWithLose();
        DOTween.KillAll();
        foreach (var booker in BookerManager.Instance.BookerDict.Values)
        {
            booker.BookerAnim.SetBool(Running,false);
        }

        foreach (var b in BookerManager.Instance.allSubwayBookers)
        {
            b.BookerAnim.SetBool(Running, false);
        }
        
        endGameControl = true;

    }

    public void HandleBookerExit(Booker booker)
    {
        lineChecker[booker.BookedPos] = true;
        
        booker.OnExit -= HandleBookerExit;
    }

    public void HandleOnVehicleReach(Vehicle vehicle)
    {
        arrivingVehicle = vehicle;

        var flag = false;
        var length = bookers.Count;
        for (var i = 0; i < length; i++)
        {
            var booker = bookers[i];
            if(booker.Attributes.bookerColor != vehicle.Attributes.VehicleColor) continue;
            if (ColorControl(vehicle, booker) || ReserveControl(vehicle,booker))
            {
                flag = true;
                booker.MoveCharacterToBus(vehicle);

                if (vehicle.bookers.Count != 3) continue;
                return;
            }
            
        }

        length = unlistedBookers.Count;
        for (int i = 0; i < length; i++)
        {
            var booker = unlistedBookers[i];
            AddBookerToLine(booker);
            unlistedBookers.Remove(booker);
        }

        if (!flag && vehicle)
        {
            EndGameController();
        }

    }

    private bool ReserveControl(Vehicle vehicle, Booker booker)
    {
        return vehicle.isReserveRequired && !vehicle.reservationArrive &&
               booker.IsReserved && vehicle.reservationCount < vehicle.reservationInfo ||
               !booker.IsReserved && vehicle.noReservationCount < 3 - vehicle.reservationInfo;
    }
    private bool ColorControl(Vehicle vehicle, Booker booker)
    {
        return booker.Attributes.bookerColor == vehicle.Attributes.VehicleColor && !vehicle.isReserveRequired;
    }

    public void HandleOnVehicleExit(Vehicle vehicle)
    {
        foreach (var booker in arrivingVehicle.bookers)
        {
            bookers.Remove(booker);
        }
        arrivingVehicle = null;
        vehicle.OnExit -= HandleOnVehicleExit;
    }

    public void ClearLevel()
    {
        bookers?.Clear();
        lineChecker?.Clear();
    }

    public int CountEmptyPlaces()
    {
        return linePlaces.Count(place => lineChecker[place.position]);
    }
}
