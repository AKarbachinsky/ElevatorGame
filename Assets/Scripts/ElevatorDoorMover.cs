using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorDoorMover : MonoBehaviour
{
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [SerializeField] InputAction doorOperateOpen;
    [SerializeField] InputAction doorOperateClosed;

    [SerializeField] float doorSpeed = 1f;

    Vector3 leftDoorOpenLocal;
    Vector3 rightDoorOpenLocal; 
    Vector3 leftDoorCloseLocal; 
    Vector3 rightDoorCloseLocal; 

    float doorOpenDistance = -0.75f;

    void Start()
    {
        doorOperateOpen.Enable();
        doorOperateClosed.Enable();

        leftDoorCloseLocal = leftDoor.transform.localPosition;
        rightDoorCloseLocal = rightDoor.transform.localPosition;

        leftDoorOpenLocal = leftDoorCloseLocal + Vector3.left * doorOpenDistance;
        rightDoorOpenLocal = rightDoorCloseLocal + Vector3.right * doorOpenDistance;
    }

    void Update()
    {
        ProcessDoorOperations();
    }

    void ProcessDoorOperations()
    {
        if (doorOperateOpen.IsPressed())
        {
            OpenLeftDoor();
            OpenRightDoor();
        }
        else if (doorOperateClosed.IsPressed())
        {
            CloseLeftDoor();
            CloseRightDoor();
        }
    }

    void OpenLeftDoor()
    {
        if (leftDoor != null)
        {
            leftDoor.transform.localPosition = Vector3.MoveTowards(leftDoor.transform.localPosition, leftDoorOpenLocal, doorSpeed * Time.deltaTime);
        }
    }

    void OpenRightDoor()
    {
        if (rightDoor != null)
        {
            rightDoor.transform.localPosition = Vector3.MoveTowards(rightDoor.transform.localPosition, rightDoorOpenLocal, doorSpeed * Time.deltaTime);
        }
    }

    void CloseLeftDoor()
    {
        if (leftDoor != null)
        {
            leftDoor.transform.localPosition = Vector3.MoveTowards(leftDoor.transform.localPosition, leftDoorCloseLocal, doorSpeed * Time.deltaTime);
        }
    }

    void CloseRightDoor()
    {
        if (rightDoor != null)
        {
            rightDoor.transform.localPosition = Vector3.MoveTowards(rightDoor.transform.localPosition, rightDoorCloseLocal, doorSpeed * Time.deltaTime);
        }
    }
}