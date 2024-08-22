using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSelector : MonoBehaviour
{
    public int CalculateObstacle(Dictionary<Vector2, Tile> tileDict, Tile tile)
    {
        var code = 0;
    
        tileDict.TryGetValue(new Vector2(tile.x, tile.y) + Vector2.left, out var neighbour);

        
        if (!neighbour || neighbour.hasObstacle && !neighbour.isSubway)
            code += 1000;
        
    
        tileDict.TryGetValue(new Vector2(tile.x, tile.y) + Vector2.up, out neighbour);
        if (!neighbour || neighbour.hasObstacle && !neighbour.isSubway)
            code += 100;
        
        
        
        tileDict.TryGetValue(new Vector2(tile.x, tile.y) + Vector2.right, out neighbour);
        if (!neighbour || neighbour.hasObstacle && !neighbour.isSubway)
            code += 10;
        
        tileDict.TryGetValue(new Vector2(tile.x, tile.y) + Vector2.down, out neighbour);
        if (!neighbour || neighbour.hasObstacle && !neighbour.isSubway)
        {
            code += 1;
        }
    
        return code;
    
    }
    
}
