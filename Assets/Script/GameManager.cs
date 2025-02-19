using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
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

    // Nilai gravitasi default
    private Vector3 defaultGravity;

    // Nilai kecepatan default
    public float defaultSpeed;

    public GameObject windPrefabs;

    void Start()
    {
        // Simpan nilai default gravitasi dan kecepatan
        defaultGravity = Physics.gravity;
        StartCoroutine(ChangeStatePeriodically());
        // Set state awal
        SetState(RulesState.Normal);
    }

    void Update()
    {
        
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
                break;

            case RulesState.GravityLow:
                SetGravity(defaultGravity * 0.2f); // Gravitasinya setengah dari normal
                Debug.Log("State: Gravity Low");
                break;

            case RulesState.GravityHigh:
                SetGravity(defaultGravity * 3f); // Gravitasinya dua kali lipat dari normal
                Debug.Log("State: Gravity High");
                break;

            case RulesState.SpeedBoost:
                SetSpeed(defaultSpeed * 2f); // Kecepatan dua kali lipat dari normal
                Debug.Log("State: Speed Boost");
                break;

            case RulesState.SpeedReduction:
                SetSpeed(defaultSpeed * 0.5f); // Kecepatan setengah dari normal
                Debug.Log("State: Speed Reduction");
                break;

            case RulesState.Wind:
                /*Instantiate(windPrefabs, windPrefabs.transform.position ,Quaternion.identity);*/
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

    }

    public float stateChangeInterval = 10f; // Interval perubahan state

    IEnumerator ChangeStatePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(stateChangeInterval);
            SetNormalState();
            RandomizeState();
        }
    }

    public void SetNormalState()
    {
        SetGravity(defaultGravity);
        SetSpeed(defaultSpeed);
        windPrefabs.SetActive(false);
        Physics.gravity = defaultGravity;

    }
}