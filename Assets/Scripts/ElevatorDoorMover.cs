using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static ElevatorCartMover;


public class ElevatorDoorMover : MonoBehaviour
{
    #region Enums

    public enum DoorState
    {
        None,
        Idle,
        ForcedOpen,
        PreMovement,
        Moving,
        Arrived,
        ForcedClosed,
        CombatPhase
    }

    public enum DoorSide
    {
        Left,
        Right
    }

    #endregion

    #region Inspector: Config 

    [SerializeField] private DoorSide side; // set Left on LeftDoor, Right on RightDoor

    [SerializeField] bool showDebug = true;

    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [SerializeField] InputAction doorOperateOpen;
    [SerializeField] InputAction doorOperateClosed;

    [SerializeField] float doorSpeed = 1f;

    [SerializeField] float closeThreshold = 0.01f;
    [SerializeField] float openThreshold = 0.01f;

    [SerializeField] Animator leftAnim;
    [SerializeField] Animator rightAnim;

    [SerializeField] string openParam = "TrOpen";
    [SerializeField] string closeParam = "TrClose";
    [SerializeField] string idleParam = "TrIdle";
    [SerializeField] string movingParam = "TrMoving";
    [SerializeField] string emptyParam = "TrEmpty";

    #endregion

    public ElevatorCartMover elevatorCartMover;

    public DoorState currentDoorState = DoorState.ForcedOpen;

    Vector3 leftDoorOpenLocal;
    Vector3 rightDoorOpenLocal; 
    Vector3 leftDoorCloseLocal; 
    Vector3 rightDoorCloseLocal; 

    Vector3 currentTargetLeft;
    Vector3 currentTargetRight;

    float doorOpenDistance = 2.05f;

    bool isMovingLeft = false;
    bool isMovingRight = false;
    public bool isDoorsMoving = false; 

    public bool isIdlePlaying = false;
    bool isPlayingMovingAnim = false;

    public bool isReadyToMove = false;

    public bool isDoorOpen = false;
    public bool isDoorsClosed = false;

    private void Awake()
    {
        leftAnim = leftDoor.GetComponent<Animator>();
        rightAnim = rightDoor.GetComponent<Animator>();
    }
    
