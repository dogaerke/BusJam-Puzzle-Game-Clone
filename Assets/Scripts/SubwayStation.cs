using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SubwayStation: MonoBehaviour
{
    [SerializeField] private Transform meshRoot;
    
    [SerializeField]private List<Booker> stationBookerList;
    [SerializeField]private Vector3 direction;//forward backward left right
    [SerializeField] private TMP_Text text;
    public int bookerNumber;

    
    public Vector3 Direction
    {
        get => direction;
        set => direction = value;
    }

    public List<Booker> StationBookerList
    {
        get => stationBookerList;
        set => stationBookerList = value;
    }
    
    public Transform MeshRoot => meshRoot;


    private void Start()
    {
        text.text = stationBookerList.Count.ToString();

        foreach (var b in stationBookerList)
        {
            BookerManager.Instance.allSubwayBookers.Add(b);
        }
    }

    public void MoveFirstBooker()
    {
        stationBookerList[0].gameObject.SetActive(true);
        
        stationBookerList[0].transform.DOMove(transform.position + direction + Vector3.up * 0.9f, 0.2f).OnComplete(() =>
        {
            stationBookerList.Remove(stationBookerList[0]);
            bookerNumber--;
            text.text = stationBookerList.Count.ToString();

        }).SetEase(Ease.Linear);
        
        var targetPos = new Vector2(transform.position.x, transform.position.z);
        var targetDirection = new Vector2(direction.x, direction.z);
        GridManager.Instance.Tiles[targetPos + targetDirection].hasObstacle = true;

    }


    public void CallBooker(Booker booker)
    {
        if(stationBookerList.Count == 0) return;
        
        booker.OnClick -= CallBooker;
        
        stationBookerList[0].gameObject.SetActive(true);
        stationBookerList[0].transform.DOMove(transform.position + direction + Vector3.up * 0.9f, 0.2f).OnComplete(() =>
        {
            stationBookerList.Remove(stationBookerList[0]);
            bookerNumber--;
            text.text = stationBookerList.Count.ToString();
            
        }).SetEase(Ease.Linear);
        
        var targetPos = new Vector2(transform.position.x, transform.position.z);
        var targetDirection = new Vector2(direction.x, direction.z);
        GridManager.Instance.Tiles[targetPos + targetDirection].hasObstacle = true;
    }
    
}
