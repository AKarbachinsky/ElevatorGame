using UnityEngine;
using UnityEngine.InputSystem;

public class FreeRoamMover : MonoBehaviour
{

    [SerializeField] InputAction operateUp;
    [SerializeField] InputAction operateDown;

    [SerializeField] Rigidbody rb;
    [SerializeField] float thrustStrength = 1000f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!rb)
            rb = GetComponentInChildren<Rigidbody>(true);

        operateUp.Enable();
        operateDown.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (operateUp.IsPressed())
        {
            rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
        }

        if (operateDown.IsPressed())
        {
            rb.AddRelativeForce(Vector3.down * thrustStrength * Time.fixedDeltaTime);
        }
    }
}
