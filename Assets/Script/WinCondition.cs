using UnityEngine;
using UnityEngine.SceneManagement; // Import namespace untuk SceneManager

public class WinCondition : MonoBehaviour
{
    // LayerMask untuk Player1 dan Player2
    public LayerMask player1Layer;
    public LayerMask player2Layer;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        ReloadScene();
        // Cek jika objek yang bertabrakan adalah Player1
        if (player1Layer == (player1Layer | (1 << collision.gameObject.layer)))
        {
            Debug.Log("Player1 reached the finish line!");
            GameManager.Instance.timeText.text = "Player 1 Winner";
            GameManager.Instance.isStart = false; // Menghentikan game
            
        }

        // Cek jika objek yang bertabrakan adalah Player2
        if (player2Layer == (player2Layer | (1 << collision.gameObject.layer)))
        {
            Debug.Log("Player2 reached the finish line!");
            GameManager.Instance.timeText.text = "Player 2 Winner";
            GameManager.Instance.isStart = false; // Menghentikan game
            ReloadScene(); // Memuat ulang scene
        }
    }

    // Method untuk memuat ulang scene
    private void ReloadScene()
    {
        // Dapatkan nama scene yang sedang aktif
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Memuat ulang scene
        SceneManager.LoadScene("Main");
    }
}