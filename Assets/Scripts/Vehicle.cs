using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class Vehicle : MonoBehaviour
{
    [SerializeField] private BusAttributes busAttributes;
    [SerializeField] private Transform vehicleDoor;
    [SerializeField] private List<GameObject> reservationFlags;

    [SerializeField] private GameObject planeWing;
    [SerializeField] private GameObject planeMotor;
    
    [Header("Booker Part")]
    //[SerializeField] private short bookerCount = 1;
    [SerializeField] public List<Booker> bookers;
    [SerializeField] private List<GameObject> bookersSittingInVehicle;
    [SerializeField] private List<GameObject> bookerSittingResHats;
    [SerializeField] public Renderer busRenderer;
    [SerializeField] private Animator doorAnim;
    
    public bool isReserveRequired;
    public bool reservationArrive;
    [Range(1,3)] public short reservationInfo = 1; //Added reservation info
    public int noReservationCount = 0;
    public int reservationCount = 0;
    public int bookerCount = 0;
    
    private Transform stopPoint;
    private Transform exitPoint;
    private static readonly int DoorOpen = Animator.StringToHash("doorOpen");

    public event Action<Vehicle> OnReach;
    public event Action<Vehicle> OnExit;
    
    public event Action<Vehicle> OnEnd;


    #region Getters&Setters
    public Transform StopPoint
    {
        get => stopPoint;
        set => stopPoint = value;
    }

    public Transform ExitPoint
    {
        get => exitPoint;
        set => exitPoint = value;
    }

    public BusAttributes Attributes
    {
        get => busAttributes;
        set => busAttributes = value;
    }

    public Animator DoorAnim
    {
        get => doorAnim;
        set => doorAnim = value;
    }

    public List<GameObject> BookersSittingInVehicle
    {
        get => bookersSittingInVehicle;
        set => bookersSittingInVehicle = value;
    }

    public Transform VehicleDoor => vehicleDoor;

    public List<GameObject> BookerSittingResHats
    {
        get => bookerSittingResHats;
        set => bookerSittingResHats = value;
    }

    public List<GameObject> ReservationFlags
    {
        get => reservationFlags;
        set => reservationFlags = value;
    }

    #endregion

    public void MoveVehicleToLine()
    {
        transform.DOMove(stopPoint.position, 1f).OnComplete(() =>
        {
            OnReach?.Invoke(this);
            if (LevelManager.Instance.CurrentLevel >= 30 && LevelManager.Instance.CurrentLevel < 40)
            {
                planeMotor.SetActive(false);
                planeWing.SetActive(false);
            }
            
        }).SetEase(Ease.OutQuart);
    }

    public void HandleExit()
    {
        if (bookerCount != 3)
        {
            return;
        }

        foreach (var booker in bookers)
        {
            booker.gameObject.SetActive(false);
        }

        if (LevelManager.Instance.CurrentLevel >= 30 && LevelManager.Instance.CurrentLevel < 40)
        {
            planeMotor.SetActive(true);
            planeWing.SetActive(true);
        }
        transform.DOMove(exitPoint.position, .5f).OnComplete(() =>
        {
            OnExit?.Invoke(this);
            OnEnd?.Invoke(this);
        }).SetEase(Ease.InQuart);
        
    }
    
}

