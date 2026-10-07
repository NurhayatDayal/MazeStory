using System.Collections;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] private MenuUIController menuUIController;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private AudioSource winSFX;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.Win();
        }
        if (winSFX != null && winSFX.clip != null)
        {
            winSFX.PlayOneShot(winSFX.clip);
        }

        if (winPanel != null)
        {
            StartCoroutine(ShowWinPanelDelayed());
        }

    }
    private IEnumerator ShowWinPanelDelayed()
    {

        yield return new WaitForSeconds(2f);

        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            player.StopPlayer();
        }

        

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }
}
