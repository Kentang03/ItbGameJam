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
        Wind,
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
        RulesState.Wind
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

    public GameObject windPrefabs;

    public GameObject collider;

    public TextMeshProUGUI timeText;

    public bool isStart = false;
    private bool isCountdownRunning = false;

    // Variabel untuk countdown
    private int countdownTime = 5; // Waktu countdown dalam detik

    private Coroutine stateChangeCoroutine; // Simpan referensi coroutine

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
    }

    void Update()
    {
        // Mulai countdown jika jumlah pemain >= 2 dan countdown belum berjalan
        if (player.Count >= 2 && !isCountdownRunning)
        {
            StartCoroutine(StartCountdown());
            isCountdownRunning = true;
        }
    }

    // Method untuk mengubah state
    public void SetState(RulesState newState)
    {
        currentState = newState;
        ApplyStateRules();
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

            case RulesState.Wind:
                timeText.text = "Wind";
                windPrefabs.SetActive(true);
                break;

            default:
                Debug.LogWarning("State tidak dikenal!");
                break;
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
        windPrefabs.SetActive(false);
        Physics.gravity = defaultGravity;
        player1Rigidbody.GetComponent<PlayerController>().speed = defaultSpeed;
        player2Rigidbody.GetComponent<PlayerController>().speed = defaultSpeed;
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
        }
    }
}