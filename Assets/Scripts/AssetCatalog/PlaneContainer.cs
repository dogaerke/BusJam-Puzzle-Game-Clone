using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Plane Catalog", menuName = "ScriptableObjects/Catalogs/Plane")]
public class PlaneContainer : ScriptableObject
{
    [SerializedDictionary("Color", "Asset")]
    public SerializedDictionary<GameColors, Vehicle> planeCatalog;
}