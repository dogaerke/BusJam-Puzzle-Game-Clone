using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CountDown : MonoBehaviour
{
    [SerializeField] private int defaultHeartCount = 5;
    [SerializeField] private float defaultSeconds = 30 * 60;
    
    
    [SerializeField] private TMP_Text heartLeftText;
    [SerializeField] private int heartLeft;
    
    [SerializeField] private TMP_Text healthCount;

    [SerializeField] private int levelRepeatCount = 0;
    [SerializeField] private float _secondsPassedInGame;
    private DateTime _savedTime;

    public int HeartLeft => heartLeft;

    public int LevelRepeatCount
    {
        get => levelRepeatCount;
        set => levelRepeatCount = value;
    }

    public static CountDown Instance { get; private set; }

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
    void OnApplicationQuit()
    {
        // Convert the current time to a long, then to a string, and save it
        string savedTime = DateTime.Now.ToBinary().ToString();
        PlayerPrefs.SetString("SavedTime", savedTime);
        PlayerPrefs.SetInt("Heart", heartLeft);
        PlayerPrefs.SetFloat("SecondsPassed", _secondsPassedInGame);
    }

    void Start()
    {
        SetHeartCount();

        SetSeconds();
        
        StartCoroutine(CountTime());
    }

    private void SetHeartCount()
    {
        heartLeft = PlayerPrefs.HasKey("Heart") ? PlayerPrefs.GetInt("Heart") : defaultHeartCount;
        heartLeftText.SetText(heartLeft.ToString());
    }

    private void SetSeconds()
    {
        if (!PlayerPrefs.HasKey("SecondsPassed"))
        {
            _secondsPassedInGame = defaultSeconds;
            DisplayTime();
        }
        else
        {
            if(heartLeft != defaultHeartCount)
                _secondsPassedInGame = PlayerPrefs.GetFloat("SecondsPassed");
            else
            {
                _secondsPassedInGame = defaultSeconds;
            }
        }

        if (heartLeft >= 5) return;
        if (!PlayerPrefs.HasKey("SavedTime")) return;
        
        // Get the saved time string, convert it to a long, then to a DateTime
        string savedTime = PlayerPrefs.GetString("SavedTime");
        long savedTimeBinary = Convert.ToInt64(savedTime);
        DateTime lastTime = DateTime.FromBinary(savedTimeBinary);

        // Calculate the difference between the current time and the saved time
        TimeSpan timePassedSpan = DateTime.Now - lastTime;
        
        var timePassed = (int) timePassedSpan.TotalSeconds;
        var diff = timePassed - _secondsPassedInGame;
        
        if (diff > 0)
        {
            heartLeft = defaultHeartCount;
        }
        else
        {
            _secondsPassedInGame = Mathf.Abs(diff);
        }
    }

    private void Update()
    {
        if(heartLeft == 5) return;
        _secondsPassedInGame -= Time.deltaTime;
        
    }

    private IEnumerator CountTime()
    {
        while (true)
        {
            if (heartLeft < 5)
            {
                DisplayTime();

                if (_secondsPassedInGame <= 0)
                {
                    heartLeft = 5;
                    heartLeftText.SetText(heartLeft.ToString());
                    _secondsPassedInGame = defaultSeconds;
                }
            }
            yield return new WaitForSeconds(1);
        }

    }

    private void DisplayTime()
    {
        var minutes = (int)_secondsPassedInGame / 60;
        var seconds = (int)_secondsPassedInGame % 60;
        var displayTime = $"{minutes:00}:{seconds:00}";
        healthCount.text = displayTime;
    }

    public void DecreaseHeart()
    {
        heartLeft--;
        levelRepeatCount++;
        heartLeftText.SetText(heartLeft.ToString());
    }

    public void ResetValues()
    {
        LevelRepeatCount = 0;
    }

    public bool HasEnoughHeart()
    {
        return heartLeft > 0;
    }
}
