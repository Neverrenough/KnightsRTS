using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainPanel;
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SwitchStateSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        mainPanel.SetActive(!mainPanel.activeSelf);
    }
}
