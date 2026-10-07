using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform[] arrowPoints;
    [SerializeField] private float spawnInterval = 1f;
    
    [SerializeField] private int maxArrowWaves = 2;
    [SerializeField] private AudioSource ArrowsSFX;

    [SerializeField] private GameObject deathPanel;

    private bool isActive;
    private float timer;
   

    private Queue<List<GameObject>> arrowWaves = new Queue<List<GameObject>>();

    public static ArrowSpawner Instance { get; private set; }

     private void Awake()
     {
        Instance = this;
    }

    /* public void StartTrap()
    {
       isActive = true;
        timer = 0f;

        ArrowsSFX.Play();
     }*/

    public void StartTrap()
    {
        isActive = true;
        timer = 0f;

        Instance = this;

        if (ArrowsSFX != null)
            ArrowsSFX.Play();
    }



    public void StopTrap()
      {
          isActive = false;
          timer = 0f;
        if (ArrowsSFX != null)
        {
            ArrowsSFX.Stop();
        }
    }

      public void StopTrapAndDestroyArrows()
      {

          isActive = false;
          timer = 0f;
        if (ArrowsSFX != null)
            ArrowsSFX.Stop();

        foreach (List<GameObject> wave in arrowWaves)
          {
              foreach (GameObject arrow in wave)
              {
                  if (arrow != null)
                  {
                      Destroy(arrow);
                  }
              }
          }

          arrowWaves.Clear();
      }

      private void Update()
      {
          if (!isActive)
              return;

          timer += Time.deltaTime;

          if (timer >= spawnInterval)
          {
              SpawnArrowWave();
              timer = 0f;
          }
      }
   

    private void SpawnArrowWave()
    {
        List<GameObject> newWave = new List<GameObject>();
      

        foreach (Transform point in arrowPoints)
        {
            if (point == null)
                continue;

            GameObject arrow = Instantiate(
                arrowPrefab,
                point.position,
                point.rotation
            );

            arrow arrowScript = arrow.GetComponent<arrow>();
            if (arrowScript != null)
            {
                arrowScript.deathPanelRef = deathPanel; // Panel adresini oka teslim et
            }

            newWave.Add(arrow);
        }
       
        

        arrowWaves.Enqueue(newWave);

        if (arrowWaves.Count > maxArrowWaves)
        {
            List<GameObject> oldestWave = arrowWaves.Dequeue();

            foreach (GameObject arrow in oldestWave)
            {
                if (arrow != null)
                {
                    Destroy(arrow);
                }
            }
        }
    }
}