    void Start()
    {
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

                if (!isDoorsMoving)
                {
                    currentDoorState = DoorState.Idle;
                }
                break;

            case DoorState.Idle:

                if (!isDoorOpen)
                {
                    ForceOpenDoors();
                }
                else if (!isIdlePlaying && isDoorOpen)
                {
                    PlayIdleAnimation();
                }
                break;

            case DoorState.ForcedClosed:
                ForceClosedDoors();

                if (!isDoorsMoving)
                {
                    currentDoorState = DoorState.Moving;
                }
                break;

            case DoorState.PreMovement:
                if (!isDoorsClosed)
                {
                    ForceClosedDoors();
                }
                break;

            case DoorState.Moving:
                if (!isPlayingMovingAnim)
                {
                    ResetAllLeftAnimTriggers();
                    ResetAllRightAnimTriggers();
                    PlayMovingAnimation();
                }
                break;

                case DoorState.Arrived:

                if (!isDoorOpen)
                {
                    ForceOpenDoors();
                }
                break;

            case DoorState.CombatPhase:
                doorOperateOpen.Enable();
                doorOperateClosed.Enable();
                ProcessDoorOperations();

                if (elevatorCartMover.currentState == ElevatorState.Idle)
                {
                    currentDoorState = DoorState.ForcedOpen;
                }
                break;
        }
    }

    #region ForceOpen State Methods

    private void ForceOpenDoors()
    {
        currentTargetLeft = leftDoorOpenLocal;
        currentTargetRight = rightDoorOpenLocal;

        if (!isDoorsMoving)
        {
            if (leftAnim != null)
            {
                ResetAllLeftAnimTriggers();
                leftAnim.SetTrigger(openParam);
            }

            if (rightAnim != null)
            {
                ResetAllRightAnimTriggers();
                rightAnim.SetTrigger(openParam);
            }

            isDoorsMoving = true;
        }


        bool leftFinished = IsDoorFinishedOpening(leftAnim, "LeftDoor_Open_Door") || IsDoorFinishedOpening(leftAnim, "Open");
        bool rightFinished = IsDoorFinishedOpening(rightAnim, "RightDoor_Open_Door") || IsDoorFinishedOpening(rightAnim, "Open");

        if (leftFinished && rightFinished)
        {
            currentTargetLeft = leftDoor.transform.localPosition;
            currentTargetRight = rightDoor.transform.localPosition;

            isDoorsMoving = false; 
            isDoorOpen = true;
        }
    }

    private bool IsDoorFinishedOpening(Animator anim, string animationNameOrTag)
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        bool isCorrectAnim = state.IsName(animationNameOrTag);
        bool isFinished = state.normalizedTime >= 0.99f && !anim.IsInTransition(0);
        return isCorrectAnim && isFinished;
    }

    #endregion

    #region ForceClose State Methods

    private void ForceClosedDoors()
    {
        currentTargetLeft = leftDoorCloseLocal;
        currentTargetRight = rightDoorCloseLocal;

        if (!isDoorsMoving)
        {
            if (leftAnim != null)
            {
                ResetAllLeftAnimTriggers();
                leftAnim.SetTrigger(closeParam);
            }

            if (rightAnim != null)
            {
                ResetAllRightAnimTriggers();
                rightAnim.SetTrigger(closeParam);
            }

            isDoorsMoving = true;
        }

        bool leftFinished = IsDoorFinishedClosing(leftAnim, "LeftDoor_Close_Door") || IsDoorFinishedClosing(leftAnim, "Close");
        bool rightFinished = IsDoorFinishedClosing(rightAnim, "RightDoor_Close_Door") || IsDoorFinishedClosing(rightAnim, "Close");

        if (leftFinished && rightFinished)
        {
            currentTargetLeft = leftDoor.transform.localPosition;
            currentTargetRight = rightDoor.transform.localPosition;

            isDoorsMoving = false;
            isDoorsClosed = true;
        }
    }

    private bool IsDoorFinishedClosing(Animator anim, string animationNameOrTag)
    {
        if (anim == null) return false;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        bool isCorrectAnim = state.IsTag(animationNameOrTag) || state.IsName(animationNameOrTag);
        bool isFinished = state.normalizedTime >= 0.99f && !anim.IsInTransition(0);

        return isCorrectAnim && isFinished;
    }

    #endregion

    #region Idle

    private void PlayIdleAnimation()
    {
        if (leftAnim != null)
        {
            if (IsDoorFinishedOpening(leftAnim, "LeftDoor_Open_Door"))
            {
                leftAnim.SetTrigger(idleParam);
                Debug.Log("Played Idle Animation on Left Door");
            }
        }

        if (rightAnim != null)
        {
            if (IsDoorFinishedOpening(rightAnim, "RightDoor_Open_Door"))
            {
                rightAnim.SetTrigger(idleParam);
                Debug.Log("Played Idle Animation on Right Door");
            }   
        }

        isIdlePlaying = true;
    }

    #endregion

    #region Moving

    private void PlayMovingAnimation()
    {
        if (leftAnim != null)
        {
            ResetAllLeftAnimTriggers();
            leftAnim.SetTrigger(movingParam);
        }
        if (rightAnim != null)
        {
            ResetAllRightAnimTriggers();
            rightAnim.SetTrigger(movingParam);
        }

        isPlayingMovingAnim = true;
    }

    #endregion

    private void ResetAllLeftAnimTriggers()
    {
        leftAnim.ResetTrigger(idleParam);
        leftAnim.ResetTrigger(openParam);
        leftAnim.ResetTrigger(closeParam);
        leftAnim.ResetTrigger(movingParam);
    }

    private void ResetAllRightAnimTriggers()
    {
        rightAnim.ResetTrigger(idleParam);
        rightAnim.ResetTrigger(openParam);
        rightAnim.ResetTrigger(closeParam);
        rightAnim.ResetTrigger(movingParam);
    }

    private IEnumerator StartForceOpenState(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentDoorState = DoorState.ForcedOpen;
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

    #region State Transition Handlers 

    public void OnEnemyHitElevator()
    {
        Debug.Log("Enemy hit the elevator! Running door reaction logic...");
        StartCoroutine(StartForceOpenState(3f));
    }

    public void GoIdle()
    {
        currentDoorState = DoorState.Idle;
        isIdlePlaying = false;
        isDoorsMoving = false;
        isPlayingMovingAnim = false;
        isDoorsClosed = false;
        isDoorOpen = false;
        ResetAllLeftAnimTriggers();
        ResetAllRightAnimTriggers();
        ForceOpenDoors();
    }

    public void PrepareForMovement()
    {
        currentDoorState = DoorState.PreMovement;
    }

    public void StartMoving()
    {
        currentDoorState = DoorState.Moving;
    }

    public void ForceOpen()
    {
        ForceOpenDoors();
        currentDoorState = DoorState.ForcedOpen;
    }
    
    public void ForceClose()
    {
        ForceClosedDoors(); 
        currentDoorState = DoorState.ForcedClosed;
    }

    public void HandleArrival()
    {
        currentDoorState = DoorState.Arrived;
        ResetAllLeftAnimTriggers();
        ResetAllRightAnimTriggers();
        isIdlePlaying = false;
        isDoorsMoving = false;
        isPlayingMovingAnim = false;
        isDoorsClosed = false;
        isDoorOpen = false;
        ForceOpenDoors();
    }

    public void EnterCombat()
    {
        currentDoorState = DoorState.CombatPhase;
        ResetAllLeftAnimTriggers();
        ResetAllRightAnimTriggers();
        isIdlePlaying = false;
        isDoorsMoving = false;
        isPlayingMovingAnim = false;
        isDoorsClosed = false;
        isDoorOpen = false;
        leftAnim.SetTrigger(emptyParam);
    }

    #endregion

    #region Debug

    void OnGUI()
    {
        if (!showDebug) return;

        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 100, 200, 20), "Door State: " + currentDoorState);
    }

    #endregion
}