using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHidePanel : MonoBehaviour
{
    public GameObject panel;        // The panel to show/hide
    public GameObject showButton;   // The button to show the panel
    public GameObject hideButton;   // The button to hide the panel

    // Method to show the panel
    public void ShowPanel()
    {
        panel.SetActive(true);
        showButton.SetActive(false);
        hideButton.SetActive(true);
    }

    // Method to hide the panel
    public void HidePanel()
    {
        panel.SetActive(false);
        showButton.SetActive(true);
        hideButton.SetActive(false);
    }
}
