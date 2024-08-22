using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : UnityEditor.Editor
{
    private string warning;
    private GridManager _myScript;
    
    private void OnEnable()
    {
        _myScript = (GridManager)target;
    }

    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Build Tile"))
        {
            _myScript.GenerateGridsInEditor();
            _myScript.EditorBookersColorsMap = new Dictionary<Vector2, GameColors>();
        }
        if (GUILayout.Button("Clear"))
        {
            _myScript.ClearGrids();
        }

        
        GUI.enabled = _myScript.warnings == "";
        if (GUILayout.Button("Save Data"))
        {
            _myScript.SaveData();
        }
    }

    private void OnSceneGUI()
    {
        if (_myScript.width * _myScript.height != _myScript.CellDataList.Count)//Checks 
        {
            _myScript.CellDataList.Clear();
            _myScript.ClearGrids();
        }

        if (_myScript.VehicleReservationList.Count < _myScript.BookerList.Count / 3)
        {
            _myScript.VehicleColorList.Add(GameColors.DEFAULT);
            _myScript.VehicleReservationList.Add(new ReservationVehicleAttributes(false, 1));
        }
        
        for (int x = 0; x < _myScript.width; x++)
        {
            for (int y = 0; y < _myScript.height; y++)
            {
                Vector3 worldPos = new Vector3(-0.5f + x, 0, 0.5f + y);
                
                // Convert world position to GUI position
                Vector3 basePosition = HandleUtility.WorldToGUIPoint(worldPos);
                Vector3 xPosition = HandleUtility.WorldToGUIPoint(worldPos + new Vector3(1f, 0f, 0f));
                Vector3 yPosition = HandleUtility.WorldToGUIPoint(worldPos + new Vector3(1f, 0f, 1f));
                
                float xDiff = Mathf.Abs(xPosition.x - basePosition.x);
                float yDiff = Mathf.Abs(yPosition.y - basePosition.y);
                
                Rect rect = new Rect(basePosition.x, basePosition.y, xDiff, yDiff);

                // Begin GUI block
                Handles.BeginGUI();
                GUI.color = Color.white;
                GUI.Box(rect, $"({x}, {y})");
                GUI.color = Color.white;

                // Begin Char Selection
                GUILayout.BeginArea(rect);
                GUILayout.BeginVertical();
                TileSelection(worldPos, x, y);
                GUILayout.EndVertical();
                GUILayout.EndArea();

                // End GUI block
                Handles.EndGUI();
            }
        }

        //Clears Messages
        _myScript.warnings = "";
        warning = "";
        
        VehicleCountChecker();
        BookerChecker();
        SubwayChecker();
        
        //Output Message
        _myScript.warnings = warning;
    }

    //Checks Subway Direction
    private void SubwayChecker()
    {
        var existingDirectionsDict = new Dictionary<Vector2, CellData>();
        foreach (var cellData in _myScript.CellDataList)
        {
            existingDirectionsDict.Add(new Vector2(cellData.x, cellData.y), cellData);
        }
        
        foreach (var cd in _myScript.CellDataList)
        {
            if(!cd.isSubway) continue;
            
            if(cd.subwayDirection == Directions.None)
            {
                warning += $"Grid {cd.x}, {cd.y} cannot be none!!!\n";
                continue;
            }
            
            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                if (direction == Directions.None) continue;
                
                if(!existingDirectionsDict.ContainsKey(new Vector2(cd.x, cd.y) + cd.subwayDirection.Set2DDirection()))
                {
                    warning += $"Direction of {cd.x}, {cd.y} cannot be {cd.subwayDirection}!!!\n";
                    break; 
                }

            }
        }
        
        
    }

    private void BookerChecker()
    {
        var bookerCountDict = new Dictionary<GameColors, int>();
        foreach (var cd in _myScript.CellDataList)
        {
            bookerCountDict.TryAdd(cd.gameColors, 0);
            var count = bookerCountDict[cd.gameColors];
            bookerCountDict[cd.gameColors] = ++count;
            
            if(!cd.isSubway || cd.bookersInSubway == null) continue;
            
            foreach (var subwayBooker in cd.bookersInSubway)
            {
                bookerCountDict.TryAdd(subwayBooker, 0);
                count = bookerCountDict[subwayBooker];
                bookerCountDict[subwayBooker] = ++count;
            }
        }

        foreach (GameColors color in Enum.GetValues(typeof(GameColors)))
        {
            if(color == GameColors.DEFAULT) continue;
            
            var bookerCount = bookerCountDict.GetValueOrDefault(color, 0);
            if (bookerCount % 3 == 0) continue;

            var remains = 3 - bookerCount % 3;
            if(remains < 0) continue;
            
            warning += $"{remains + " " + color} Character Needed!!!\n";
        }
    }


    private void VehicleCountChecker()
    {
        var bookerCountDict = new Dictionary<GameColors, int>();
        var vehicleCountDict = new Dictionary<GameColors, int>();
            
        //Counts Booker Colors
        foreach (var cd in _myScript.CellDataList)
        {
            
            bookerCountDict.TryAdd(cd.gameColors, 0);
                    
            var count = bookerCountDict[cd.gameColors];
            bookerCountDict[cd.gameColors] = ++count;

            if (!cd.isSubway || cd.bookersInSubway == null) continue;
            
            foreach (var booker in cd.bookersInSubway)
            {
                bookerCountDict.TryAdd(booker, 0);
                count = bookerCountDict[booker];
                bookerCountDict[booker] = ++count;
            }
        }
            
        //Counts Vehicle Colors
        foreach (var cd in _myScript.VehicleColorList)
        {
            vehicleCountDict.TryAdd(cd, 0);
            var count = vehicleCountDict[cd];
            vehicleCountDict[cd] = ++count;
        }
        
            
        //Check 
        foreach (GameColors color in Enum.GetValues(typeof(GameColors)))
        {
            if(color == GameColors.DEFAULT) continue;
            if(!bookerCountDict.TryGetValue(color, out var countBooker)) continue;

            var countVehicle = vehicleCountDict.GetValueOrDefault(color, 0);
                        
            if(countBooker / 3 == countVehicle) continue;
            if (countBooker / 3 < countVehicle)
            {
                warning += $"{color} Vehicle needless!!!\n";
            }

            var remains = countBooker / 3 - countVehicle;
            if(remains < 0) continue;

            
            warning += $"{remains + " " + color} Vehicle needed!!!\n";
        }

        foreach (var color in _myScript.VehicleColorList)
        {
            if(color == GameColors.DEFAULT) continue;

            if (!bookerCountDict.ContainsKey(color))
            {
                warning += $"Characters doesn't contains {color} !!!\n";
            }
        }
        
    }

    private void TileSelection(Vector3 worldPos, int x, int y)
    {
        if(_myScript.CellDataList.Any(c => c.x == x && c.y == y))
        {
            var cd = _myScript.CellDataList.Find(c => c.x == x && c.y == y);
            cd.hasObstacle = EditorGUILayout.ToggleLeft("Has Obstacle", cd.hasObstacle);
            cd.isSubway = EditorGUILayout.ToggleLeft("Is Subway", cd.isSubway);


            if (!cd.hasObstacle && !cd.isSubway)
            {
                CreateBooker(worldPos, cd);
            }

            SubwaySelection(worldPos, cd);
              
        }
        else
        {
            var cd = new CellData(worldPos, x, y, GameColors.DEFAULT);
            _myScript.CellDataList.Add(cd);
            
            cd.hasObstacle = EditorGUILayout.ToggleLeft("Has Obstacle", cd.hasObstacle);
            
            if (cd.hasObstacle) return;
            
            CreateBooker(worldPos, cd);
        }
        
    }

    private void SubwaySelection(Vector3 worldPos, CellData cd)
    {
        
        if (cd.isSubway)
        {
            cd.subwayDirection = (Directions)EditorGUILayout.EnumPopup("Direction: ", cd.subwayDirection);

            if (cd.bookersInSubway.Count > cd.reservedSubwayBookers.Count)
            {
                cd.reservedSubwayBookers.Add(false);
            }
            else if (cd.bookersInSubway.Count < cd.reservedSubwayBookers.Count)
            {
                cd.reservedSubwayBookers.Clear();
            }
            
            if(cd.subwayStation) return;
            
            var createPos = new Vector3(worldPos.x - .5f,0, worldPos.z - .5f);
            createPos += Vector3.up + Vector3.right;
            
            var station = (SubwayStation)PrefabUtility.InstantiatePrefab(_myScript.subwayPrefab);
            
            _myScript.subwayStations.Add(station);
            station.Direction = cd.subwayDirection.Set3DDirection();
                
            station.transform.position = createPos;
            cd.subwayStation = station;
        }
        else
        {
            if(cd.subwayStation)
                DestroyImmediate(cd.subwayStation.gameObject);

            cd.subwayStation = null;
        }
    }

    private void CreateBooker(Vector3 worldPos, CellData cd)
    {
        GameColors oldGameColor = cd.gameColors;
        var newGameColor = (GameColors)EditorGUILayout.EnumPopup("Color: ", cd.gameColors);
        
        if(newGameColor == GameColors.DEFAULT)
        {
            var b = _myScript.BookerList.Find(b =>
                b.transform.position.x == cd.x && b.transform.position.z == cd.y);
            if (b)
            {
                _myScript.BookerList.Remove(b);
                DestroyImmediate(b.gameObject);
            }

            cd.gameColors = GameColors.DEFAULT;
            return;
        }
        
        cd.isReserved = EditorGUILayout.ToggleLeft("Is Reserved", cd.isReserved);
        cd.isSecret = EditorGUILayout.ToggleLeft("Is Secret", cd.isSecret);

        var createPos = new Vector3(worldPos.x - .5f,0, worldPos.z - .5f);
        createPos += Vector3.up + Vector3.right;
        
        if (oldGameColor != newGameColor)
        {
            var existBooker = _myScript.BookerList.Find(b =>
                b.transform.position.x == cd.x && b.transform.position.z == cd.y);

            if (existBooker)
            {
                _myScript.BookerList.Remove(existBooker);
                var newBooker = (Booker)PrefabUtility.InstantiatePrefab(_myScript.bookerContainer.bookerCatalog[newGameColor], 
                    _myScript.createdCharacters);
                
                _myScript.BookerList.Add(newBooker);

                newBooker.transform.position = existBooker.transform.position;
                DestroyImmediate(existBooker.gameObject);
                
                // If there's already a Booker, change its gameColors value
                newBooker.Attributes.bookerColor = newGameColor;
                existBooker.IsSecret = cd.isSecret;
            }
            else
            {
                //Instantiate booker in editor.
                var booker = (Booker)PrefabUtility.InstantiatePrefab(_myScript.bookerContainer.bookerCatalog[newGameColor], 
                    _myScript.createdCharacters);
                booker.transform.position = createPos;
                booker.Attributes.bookerColor = newGameColor;
                
                _myScript.BookerList.Add(booker);
                
                booker.IsSecret = cd.isSecret;
            }
            
        }

        cd.gameColors = newGameColor;
        
        
    }
    
}
