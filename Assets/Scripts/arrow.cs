using UnityEngine;

public class arrow : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 5f;
    

    private Rigidbody _rigidbody;
    private bool isBroken = false;

    [HideInInspector] public GameObject deathPanelRef;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        _rigidbody.linearVelocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("OYUNCUYA ÇARPAN OBJE: " + gameObject.name);
            isBroken = true;
            // Oyuncunun üzerindeki bizim hazýrladýðýmýz ana script'i çaðýrýyoruz
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                player.Die(); // Bu fonksiyon zaten karakter kodunuzun içinde Animator'daki "Die" trigger'ýný tetikliyor
              
            }
            GameObject dp = GameObject.Find("Canvas/DeathPanel");

            if (deathPanelRef != null)
            {
                deathPanelRef.SetActive(true);
                Debug.Log("Ölüm paneli baþarýyla açýldý!");
            }
            else
            {
                Debug.LogError("HATA: Ok nesnesine spawner tarafýndan Death Panel referansý iletilemedi!");
            }

            Destroy(gameObject);
            return;
        }


        // if (collision.gameObject.CompareTag("Wall") )
        // {
        //     Destroy(gameObject);
        // }

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("OK DUVARA ÇARPTI: " + collision.gameObject.name);
            Destroy(gameObject);
        }
        Debug.Log("OBJE: " + collision.gameObject.name + " | TAG: " + collision.gameObject.tag);
    }
}
