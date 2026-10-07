using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject winPanel;

    private void Start()
    {
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        deathPanel.SetActive(true);
    }

    public void ShowWinScreen()
    {
        winPanel.SetActive(true);
        Debug.Log("DEATH SCREEN ÇALIÞTI!");
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
