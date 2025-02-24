using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkingwin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 12)
        {
            GameManager.Instance.timeText.text = "Player 1 Winner";
            Time.timeScale = 0;
        }   
    }
}
