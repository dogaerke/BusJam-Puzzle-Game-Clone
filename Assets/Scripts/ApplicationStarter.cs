using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class ApplicationStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DOTween.SetTweensCapacity(100, 10);
        Application.targetFrameRate = 60;
    }
    
}
