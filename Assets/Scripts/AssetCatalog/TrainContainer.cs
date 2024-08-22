using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Train Catalog", menuName = "ScriptableObjects/Catalogs/Train")]
public class TrainContainer : ScriptableObject
{
    [SerializedDictionary("Color", "Asset")]
    public SerializedDictionary<GameColors, Vehicle> trainCatalog;
}