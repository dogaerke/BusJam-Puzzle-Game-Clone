using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CellData
{
        public int x, y;
        public Vector3 vectorPos;
        public bool hasObstacle;
        public GameColors gameColors;
        public bool isReserved;
        public bool isSecret;
        public bool isSubway;
        public Directions subwayDirection;
        public SubwayStation subwayStation;
        public List<GameColors> bookersInSubway;
        public List<bool> reservedSubwayBookers;
        
        public CellData(Vector3 newVec, int x, int y, GameColors newColors)
        {
                this.x = x;
                this.y = y;
                vectorPos = newVec; 
                gameColors = newColors;
        }
}

public enum Directions
{
        None = 0,
        Forward = 1,
        Backward = 2, 
        Left = 3,
        Right = 4,
}

public static class Dir
{
        public static Vector3 Set3DDirection(this Directions e)
        {
                return e switch
                {
                        Directions.Forward => Vector3.forward,
                        Directions.Backward => Vector3.back,
                        Directions.Left => Vector3.left,
                        Directions.Right => Vector3.right,
                        _ => Vector3.one * -1
                };
        }

        public static Vector2 Set2DDirection(this Directions d)
        {
                return d switch
                {
                        Directions.Forward => Vector2.up,
                        Directions.Backward => Vector2.down,
                        Directions.Left => Vector2.left,
                        Directions.Right => Vector2.right,
                        _ => Vector2.one * -1
                };
        }

        public static Vector2 Convert3DTo2D(this Vector3 v3)
        {
                return new Vector2(v3.x, v3.z);
        }
}


