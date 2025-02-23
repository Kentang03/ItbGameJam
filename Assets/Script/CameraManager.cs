using Cinemachine;
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
    private bool activateTargetGroup = false;
    public CinemachineVirtualCamera targetVirtualCamera;
    public CinemachineTargetGroup targetGroup;

    private void Start()
    {
        // Inisialisasi awal (jika diperlukan)
    }

    void Update()
    {
        if (GameManager.Instance.player.Count <= 1) return;

        List<GameObject> player = GameManager.Instance.player;
        player1 = player[0].transform;
        player2 = player[1].transform;
        player1Camera = GameManager.Instance.cameraController[0];
        player2Camera = GameManager.Instance.cameraController[1];

        if (activateTargetGroup == false)
        {
            Debug.Log("Setting Target Group");
            SetTargetGroup(player);
            activateTargetGroup = true;
        }

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
    }

    void SetTargetGroup(List<GameObject> player)
    {
        // Buat array target dengan panjang sesuai jumlah pemain
        CinemachineTargetGroup.Target[] groupsTargets = new CinemachineTargetGroup.Target[player.Count];

        for (int i = 0; i < player.Count; i++)
        {
            // Isi target dengan transform dari pemain yang sesuai
            groupsTargets[i].target = player[i].transform;
            groupsTargets[i].weight = 1; // Berat target
            groupsTargets[i].radius = 4; // Radius target
        }

        // Assign array target ke targetGroup
        targetGroup.m_Targets = groupsTargets;

        // Debug log untuk memastikan target diatur dengan benar
        Debug.Log("Target Group Set with " + player.Count + " targets.");
    }
}