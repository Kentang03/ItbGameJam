using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player1; // Transform dari Player 1
    public Transform player2; // Transform dari Player 2
    public float minDistance = 5f; // Jarak minimum untuk trigger event

    public Camera player1Camera;
    public Camera player2Camera;
    public Camera targetCamera;

    public CinemachineVirtualCamera targetVirtualCamera;

    void Update()
    {
        // Hitung jarak antara Player 1 dan Player 2
        float distance = Vector3.Distance(player1.position, player2.position);

        // Jika jarak kurang dari minDistance, execute suatu method
        if (distance < minDistance)
        {
            OnPlayersClose();
        }
        else
        {
            ChangeToSplitCamera();
        }
    }

    // Method yang akan dijalankan ketika jarak antar pemain kurang dari minDistance
    void OnPlayersClose()
    {
        player1Camera.gameObject.SetActive(false);
        player2Camera.gameObject.SetActive(false);
        targetCamera.gameObject.SetActive(true);
        targetVirtualCamera.gameObject.SetActive(true);
    }

    void ChangeToSplitCamera()
    {
        player1Camera.gameObject.SetActive(true);
        player2Camera.gameObject.SetActive(true);
        targetCamera.gameObject.SetActive(false);
        targetVirtualCamera.gameObject.SetActive(false);

        // Tambahkan logika atau aksi yang ingin dilakukan di sini
    }

}
