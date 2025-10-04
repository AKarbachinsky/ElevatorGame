using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static ElevatorCartMover;

public class ElevatorDoorMover : MonoBehaviour
{
    public ElevatorCartMover elevatorCartMover;

    public enum DoorState
    {
        None,
        Idle,
        ForcedOpen,
        Moving,
        ForcedClosed,
        CombatPhase
    }

    public enum DoorFlags
    {
        None,
        Opened,
        Closed
    }

    public enum DoorSide 
    { Left, 
      Right
    }

    [SerializeField] private DoorSide side; // set Left on LeftDoor, Right on RightDoor

    public DoorState currentDoorState = DoorState.ForcedOpen;
    public DoorFlags currentDoorFlag = DoorFlags.None;

    [SerializeField] bool showDebug = true;

    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [SerializeField] InputAction doorOperateOpen;
    [SerializeField] InputAction doorOperateClosed;

    [SerializeField] float doorSpeed = 1f;

    Vector3 leftDoorOpenLocal;
    Vector3 rightDoorOpenLocal; 
    Vector3 leftDoorCloseLocal; 
    Vector3 rightDoorCloseLocal; 

    float doorOpenDistance = 2.05f;

    [SerializeField] float closeThreshold = 0.01f;
    [SerializeField] float openThreshold = 0.01f;
    Vector3 currentTargetLeft;
    Vector3 currentTargetRight;
    bool isMovingLeft = false;
    bool isMovingRight = false;

    public bool isEnemyHit = false;  
    
    bool hasOpenedLeft = false;
    bool hasOpenedRight = false;

    bool hasClosedLeft = false;
    bool hasClosedRight = false;

    bool canPlayIdleAnim = false;
    bool isPlayingMovingAnim = false;

    [SerializeField] Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        hasOpenedLeft = false;
        hasOpenedRight = false;

        hasClosedLeft = false;
        hasClosedRight = false;

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

