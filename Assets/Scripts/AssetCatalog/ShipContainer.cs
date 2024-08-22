using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship Catalog", menuName = "ScriptableObjects/Catalogs/Ship")]
public class ShipContainer : ScriptableObject
{
    [SerializedDictionary("Color", "Asset")]
    public SerializedDictionary<GameColors, Vehicle> shipCatalog;
}