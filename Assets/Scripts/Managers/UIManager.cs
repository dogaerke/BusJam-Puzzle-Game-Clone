using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private GameObject levelButtons;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject levelLossButtons;
    
    [SerializeField] private Button skipButton;
    [SerializeField] private Button spendHeartButton;
    
    [SerializeField] private Button nextLevelButton;
    

    [SerializeField] private AudioSource audioSource;
    
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void GameEndsWithLose()
    {
        
        EnableButtons();

        levelLossButtons.SetActive(true);
        
        DisableButtonsIfNeeded();
    }

    private void DisableButtonsIfNeeded()
    {
        CoinManager.Instance.CheckHasEnoughCoin(50);
        if (!CoinManager.Instance.HasEnoughCoin) skipButton.enabled = false;
        if (!CountDown.Instance.HasEnoughHeart()) spendHeartButton.enabled = false;
    }

    private void EnableButtons()
    {
        skipButton.enabled = true;
        spendHeartButton.enabled = true;
    }

    public void GameEndsWithWin()
    {
        audioSource.Play();
        levelButtons.SetActive(true);
        nextLevelButton.enabled = true;
    }

    public void OpenSettings()
    {
        settingsPanel.gameObject.SetActive(true);
    }

    public void DisableAfterClick(GameObject go)
    {
        go.SetActive(false);
    }

    public void EnableButton(Button button)
    {
        button.enabled = true;
    }
    public void DisableButton(Button button)
    {
        button.enabled = false;
    }

}
