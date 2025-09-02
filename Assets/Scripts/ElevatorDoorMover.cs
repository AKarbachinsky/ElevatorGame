using UnityEngine;
using UnityEngine.InputSystem;
using static ElevatorCartMover;

public class ElevatorDoorMover : MonoBehaviour
{
    public ElevatorCartMover elevatorCartMover;

    enum DoorState
    {
        ForcedOpen,
        ForcedClosed,
        CombatPhase
    }

    public enum DoorFlags
    {
        None,
        Opened,
        Closed
    }

    DoorState currentDoorState = DoorState.ForcedOpen;
    public DoorFlags currentDoorFlag = DoorFlags.None;

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

    [SerializeField] float closeThreshold = 0.01f;
    [SerializeField] float openThreshold = 0.01f;
    Vector3 currentTargetLeft;
    Vector3 currentTargetRight;
    bool isMovingLeft = false;
    bool isMovingRight = false;

    

    void Start()
    {
        currentDoorState = DoorState.ForcedOpen;
        currentDoorFlag = DoorFlags.None;

        doorOperateOpen.Enable();
        doorOperateClosed.Enable();

        leftDoorCloseLocal = leftDoor.transform.localPosition;
        rightDoorCloseLocal = rightDoor.transform.localPosition;

        leftDoorOpenLocal = leftDoorCloseLocal + Vector3.left * doorOpenDistance;
        rightDoorOpenLocal = rightDoorCloseLocal + Vector3.right * doorOpenDistance;

        currentTargetLeft = transform.position;
        currentTargetRight = transform.position;
    }

    void Update()
    {
        switch (currentDoorState)
        {
            case DoorState.ForcedOpen:
                ForceOpenDoors();
                doorOperateOpen.Disable();
                doorOperateClosed.Disable();

                if (elevatorCartMover.currentState == ElevatorState.Moving)
                {
                    currentDoorState = DoorState.ForcedClosed;
                }
                else if (elevatorCartMover.currentState == ElevatorState.CombatPhase)
                {
                    currentDoorState = DoorState.CombatPhase;
                }

                    break;
            case DoorState.ForcedClosed:
                ForceClosedDoors();

                if (elevatorCartMover.currentState == ElevatorState.Arrived)
                {
                    currentDoorState = DoorState.ForcedOpen;
                }

                break;
            case DoorState.CombatPhase:
                doorOperateOpen.Enable();
                doorOperateClosed.Enable();
                ProcessDoorOperations();
                break;
        }
    }

    private void ForceClosedDoors()
    {
        currentTargetLeft = leftDoorCloseLocal;
        currentTargetRight = rightDoorCloseLocal;

        isMovingLeft = true;
        isMovingRight = true;

        if (isMovingLeft)
        {
            leftDoor.transform.localPosition = Vector3.MoveTowards(leftDoor.transform.localPosition, currentTargetLeft, doorSpeed * Time.deltaTime);

            if ((leftDoor.transform.localPosition - currentTargetLeft).sqrMagnitude <= closeThreshold * closeThreshold)
            {
                isMovingLeft = false;
                currentTargetLeft = leftDoor.transform.localPosition;
                currentDoorFlag = DoorFlags.Closed;
            }
        }

        if (isMovingRight)
        {
            rightDoor.transform.localPosition = Vector3.MoveTowards(rightDoor.transform.localPosition, currentTargetRight, doorSpeed * Time.deltaTime);
            if ((rightDoor.transform.localPosition - currentTargetRight).sqrMagnitude <= openThreshold * openThreshold)
            {
                isMovingRight = false;
                currentTargetRight = rightDoor.transform.localPosition;
                currentDoorFlag = DoorFlags.Closed;
            }
        }
    }

    private void ForceOpenDoors()
    {
        currentTargetLeft = leftDoorOpenLocal;
        currentTargetRight = rightDoorOpenLocal;

        isMovingLeft = true;
        isMovingRight = true;

        if (isMovingLeft)
        {
            leftDoor.transform.localPosition = Vector3.MoveTowards(leftDoor.transform.localPosition, currentTargetLeft, doorSpeed * Time.deltaTime);

            if ((leftDoor.transform.localPosition - currentTargetLeft).sqrMagnitude <= closeThreshold * closeThreshold)
            {
                isMovingLeft = false;
                currentTargetLeft = leftDoor.transform.localPosition;
                currentDoorFlag = DoorFlags.Opened;
            }
        }

        if (isMovingRight)
        {
            rightDoor.transform.localPosition = Vector3.MoveTowards(rightDoor.transform.localPosition, currentTargetRight, doorSpeed * Time.deltaTime);
            if ((rightDoor.transform.localPosition - currentTargetRight).sqrMagnitude <= openThreshold * openThreshold)
            {
                isMovingRight = false;
                currentTargetRight = rightDoor.transform.localPosition;
                currentDoorFlag = DoorFlags.Opened;
            }
        }
    }

    void ProcessDoorOperations()
    {
        if (doorOperateOpen.IsPressed())
        {
            currentTargetLeft = leftDoorOpenLocal;
            currentTargetRight = rightDoorOpenLocal;

            isMovingLeft = true;
            isMovingRight = true;
        }
        else if (doorOperateClosed.IsPressed())
        {
            currentTargetLeft = leftDoorCloseLocal;
            currentTargetRight = rightDoorCloseLocal;

            isMovingLeft = true;
            isMovingRight = true;
        }

        if (isMovingLeft)
        {
            leftDoor.transform.localPosition = Vector3.MoveTowards(leftDoor.transform.localPosition, currentTargetLeft, doorSpeed * Time.deltaTime);

            if ((leftDoor.transform.localPosition - currentTargetLeft).sqrMagnitude <= closeThreshold * closeThreshold)
            {
                isMovingLeft = false;
                currentTargetLeft = leftDoor.transform.localPosition;
            }
        }

        if (isMovingRight)
        {
            rightDoor.transform.localPosition = Vector3.MoveTowards(rightDoor.transform.localPosition, currentTargetRight, doorSpeed * Time.deltaTime);
            if ((rightDoor.transform.localPosition - currentTargetRight).sqrMagnitude <= openThreshold * openThreshold)
            {
                isMovingRight = false;
                currentTargetRight = rightDoor.transform.localPosition;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
                Debug.Log("I hit enemy");
                break;
        }
    }
}