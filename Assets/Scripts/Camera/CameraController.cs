using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [SerializeField] private List<CameraPropertiesWidth> widthPropertiesList;
    [SerializeField] private List<CameraPropertiesHeight> heightPropertiesList;
    public static CameraController Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        else
            Instance = this;

    }
    void Start()
    {
        //  var position = transform.position;
        //  position.x = GridManager.Instance.width / 2.0f - 0.5f;
        //  transform.position = position;
        //
        // IntegrateProperties();
    
    }

    public void IntegrateProperties()
    {
        var position = transform.position;
        position.x = GridManager.Instance.width / 2.0f - 0.5f;
        transform.position = position;
        
        if(GridManager.Instance.width < 1 || GridManager.Instance.height < 1) return;
        if(GridManager.Instance.width > widthPropertiesList.Count ||
           GridManager.Instance.height > heightPropertiesList.Count) return;
        
        var width = GridManager.Instance.width;
        var transformRotation = transform.eulerAngles;
        transformRotation.x = widthPropertiesList[width - 1].rotationX;
        transform.eulerAngles = transformRotation;
        
        var height = GridManager.Instance.height;
        position = transform.position;
        position.z = heightPropertiesList[height - 1].positionZ;
        transform.position = position;

        
        var camSize = Camera.main.orthographicSize;
        if (widthPropertiesList[width - 1].cameraSize >= heightPropertiesList[height - 1].cameraSize)
        {
            camSize = widthPropertiesList[width - 1].cameraSize;
            Camera.main.orthographicSize = camSize;
        }
        else
        {
            camSize = heightPropertiesList[height - 1].cameraSize;
            Camera.main.orthographicSize = camSize;
        }
           
    }
    

}