        isEnemyHit = false;  
    }

    void Update()
    {
        switch (currentDoorState)
        {
            case DoorState.ForcedOpen:
                isEnemyHit = false;

                if (elevatorCartMover.currentState != ElevatorState.CombatPhase)
                {
                    if (elevatorCartMover.currentState == ElevatorState.Idle || elevatorCartMover.currentState == ElevatorState.Arrived)
                    {
                        ForceOpenDoors();
                        doorOperateOpen.Disable();
                        doorOperateClosed.Disable();
                    }
                }
                else if (elevatorCartMover.currentState == ElevatorState.CombatPhase)
                {
                    currentDoorState = DoorState.CombatPhase;
                }
                break;

            case DoorState.Idle:
                PlayIdleAnimation();
                
                if (elevatorCartMover.currentState == ElevatorState.Moving)
                {
                    canPlayIdleAnim = false;
                    animator.ResetTrigger("TrLeftIdle");
                    animator.ResetTrigger("TrRightIdle");
                    currentDoorState = DoorState.ForcedClosed;
                }
                break;

            case DoorState.ForcedClosed:
                ForceClosedDoors();

                if (currentDoorFlag == DoorFlags.Closed)
                {
                    currentDoorState = DoorState.Moving;
                }

                break;

            case DoorState.Moving:
                if (!isPlayingMovingAnim)
                {
                    PlayMovingAnimation();
                }

                if (elevatorCartMover.currentState == ElevatorState.Arrived)
                {
                    isPlayingMovingAnim = false;
                    currentDoorState = DoorState.ForcedOpen;
                }

                break;

            case DoorState.CombatPhase:
                doorOperateOpen.Enable();
                doorOperateClosed.Enable();
                ProcessDoorOperations();

                hasOpenedLeft = false; // reset to allow re-triggering animation for idle state
                hasOpenedRight = false; // reset to allow re-triggering animation for idle state

                if (isEnemyHit)
                {
                    StartCoroutine(StartForceOpenState(3f));
                }

                if (elevatorCartMover.currentState == ElevatorState.Idle)
                {
                    currentDoorState = DoorState.ForcedOpen;
                }
                break;
        }
    }

    private IEnumerator StartForceOpenState(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentDoorState = DoorState.ForcedOpen;
    }

    private void PlayMovingAnimation()
    {
        
        if (side == DoorSide.Left)
        {
            if (animator != null)
            {
                animator.ResetTrigger("TrLeftClose");
                animator.ResetTrigger("TrLeftIdle");
                animator.ResetTrigger("TrLeftOpen");
                animator.SetTrigger("TrLeftMoving");
            }
        }

        if (side == DoorSide.Right)
        {
            if (animator != null)
            {
                animator.ResetTrigger("TrRightClose");
                animator.SetTrigger("TrRightMoving");
            }
        }

        isPlayingMovingAnim = true;

    }

    private void PlayIdleAnimation()
    {
        if (canPlayIdleAnim)
        {
            if (side == DoorSide.Left)
            {
                if (animator != null)
                {
                    animator.SetTrigger("TrLeftIdle");
                    Debug.Log("Played Left Idle Animation");
                }
            }

            if (side == DoorSide.Right)
            {
                if (animator != null)
                {
                    animator.SetTrigger("TrRightIdle");
                    Debug.Log("Played Right Idle Animation");
                }
            }
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
            if (side == DoorSide.Left)
            {
                if (!hasClosedLeft)
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("TrLeftClose");
                        hasClosedLeft = true;
                        hasOpenedLeft = false;
                    }
                }

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (!animator.IsInTransition(0) && stateInfo.IsName("LeftDoor_Armature_Close") && stateInfo.normalizedTime >= .99f)
                {
                    isMovingLeft = false;
                    currentTargetLeft = leftDoor.transform.localPosition;
                    currentDoorFlag = DoorFlags.Closed;
                }
            }
        }

        if (isMovingRight)
        {
            if (side == DoorSide.Right)
            {
                if (!hasClosedRight)
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("TrRightClose");
                        hasClosedRight = true;
                        hasOpenedRight = false;
                    }
                }

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (!animator.IsInTransition(0) && stateInfo.IsName("RightDoor_Armature_Close") && stateInfo.normalizedTime >= .99f)
                {
                    isMovingRight = false;
                    currentTargetRight = rightDoor.transform.localPosition;
                    currentDoorFlag = DoorFlags.Closed;
                }
            }
        }

        if (isMovingRight == false && isMovingLeft == false)
        {
            isPlayingMovingAnim = false;
            currentDoorState = DoorState.Moving;
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
            if (side == DoorSide.Left)
            {
                if (!hasOpenedLeft)
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("TrLeftOpen");
                        hasOpenedLeft = true;
                        hasClosedLeft = false;
                        return;
                    }
                }

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (!animator.IsInTransition(0) && stateInfo.IsName("LeftDoor_Armature_Open") && stateInfo.normalizedTime >= .99f)
                {
                    isMovingLeft = false;
                    currentTargetLeft = leftDoor.transform.localPosition;
                    currentDoorFlag = DoorFlags.Opened;
                    Debug.Log(currentDoorFlag);

                    if (elevatorCartMover.currentState != ElevatorState.CombatPhase && elevatorCartMover.currentState != ElevatorState.Arrived)
                    {
                        canPlayIdleAnim = true;
                        currentDoorState = DoorState.Idle;
                    } 
                }
                else
                {
                    canPlayIdleAnim = false;
                }
            }
        }

        if (isMovingRight)
        {
            if(side == DoorSide.Right)
            {
                if (!hasOpenedRight)
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("TrRightOpen");
                        hasOpenedRight = true;
                        hasClosedRight = false;
                        return;
                    }
                }
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (!animator.IsInTransition(0) && stateInfo.IsName("RightDoor_Armature_Open") && stateInfo.normalizedTime >= .99f)
                {
                    isMovingRight = false;
                    currentTargetRight = rightDoor.transform.localPosition;
                    currentDoorFlag = DoorFlags.Opened;
                    Debug.Log(currentDoorFlag);

                    if (elevatorCartMover.currentState != ElevatorState.CombatPhase && elevatorCartMover.currentState != ElevatorState.Arrived)
                    {
                        canPlayIdleAnim = true;
                        currentDoorState = DoorState.Idle;
                    }
                }
                else
                {
                    canPlayIdleAnim = false;
                }
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
                isEnemyHit = true;
                break;
        }
    }

    #region Debug

    void OnGUI()
    {
        if (!showDebug) return;

        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 100, 200, 20), "Door State: " + currentDoorState);
    }

    #endregion
}