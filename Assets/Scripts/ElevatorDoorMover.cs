using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorDoorMover : MonoBehaviour
{
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [SerializeField] InputAction doorOperate;

    [SerializeField] float doorSpeed = 1f;

    Rigidbody rbLeftDoor;
    Rigidbody rbRightDoor;

    private float leftDoorOpenPosition = 1.475f;
    private float rightDoorOpenPosition = -1.475f;
    private float leftDoorClosePosition = 0.825f;
    private float rightDoorClosePosition = -0.825f;

    void Start()
    {
        doorOperate.Enable();
       
        rbLeftDoor = leftDoor.GetComponent<Rigidbody>();
        rbRightDoor = rightDoor.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ProcessDoorOperations();

        Vector3 leftPos = rbLeftDoor.position;
        leftPos.x = Mathf.Clamp(leftPos.x, leftDoorClosePosition, leftDoorOpenPosition);
        rbLeftDoor.position = leftPos;

        Vector3 rightPos = rbRightDoor.position;
        rightPos.x = Mathf.Clamp(rightPos.x, rightDoorOpenPosition, rightDoorClosePosition);
        rbRightDoor.position = rightPos;
    }

    void ProcessDoorOperations()
    {
        float doorInput = doorOperate.ReadValue<float>();

        if (doorOperate.IsPressed())
        {
            if (doorInput < 0)
            {
                OpenLeftDoor();
                OpenRightDoor();
            }
            else
            {
                CloseLeftDoor();
                CloseRightDoor();
            }
        }
        else
        {
            rbLeftDoor.linearVelocity = Vector3.zero;
            rbRightDoor.linearVelocity = Vector3.zero;
        }
    }

    void OpenLeftDoor()
    {
        if (leftDoor != null)
        {
            
            rbLeftDoor.AddForce(Vector3.left * doorSpeed * Time.fixedDeltaTime);
        }
    }

    void OpenRightDoor()
    {
        if (rightDoor != null)
        {
            
            rbRightDoor.AddForce(Vector3.right * doorSpeed * Time.fixedDeltaTime);
        }
    }

    void CloseLeftDoor()
    {
        if (leftDoor != null)
        {
            
            rbLeftDoor.AddForce(Vector3.right * doorSpeed * Time.fixedDeltaTime);
        }
    }

    void CloseRightDoor()
    {
        if (rightDoor != null)
        {
            
            rbRightDoor.AddForce(Vector3.left * doorSpeed * Time.fixedDeltaTime);
        }
    }
}