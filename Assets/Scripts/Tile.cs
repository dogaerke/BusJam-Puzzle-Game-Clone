using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public Tile parent;
    public Renderer tileRenderer;
    public bool hasObstacle;
    public bool isSubway;
    public int gCost;
    public int hCost;
    public int fCost;
    
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}
