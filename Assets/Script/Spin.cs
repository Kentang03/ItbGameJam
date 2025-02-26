using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float rotationSpeed = 300f; // Kecepatan rotasi kipas (derajat per detik)

    void Update()
    {
        // Memutar kipas di sumbu Z
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
