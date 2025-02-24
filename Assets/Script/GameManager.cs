using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    // Enum untuk menyimpan semua rules state yang mungkin
    public enum RulesState
    {
        Normal,
        GravityLow,
        GravityHigh,
        SpeedBoost,
        SpeedReduction,
        DontMove,
        DontJump,
        StoneRain,
        RandomizeState // State khusus untuk mengacak state lainnya
    }

    // State saat ini
    public RulesState currentState;

    // Daftar state yang bisa dipilih (untuk randomize)
    public List<RulesState> availableStates = new List<RulesState>
    {
        RulesState.Normal,
        RulesState.GravityLow,
        RulesState.GravityHigh,
        RulesState.SpeedBoost,
        RulesState.SpeedReduction,
        RulesState.DontMove,
        RulesState.DontJump,
        RulesState.StoneRain
    };

    // Referensi ke Rigidbody player
    public GameObject player1Rigidbody;
    public GameObject player2Rigidbody;

    public List<GameObject> player;
    public List<Camera> cameraController;

    // Nilai gravitasi default
    private Vector3 defaultGravity;
    public Transform spawnPoint;
    // Nilai kecepatan default
    public float defaultSpeed;

    public GameObject collider;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI stateTimerText; // Referensi ke UI Text untuk menampilkan waktu pergantian state

    public bool isStart = false;
    private bool isCountdownRunning = false;

    // Variabel untuk countdown
    private int countdownTime = 5; // Waktu countdown dalam detik

    private Coroutine stateChangeCoroutine; // Simpan referensi coroutine
    private float stateTimer; // Waktu tersisa sebelum pergantian state

    // Prefab batu untuk Stone Rain
    public GameObject stonePrefab;

    // Batas area map (sesuaikan dengan ukuran map Anda)
    public Vector2 mapBoundsMin = new Vector2(-10, -10); // Batas minimal (x, z)
    public Vector2 mapBoundsMax = new Vector2(10, 10);   // Batas maksimal (x, z)

    // Interval spawn batu
    public float stoneSpawnInterval = 1f;

    // Coroutine untuk Stone Rain
    private Coroutine stoneRainCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Simpan nilai default gravitasi dan kecepatan
        defaultGravity = Physics.gravity;

        // Set state awal
        SetState(RulesState.Normal);

        // Inisialisasi stateTimer
        stateTimer = stateChangeInterval;
    }

    void Update()
    {
        // Mulai countdown jika jumlah pemain >= 2 dan countdown belum berjalan
        if (player.Count >= 2 && !isCountdownRunning)
        {
            StartCoroutine(StartCountdown());
            isCountdownRunning = true;
        }

        if(player.Count == 1)
        {
            timeText.text = "Wait Other Player To Join";
        }

        if (currentState == RulesState.DontMove)
        {
            CheckCharacterMovement();
        }
        else if (currentState == RulesState.DontJump)
        {
            CheckCharacterJump();
        }

        // Update state timer jika game sudah mulai
        if (isStart)
        {
            UpdateStateTimer();
        }
    }

    // Method untuk mengupdate state timer
    private void UpdateStateTimer()
    {
        stateTimer -= Time.deltaTime; // Kurangi waktu tersisa
        stateTimerText.text = "" + Mathf.CeilToInt(stateTimer).ToString(); // Tampilkan waktu tersisa

        // Jika waktu habis, reset timer dan acak state
        if (stateTimer <= 0)
        {
            stateTimer = stateChangeInterval; // Reset timer
            RandomizeState(); // Acak state
        }
    }

    // Method untuk mengubah state
    public void SetState(RulesState newState)
    {
        currentState = newState;
        ApplyStateRules();
        stateTimer = stateChangeInterval; // Reset timer saat state berubah
    }

    // Method untuk mengacak state
    public void RandomizeState()
    {
        int randomIndex = Random.Range(0, availableStates.Count);
        SetState(availableStates[randomIndex]);
    }

    // Method untuk menerapkan aturan sesuai state
    private void ApplyStateRules()
    {
        switch (currentState)
        {
            case RulesState.Normal:
                SetNormalState();
                Debug.Log("State: Normal");
                timeText.text = "Normal";
                break;

            case RulesState.GravityLow:
                SetGravity(defaultGravity * 0.2f); // Gravitasinya setengah dari normal
                Debug.Log("State: Gravity Low");
                timeText.text = "Low Gravity";
                break;

            case RulesState.GravityHigh:
                SetGravity(defaultGravity * 3f); // Gravitasinya dua kali lipat dari normal
                Debug.Log("State: Gravity High");
                timeText.text = "High Gravity";
                break;

            case RulesState.SpeedBoost:
                SetSpeed(defaultSpeed * 2f); // Kecepatan dua kali lipat dari normal
                Debug.Log("State: Speed Boost");
                timeText.text = "Speed Boost";
                break;

            case RulesState.SpeedReduction:
                SetSpeed(defaultSpeed * 0.5f); // Kecepatan setengah dari normal
                Debug.Log("State: Speed Reduction");
                timeText.text = "Speed Reduction";
                break;

            case RulesState.DontMove:
                timeText.text = "Dont Move";
                break;

            case RulesState.DontJump:
                timeText.text = "Dont Jump";
                break;

            case RulesState.StoneRain:
                timeText.text = "Stone Rain";
                Debug.Log("State: Stone Rain");

                /*// Hentikan coroutine Stone Rain jika sudah berjalan
                if (stoneRainCoroutine != null)
                {
                    StopCoroutine(stoneRainCoroutine);
                }

                // Mulai Stone Rain
                stoneRainCoroutine = StartCoroutine(StartStoneRain());*/
                break;

            default:
                Debug.LogWarning("State tidak dikenal!");
                break;
        }
    }

    // Coroutine untuk Stone Rain
    IEnumerator StartStoneRain()
    {
        while (currentState == RulesState.StoneRain)
        {
            SpawnStone();
            yield return new WaitForSeconds(stoneSpawnInterval);
        }
    }

    // Method untuk menginstansiasi batu secara acak
    private void SpawnStone()
    {
        // Tentukan posisi acak di dalam batas map
        float randomX = Random.Range(mapBoundsMin.x, mapBoundsMax.x);
        float randomZ = Random.Range(mapBoundsMin.y, mapBoundsMax.y);

        // Posisi spawn batu (y tinggi agar batu jatuh dari atas)
        Vector3 spawnPosition = new Vector3(randomX, 20f, randomZ);

        // Instansiasi batu
        Instantiate(stonePrefab, spawnPosition, Quaternion.identity);
    }

    // Method untuk menghentikan Stone Rain
    private void StopStoneRain()
    {
        if (stoneRainCoroutine != null)
        {
            StopCoroutine(stoneRainCoroutine);
            stoneRainCoroutine = null;
        }
    }

    private void CheckCharacterMovement()
    {
        if (player1Rigidbody.GetComponent<PlayerController>().isMoving == true)
        {
            player1Rigidbody.GetComponent<PlayerController>().GameOver();
        }

        else if (player2Rigidbody.GetComponent<PlayerController>().isMoving == true)
        {
            player2Rigidbody.GetComponent<PlayerController>().GameOver();
        }
    }

    private void CheckCharacterJump()
    {
        if (player1Rigidbody.GetComponent<PlayerController>().isJumping == true)
        {
            player1Rigidbody.GetComponent<PlayerController>().GameOver();
        }

        else if (player2Rigidbody.GetComponent<PlayerController>().isJumping == true)
        {
            player2Rigidbody.GetComponent<PlayerController>().GameOver();
        }
    }

    // Method untuk mengubah gravitasi
    private void SetGravity(Vector3 newGravity)
    {
        Physics.gravity = newGravity;
    }

    // Method untuk mengubah kecepatan player
    private void SetSpeed(float newSpeed)
    {
        if (player1Rigidbody == null) return;

        player1Rigidbody.GetComponent<PlayerController>().speed = newSpeed;
        player2Rigidbody.GetComponent<PlayerController>().speed = newSpeed;
    }

    public float stateChangeInterval = 10f; // Interval perubahan state

    IEnumerator ChangeStatePeriodically()
    {
        while (isStart)
        {
            yield return new WaitForSeconds(stateChangeInterval);
            Debug.Log("Switch State");
            SetNormalState();
            RandomizeState();
        }
    }

    // Method untuk mengatur state normal
    public void SetNormalState()
    {
        SetGravity(defaultGravity);
        SetSpeed(defaultSpeed);
        Physics.gravity = defaultGravity;
        player1Rigidbody.GetComponent<PlayerController>().speed = defaultSpeed;
        player2Rigidbody.GetComponent<PlayerController>().speed = defaultSpeed;

        // Hentikan Stone Rain jika sedang berjalan
        StopStoneRain();
    }

    // Coroutine untuk countdown
    IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            timeText.text = countdownTime.ToString(); // Update UI
            yield return new WaitForSeconds(1); // Tunggu 1 detik
            countdownTime--; // Kurangi waktu countdown
        }

        if (!isStart)
        {
            timeText.text = "GO!";
            yield return new WaitForSeconds(1); // Tampilkan "GO!" selama 1 detik
            timeText.text = ""; // Hapus teks
            Destroy(collider);
            isStart = true;

            player1Rigidbody = player[0];
            player2Rigidbody = player[1];

            // Hentikan coroutine yang sedang berjalan (jika ada)
            if (stateChangeCoroutine != null)
            {
                StopCoroutine(stateChangeCoroutine);
            }

            // Mulai coroutine ChangeStatePeriodically
            stateChangeCoroutine = StartCoroutine(ChangeStatePeriodically());

            isCountdownRunning = false; // Reset status countdown

            // Reset state timer
            stateTimer = stateChangeInterval;
        }

        
    }


}