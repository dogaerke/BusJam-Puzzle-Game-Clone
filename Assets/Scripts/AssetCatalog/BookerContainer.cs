using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Booker Catalog", menuName = "ScriptableObjects/Catalogs/Booker")]
public class BookerContainer : ScriptableObject
{
    [SerializedDictionary("Color", "Asset")]
    public SerializedDictionary<GameColors, Booker> bookerCatalog;
}
