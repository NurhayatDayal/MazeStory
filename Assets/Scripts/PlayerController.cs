using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject cinemachineCamera;
    

    [SerializeField] private Transform startPoint;
    [SerializeField] private GameObject deathPanel;

    [SerializeField] private CapsuleCollider playerCollider;

    private Vector3 normalColliderCenter;
    private float normalColliderHeight;
    private float normalColliderRadius;


    private bool hasWon = false;
    private bool playerStopped = false;

    [SerializeField] private GameObject menuCanvas;

    

    [Header("Hareket Ayarlarý")]
    [SerializeField] public float moveSpeed = 5f;       // Normal yürüme hýzý
    [SerializeField] public float sprintSpeed = 8f;     // Shift tuþuna basýnca ulaþýlan koþma hýzý
    [SerializeField] public float rotationSpeed = 10f;  // Karakterin kameraya doðru dönme hýzý
    [SerializeField] public float crouchSpeed = 2f;

    [Header("Zýplama Ayarlarý")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform groundCheckPoint; // Karakterin ayak ucundaki boþ nesne
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;      // Zemin katmaný (Örn: Ground katmaný)

    [Header("Bileþenler")]
    [SerializeField] private Transform cameraTransform; // Kameranýn transform bileþeni
    [SerializeField] private AudioSource hitByArrowSFX;
    [SerializeField] private AudioSource WalkingSFX;
    [SerializeField] private AudioSource RunningSFX;
    [SerializeField] private AudioSource JumpSFX;
    [SerializeField] private AudioSource BackgroundMusic;
    [SerializeField] private AudioSource WinSFX;

    private Rigidbody _rigidbody;
    private Animator _animator;
    public bool isDead = false;
    private bool isGrounded;

    /*  private void Awake()
      {
          _rigidbody = GetComponent<Rigidbody>();
          _animator = GetComponent<Animator>();


      }*/
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        if (playerCollider != null)
        {
            normalColliderHeight = playerCollider.height;
            normalColliderRadius = playerCollider.radius;
            normalColliderCenter = playerCollider.center;
        }
    }


    private void Start()
    {
        // ÖNEMLÝ: Oyun baþlar baþlamaz karakteri kilitliyoruz.
        // isDead true olduðu için FixedUpdate içindeki hareket kodlarý asla çalýþmayacaktýr.
        isDead = true;

        // Karakterin ilk karede durduðundan emin olmak için hýzýný sýfýrlýyoruz
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }
        if (cinemachineCamera != null)
        {
            cinemachineCamera.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false); // Resim, panel ve buton dahil tüm Canvas'ý tek seferde kapatýr
        }

        isDead = false; // Karakterin hareket kilidini açar, artýk merdivenlerden inebilir!

        // Eðer SoundManager kodunu eklediyseniz müzik için altýna þu satýrý da ekleyebilirsiniz:
        // SoundManager.Instance.StartBackgroundSounds(); 

        if (cinemachineCamera != null)
        {
            cinemachineCamera.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        // Eðer karakter öldüyse hiçbir hareket veya tuþ girdisini iþleme
        if (isDead) return;

        if (playerStopped)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        if (hasWon)
        {
            AutoWalk();
            return;
        }

        GroundCheck();
        MoveCharacter();

       
    }

  
    // Karakterin yerde olup olmadýðýný ayak ucundaki küçük bir görünmez küre ile kontrol eder
    private void GroundCheck()
    {
        if (groundCheckPoint != null)
        {
            isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = true;
        }
    }

    private void MoveCharacter()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // 1. Klavyeden Yön Girdilerini Alýyoruz
        if (Keyboard.current.aKey.isPressed) horizontal = -1f;
        if (Keyboard.current.dKey.isPressed) horizontal = 1f;
        if (Keyboard.current.wKey.isPressed) vertical = 1f;
        if (Keyboard.current.sKey.isPressed) vertical = -1f;

        // 2. Durum Kontrolleri (Koþma ve Eðilme)
        // Sadece ileri doðru (w tuþuyla) giderken koþmaya izin veriyoruz
        
        bool isCrouching = Keyboard.current.leftCtrlKey.isPressed; // Sol CTRL tuþu eðilmeyi saðlar
        if (playerCollider != null)
        {
            if (isCrouching)
            {
                playerCollider.height = normalColliderHeight * 0.5f;

                playerCollider.center = new Vector3(
                    normalColliderCenter.x,
                    normalColliderCenter.y - normalColliderHeight * 0.25f,
                    normalColliderCenter.z
                );
            }
            else
            {
                playerCollider.height = normalColliderHeight;
                playerCollider.radius = normalColliderRadius;
                playerCollider.center = normalColliderCenter;
            }
        }
        bool isSprinting = Keyboard.current.leftShiftKey.isPressed && (horizontal != 0 || vertical != 0) && !isCrouching;

        // 3. Geçerli Hýzý Belirliyoruz
        // Eðilme hýzý animasyonla çözüleceði için eðilirken de normal moveSpeed geçerlidir
        float currentSpeed = moveSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed; // Eðildiðinde karakter bu hýza düþer (Editörden elle ayarladýðýnýz deðer)
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed; // Koþtuðunda karakter bu hýza çýkar (Editörden elle ayarladýðýnýz deðer)
        }


        // 4. Zýplama Mantýðý
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, jumpForce, _rigidbody.linearVelocity.z);
            if (JumpSFX != null && JumpSFX.clip != null)
            {
                JumpSFX.PlayOneShot(JumpSFX.clip);
            }
            if (_animator != null)
                _animator.SetTrigger("Jump"); // Animator'daki Jump trigger'ýný çalýþtýrýr
        }

        // Kameranýn ileri ve sað yön vektörlerini alýyoruz (Y eksenini sýfýrlayarak eðilmeleri önlüyoruz)
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        // Karakterin her zaman kameranýn baktýðý yöne dönmesini saðlýyoruz
        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(smoothRotation);

        // Hareket yönünü kameranýn yönlerine göre hesaplýyoruz
        Vector3 forwardMovement = camForward * vertical;
        Vector3 rightMovement = camRight * horizontal;
        Vector3 moveDir = forwardMovement + rightMovement;

        // Rigidbody üzerinden fiziksel hýzý uyguluyoruz
        Vector3 velocity = moveDir.normalized * currentSpeed;
        velocity.y = _rigidbody.linearVelocity.y; // Mevcut yerçekimi düþüþ hýzýný koruyoruz
        _rigidbody.linearVelocity = velocity;

        // 5. Animator Parametrelerini Gönderme
        if (_animator != null)
        {
            if (isSprinting)
            {
                // Karakter koþuyorsa, Blend Tree'de koþma animasyonuna geçmesi için deðerleri 2 ile çarpýp gönderiyoruz
                _animator.SetFloat("MoveX", horizontal * 2f, 0.15f, Time.fixedDeltaTime);
                _animator.SetFloat("MoveZ", vertical * 2f, 0.15f, Time.fixedDeltaTime);
               
            }
            else
            {
                // Karakter normal yürüyorsa klavyeden gelen orijinal deðerleri (-1, 0, 1) gönderiyoruz
                _animator.SetFloat("MoveX", horizontal, 0.15f, Time.fixedDeltaTime);
                _animator.SetFloat("MoveZ", vertical, 0.15f, Time.fixedDeltaTime);
               
            }

            // Animator içindeki bool durum parametrelerini güncelliyoruz
            _animator.SetBool("IsGrounded", isGrounded);
            _animator.SetBool("IsCrouching", isCrouching); // Eðilme Blend Tree geçiþini tetikler
        }
        // Movement sound
        bool isMoving = moveDir.magnitude > 0.1f && isGrounded;

        if (isMoving && !isCrouching)
        {
            if (isSprinting)
            {
                // Stop walking sound
                if (WalkingSFX != null && WalkingSFX.isPlaying)
                    WalkingSFX.Stop();

                // Play running sound
                if (RunningSFX != null && !RunningSFX.isPlaying)
                    RunningSFX.Play();
            }
            else
            {
                // Stop running sound
                if (RunningSFX != null && RunningSFX.isPlaying)
                    RunningSFX.Stop();

                // Play walking sound
                if (WalkingSFX != null && !WalkingSFX.isPlaying)
                    WalkingSFX.Play();
            }
        }
        else
        {
            // Stop movement sounds
            if (WalkingSFX != null && WalkingSFX.isPlaying)
                WalkingSFX.Stop();

            if (RunningSFX != null && RunningSFX.isPlaying)
                RunningSFX.Stop();
        }
    }

    private void AutoWalk()
    {
        Vector3 forwardDirection = Vector3.forward;

        Vector3 velocity = forwardDirection * moveSpeed;

        _rigidbody.linearVelocity = velocity;

        if (_animator != null)
        {
            _animator.SetFloat("MoveX", 0f, 0.15f, Time.fixedDeltaTime);
            _animator.SetFloat("MoveZ", 1f, 0.15f, Time.fixedDeltaTime);
        }

        if (WalkingSFX != null && !WalkingSFX.isPlaying)
        {
            WalkingSFX.Play();
        }

        if (RunningSFX != null && RunningSFX.isPlaying)
        {
            RunningSFX.Stop();
        }
    }

    public void Win()
    {
        if (isDead || hasWon) return;

        hasWon = true;

        // Oyuncunun normal hareket sesini durdur
        if (RunningSFX != null)
        {
            RunningSFX.Stop();
        }

        Debug.Log("OYUN KAZANILDI!");
    }

    public void StopPlayer()
    {
        playerStopped = true;

        _rigidbody.linearVelocity = Vector3.zero;

        if (WalkingSFX != null)
            WalkingSFX.Stop();

        if (RunningSFX != null)
            RunningSFX.Stop();
    }

    // Karaktere ok çarptýðýnda veya tuzak tetiklendiðinde dýþarýdan çaðrýlacak ölüm fonksiyonu
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        if (cinemachineCamera != null)
        {
            cinemachineCamera.SetActive(false);
        }


        // Karakterin hareketini durdur
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        if (WalkingSFX != null)
        {
            WalkingSFX.Stop();
        }

        if (RunningSFX != null)
        {
            RunningSFX.Stop();
        }
        if (BackgroundMusic != null)
        {
            BackgroundMusic.Stop();
        }

        // Ok tuzaðýný durdur ve mevcut oklarý yok et
        if (ArrowSpawner.Instance != null)
        {
            ArrowSpawner.Instance.StopTrapAndDestroyArrows();
        }


        if (hitByArrowSFX != null)
        {
            hitByArrowSFX.PlayOneShot(hitByArrowSFX.clip);
        }



        // Ölüm animasyonunu baþlat
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }

        

    }

    public void RestartGame()
    {
        // Reset player state
        isDead = false;
        hasWon = false;
        playerStopped = false;

        // Move player to starting point
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }
        if (cinemachineCamera != null)
        {
            cinemachineCamera.SetActive(true);
        }

        // Stop player movement
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        // Reset animation
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

        // Close death panel
        if (deathPanel != null)
            deathPanel.SetActive(false);

        // Stop any active arrow trap
        if (ArrowSpawner.Instance != null)
            ArrowSpawner.Instance.StopTrapAndDestroyArrows();

        // Stop sounds
        if (WalkingSFX != null)
            WalkingSFX.Stop();

        if (RunningSFX != null)
            RunningSFX.Stop();

        if (BackgroundMusic != null)
            BackgroundMusic.Play();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
