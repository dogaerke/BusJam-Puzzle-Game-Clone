using UnityEngine;

[CreateAssetMenu(fileName = "Material Catalog", menuName = "ScriptableObjects/Catalogs/Material_Catalog")]
public class ObstacleMatCatalog : ScriptableObject
{
    public Material circle;
    
    [Header("Half Circle")]
    public Material halfCircle;
    public Material halfCircle90;
    public Material halfCircle180;
    public Material halfCircle270;
    
    [Header("Square")]
    public Material square;    
    public Material square90;
    public Material fullSquare;

    [Header("3 End Square")] 
    public Material threeEnd;
    public Material threeEnd90;
    public Material threeEnd180;
    public Material threeEnd270;
    
    [Header("Curved")]
    public Material curved;
    public Material curved90;
    public Material curved180;
    public Material curved270;

    public Material ObstacleSelector(int code)
    {
        switch (code)
        {
            case 0000: //Circle each direction has not obstacle
                return circle;
            
            //Half Circle ////////////////////////////////
            case 0001:
                return halfCircle;
            
            case 0010:
                return halfCircle270;
            
            case 0100:
                return halfCircle180;
            
            case 1000:
                return halfCircle90;
            
            
            //////////////////Square
            case 0101:
                return square;
            
            case 1010:
                return square90;
            
            
            ////////////////Curved
            case 0011:
                return curved;
            
            case 0110:
                return curved270;
            
            case 1100:
                return curved180;
            
            case 1001:
                return curved90;
            
            
            /////////////////3 End Squares
            case 0111:
                return threeEnd;
            case 1110:
                return threeEnd270;
            case 1101:
                return threeEnd180;
            case 1011:
                return threeEnd90;
            
            
            /////////////////Full Square
            case 1111:
                return fullSquare;
        }

        return null;
    }
    
}