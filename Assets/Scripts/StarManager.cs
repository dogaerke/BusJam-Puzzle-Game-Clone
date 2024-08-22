using System;
using UnityEngine;
public class StarManager : MonoBehaviour
{
    [SerializeField] private Transform starL;
    [SerializeField] private Transform starM;
    [SerializeField] private Transform starR;

    [SerializeField] private CountDown countDown;

    private void OnEnable()
    {
        HandleStars();
    }


    public void HandleStars()
    {
        switch (countDown.LevelRepeatCount)
        {
            case 0:
                starL.gameObject.SetActive(true);
                starM.gameObject.SetActive(true);
                starR.gameObject.SetActive(true);
                break;
            case 1:
                starL.gameObject.SetActive(true);
                starM.gameObject.SetActive(true);
                starR.gameObject.SetActive(false);
                break;
            default:
                starL.gameObject.SetActive(true);
                starM.gameObject.SetActive(false);
                starR.gameObject.SetActive(false);
                break;
        }
    }
    
}