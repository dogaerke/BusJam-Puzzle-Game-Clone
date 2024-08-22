using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PathFinding
{
    [SerializeField] private List<Tile> openList;
    [SerializeField] private List<Tile> closeList;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 10;

    public List<Tile> FindPath(Vector2 startPos, Vector2 endPos)
    {
        var startTile = GetNode(startPos);
        var endTile = GetNode(endPos);
        openList = new List<Tile>();
        closeList = new List<Tile>();
        openList.Add(startTile);
        
        for (var x = 0; x < GridManager.Instance.width; x++)
        {
            for (var y = 0; y < GridManager.Instance.height; y++)
            {
                var tile = GetNode(new Vector2 (x, y));
                tile.gCost = int.MaxValue;
                tile.CalculateFCost();
                tile.parent = null;
            }
        }
        startTile.gCost = 0;
        startTile.hCost = GetDistance(startTile, endTile);
        startTile.CalculateFCost();
        while (openList.Count > 0)
        {
            var currentTile = GetLowestFCostNode(openList);
            if (currentTile.y + 1 == GridManager.Instance.height)
            {
                //Reached final node
                return CalculatePath(currentTile);
            }

            openList.Remove(currentTile);
            closeList.Add(currentTile);

            foreach (var neighbourNode in GetNeighbourList(currentTile))
            {
                if (closeList.Contains(neighbourNode)) continue;
                var tentativeGCost = currentTile.gCost + GetDistance(currentTile, neighbourNode);

                if (tentativeGCost >= neighbourNode.gCost || neighbourNode.hasObstacle) continue;
                neighbourNode.parent = currentTile;
                neighbourNode.gCost = tentativeGCost;
                neighbourNode.hCost = GetDistance(neighbourNode, endTile);
                neighbourNode.CalculateFCost();
                if (!openList.Contains(neighbourNode))
                {
                    openList.Add(neighbourNode);
                }
            }
        }
        //out of nodes
        return null;

    }

    private Tile GetNode(Vector2 pos)
    {
        var tile = GridManager.Instance.Tiles[pos];
        return tile;
    }
    private List<Tile> GetNeighbourList(Tile currentTile)
    {
        var neighbourList = new List<Tile>();
        //Left
        if (currentTile.x - 1 >= 0) neighbourList.Add(GetNode(new Vector2(currentTile.x - 1, currentTile.y)));         
        
        //Right
        if (currentTile.x + 1 < GridManager.Instance.width) neighbourList.Add(GetNode(new Vector2(currentTile.x + 1, currentTile.y)));
        
        //Down
        if (currentTile.y - 1 >= 0) neighbourList.Add(GetNode(new Vector2(currentTile.x, currentTile.y - 1)));
        
        //Up
        if (currentTile.y + 1 < GridManager.Instance.height) neighbourList.Add(GetNode(new Vector2(currentTile.x, currentTile.y + 1)));

        return neighbourList;
    }
    
    private int GetDistance(Tile t1, Tile t2)
    {
        var xDist = Mathf.Abs(t1.x - t2.x);
        var yDist = Mathf.Abs(t1.y - t2.y);

        var remaining = Mathf.Abs(xDist - yDist);

        return Mathf.Min(xDist, yDist) * MOVE_DIAGONAL_COST + remaining * MOVE_STRAIGHT_COST;
    }

    private Tile GetLowestFCostNode(List<Tile> pathList)
    {
        var lowestFCostNode = pathList[0];

        for (var i = 1; i < pathList.Count; i++)
        {
            if (pathList[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = pathList[i];
        }

        return lowestFCostNode;
    }

    private List<Tile> CalculatePath(Tile endNode)
    {
        var path = new List<Tile>();
        path.Add(endNode);
        var currentNode = endNode;
        while (currentNode.parent != null)
        {
            path.Add(currentNode.parent);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
    
}
