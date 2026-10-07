using UnityEngine;
using System.Collections;

public class FlickLight : MonoBehaviour
{
    [Tooltip("Bu nesnedeki veya altındaki Light bileşeni")]
    public Light lig;

    [Header("Titreme Ayarları")]
    [Tooltip("Işığın düşebileceği en az parlaklık")]
    public float minIntensity = 1f;
    [Tooltip("Işığın çıkabileceği en yüksek parlaklık")]
    public float maxIntensity = 2.0f;

    [Range(1, 50)]
    [Tooltip("Yumuşatma miktarı. Büyük sayı geçişleri daha da yumuşatır.")]
    public int flickerSmoothness = 5;

    [Header("Hız Ayarı")]
    [Tooltip("Işığın kaç saniyede bir yeni bir değere geçeceği. (Büyütürseniz titreme yavaşlar. Örn: 0.05 veya 0.1)")]
    public float updateInterval = 0.01f;

    private System.Collections.Generic.Queue<float> smoothQueue;
    private float lastSum = 0;

    void Start()
    {
        if (lig == null)
        {
            lig = GetComponent<Light>();
        }

        smoothQueue = new System.Collections.Generic.Queue<float>();

        if (lig != null)
        {
            lig.intensity = (minIntensity + maxIntensity) / 2f;
        }

        // Titreme döngüsünü zamanlayıcı (Coroutine) ile başlatıyoruz ki hıza müdahale edebilelim
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            if (lig != null)
            {
                // Beğendiğiniz orijinal sıra (Queue) algoritması
                while (smoothQueue.Count >= flickerSmoothness)
                {
                    lastSum -= smoothQueue.Dequeue();
                }

                float newVal = Random.Range(minIntensity, maxIntensity);
                smoothQueue.Enqueue(newVal);
                lastSum += newVal;

                lig.intensity = lastSum / smoothQueue.Count;
            }

            // Belirlediğiniz süre kadar bekler (Hızı burası kontrol eder)
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
