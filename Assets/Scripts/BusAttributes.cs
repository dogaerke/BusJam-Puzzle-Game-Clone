using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class BusAttributes
{
    [FormerlySerializedAs("busColor")] public GameColors VehicleColor;
}