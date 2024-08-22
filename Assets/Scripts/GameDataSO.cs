using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameData", order = 1)]
[System.Serializable]
public class GameDataSO : ScriptableObject
{
    public List<GameColors> busColorList;// DO NOT CHANGE NAME!!
    public List<ReservationVehicleAttributes> vehicleReservationList;

    //public Dictionary<Vector2, GameColors> bookerPositions;
    public List<CellData> charactersColorMap;

    [Header("Grid Width & Height")] 
    public int width;
    public int height;

}

