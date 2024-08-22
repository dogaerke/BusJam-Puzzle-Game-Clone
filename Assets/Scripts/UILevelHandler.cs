using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelHandler : MonoBehaviour
{
    [SerializeField] private GameObject levelButtons;

    public void ShowButtons()
    {
        levelButtons.SetActive(true);
    }

    public void HideButtons()
    {
        levelButtons.SetActive(false);
    }
}
