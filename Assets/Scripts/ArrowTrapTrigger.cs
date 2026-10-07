using System.Collections.Generic;
using UnityEngine;

public class ArrowTrapTrigger : MonoBehaviour
{
    [SerializeField] private ArrowSpawner arrowSpawner;
    [SerializeField] private AudioSource trapSFX;
    

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        trapSFX.Play();
       
        arrowSpawner.StartTrap();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        arrowSpawner.StopTrap();
    }
}
