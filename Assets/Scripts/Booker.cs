using System;
using System.Collections;
using System.Collections.Generic;
using BookerNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class Booker : MonoBehaviour
{
    [SerializeField] private BookerAttributes bookerAttributes;
    [SerializeField] private Animator bookerAnim;
    
    [SerializeField] private bool isSecret;
    [SerializeField] private bool isReserved;
    [SerializeField] private GameObject reservationHat;

    [SerializeField] private BoxCollider boxCollider;
    
    public Renderer rendererBooker;
    private Vector3 _bookedPos;
    private bool _reserveFlag;

    public event Action<Booker> OnClick; 
    public event Action<Booker> OnReach;
    public event Action<Booker> OnExit;

    public Vector3 BookedPos
    {
        get => _bookedPos;
        set => _bookedPos = value;
    }
    public bool IsSecret
    {
        get => isSecret;
        set => isSecret = value;
    }

    public BookerAttributes Attributes => bookerAttributes;

    public Animator BookerAnim
    {
        get => bookerAnim;
        set => bookerAnim = value;
    }

    public bool IsReserved
    {
        get => isReserved;
        set => isReserved = value;
    }

    public GameObject ReservationHat
    {
        get => reservationHat;
        set => reservationHat = value;
    }

    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int DoorOpen = Animator.StringToHash("doorOpen");

    public void MoveCharacterToBus(Vehicle vehicle)
    {
        var targetPos = vehicle.VehicleDoor.position;
        targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        transform.LookAt(targetPos);
        
        if (vehicle.isReserveRequired && !vehicle.reservationArrive
                                      && !IsReserved && 3 - vehicle.reservationInfo == vehicle.noReservationCount 
                                         || (IsReserved && vehicle.reservationInfo == vehicle.reservationCount))
        {
            LineManager.Instance.AddBookerToLine(this);
            return;
        }

        if (!IsReserved)
            vehicle.noReservationCount++;

        OnExit?.Invoke(this);
        vehicle.bookers.Add(this);
        
        if (vehicle.isReserveRequired && !vehicle.reservationArrive)
        {
            if(IsReserved && vehicle.reservationCount < vehicle.reservationInfo)
            {
                vehicle.reservationCount++;
                if (vehicle.reservationCount == vehicle.reservationInfo && 3 - vehicle.reservationInfo == vehicle.noReservationCount)
                {
                    vehicle.reservationArrive = true;
                }
            }
        }
        
        
        bookerAnim.SetBool(Running, true);
        vehicle.DoorAnim.SetBool(DoorOpen, true);
        
        
        transform.DOMove(targetPos, .5f).OnComplete(() =>
        {
            if(IsReserved)
                vehicle.BookerSittingResHats[vehicle.bookerCount].SetActive(true);
            vehicle.BookersSittingInVehicle[vehicle.bookerCount++].SetActive(true);
            vehicle.DoorAnim.SetBool(DoorOpen, false);
            vehicle.HandleExit();
            gameObject.SetActive(false);
        });
    }
    
    private readonly List<List<Tile>> _paths = new();
    private int _pathIndex = 1;

    public bool FindAPath()
    {
        var pathFinding = new PathFinding();
        var vec2Pos = new Vector2(transform.position.x, transform.position.z);
        for (var i = 0; i < GridManager.Instance.width; i++)
        {
            var path = pathFinding.FindPath(vec2Pos, new Vector2(i,GridManager.Instance.height - 1));
            if (path == null)
            {
                return false;
            }
            _paths.Add(path);
        }
        
        if(_paths == null) return false;
        
        boxCollider.enabled = false;
        
        var pathMinCost = _paths[0];
        for(var i = 1; i < _paths.Count; i++)
        {
            if (_paths[i].Count < pathMinCost.Count)
                pathMinCost = _paths[i];
        }        
        
        GridManager.Instance.Tiles[vec2Pos].hasObstacle = false;
        
        bookerAnim.SetBool(Running, true);
        OnClick?.Invoke(this);

        this.OnExit += SelectionManager.Instance.BookerEndsItsPath;
        
        MoveCharacterWithPath(pathMinCost);

        return true;
    }

    private void MoveCharacterWithPath(List<Tile> path)
    {
        if(_pathIndex >= path.Count)
        {
            OnReach?.Invoke(this);
            return;
        }

        if (_pathIndex < path.Count)
        {
            var targetPos = path[_pathIndex].transform.position;
            targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            transform.LookAt(targetPos);
        }
        
        transform.DOMove(path[_pathIndex++].transform.position + Vector3.up, 0.2f).OnComplete(() =>
        {
            MoveCharacterWithPath(path);
        }).SetEase(Ease.Linear);
    }

    
    
}
