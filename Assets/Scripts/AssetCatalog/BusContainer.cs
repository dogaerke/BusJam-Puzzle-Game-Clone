using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Bus Catalog", menuName = "ScriptableObjects/Catalogs/Bus")]
public class BusContainer : ScriptableObject
{
    [FormerlySerializedAs("vehicleCatalog")] [SerializedDictionary("Color", "Asset")]
    public SerializedDictionary<GameColors, Vehicle> busCatalog;
}