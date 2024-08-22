using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinAmountText;
    [SerializeField] private int coinAmount;
    
    [SerializeField] private List<short> coinEarned;

    [SerializeField] private bool hasEnoughCoin;
    
    public static CoinManager Instance { get; private set; }

    public bool HasEnoughCoin => hasEnoughCoin;

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    private void Start()
    {
        coinAmount = PlayerPrefs.HasKey("Coin") ? PlayerPrefs.GetInt("Coin") : 0;
        SetCoin();
    }

    private void OnApplicationQuit()
    {
        SetCoin();
    }

    public void IncreaseCoin()
    {
        coinAmount += coinEarned[LevelManager.Instance.CurrentLevel - 1];
        SetCoin();
    }

    public void DecreaseCoin(int amount)
    {
        CheckHasEnoughCoin(amount);
        if(!hasEnoughCoin) return;
        coinAmount -= amount;
        SetCoin();
    }

    public void CheckHasEnoughCoin(int amount)
    {
        hasEnoughCoin = coinAmount - amount >= 0;
    }

    private void SetCoin()
    {
        PlayerPrefs.SetInt("Coin", coinAmount);
        coinAmountText.SetText(coinAmount.ToString());
    }
    
    
}