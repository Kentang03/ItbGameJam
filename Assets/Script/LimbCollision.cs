using UnityEngine;

public class LimbCollision : MonoBehaviour
{

    public PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        /*controller = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3) return;
        controller.isGrounded = true;
    }

}
