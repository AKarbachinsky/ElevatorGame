using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorCartMover : MonoBehaviour
{
    [SerializeField] GameObject elevatorCart;

    [SerializeField] InputAction cartOperate;

    Rigidbody rbElevatorCart;

    [SerializeField] float cartSpeed = 100f;

    void Start()
    {
        rbElevatorCart = elevatorCart.GetComponent<Rigidbody>();
        cartOperate.Enable();  
    }

    
    void FixedUpdate()
    {
        ProcessMovement();
    }

    void ProcessMovement()
    {
        if (cartOperate.IsPressed())
        {
            float cartInput = cartOperate.ReadValue<float>();
            rbElevatorCart.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            if (cartInput < 0)
            {
                rbElevatorCart.AddForce(Vector3.up * cartSpeed * Time.fixedDeltaTime);
                Debug.Log("Moving Elevator Cart Up");
            }
            else
            {
                rbElevatorCart.AddForce(Vector3.down * cartSpeed * Time.fixedDeltaTime);
                Debug.Log("Moving Elevator Cart Down");
            }
        }
        else
        {
            rbElevatorCart.linearVelocity = Vector3.zero;
            rbElevatorCart.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
