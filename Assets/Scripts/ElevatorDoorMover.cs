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

    [SerializeField] bool showDebug = true;

    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [SerializeField] InputAction doorClose;
    [SerializeField] InputAction doorOperateSlowAttack;

    [SerializeField] Animator leftAnim;
    [SerializeField] Animator rightAnim;

    [SerializeField] string openParam = "TrOpen";
    [SerializeField] string closeParam = "TrClose";
    [SerializeField] string idleParam = "TrIdle";
    [SerializeField] string movingParam = "TrMoving";
    [SerializeField] string emptyParam = "TrEmpty";
    [SerializeField] string slowAttackParam = "TrSlowAttack";
    [SerializeField] string mediumAttackParam = "TrMediumAttack";
    [SerializeField] string fastAttackParam = "TrFastAttack";

    #endregion

    public ElevatorCartMover elevatorCartMover;

    public DoorState currentDoorState = DoorState.ForcedOpen;

    public bool isDoorsMoving = false;
    public bool isIdlePlaying = false;
    public bool isDoorCompletedAttack = false;
    public bool isReadyToMove = false;
    public bool isDoorOpen = false;
    public bool isDoorsClosed = false;
    public bool playerReadyToMove = false;

    bool isDoorBegunAttack = false;
    bool isPlayingMovingAnim = false;

    private void Awake()
    {
        leftAnim = leftDoor.GetComponent<Animator>();
        rightAnim = rightDoor.GetComponent<Animator>();
    }
    
    void Start()
    {
        doorOperateSlowAttack.Enable();
        doorClose.Enable();
    }

    void Update()
    {
        switch (currentDoorState)
        {
            case DoorState.ForcedOpen:
                ForceOpenDoors();
                doorOperateSlowAttack.Disable();

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

                if (isIdlePlaying)
                {
                    if(doorClose.WasPressedThisFrame())
                    {
                        playerReadyToMove = true;
                    }
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
                doorOperateSlowAttack.Enable();
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
        leftAnim.ResetTrigger(slowAttackParam);
    }

    private void ResetAllRightAnimTriggers()
    {
        rightAnim.ResetTrigger(idleParam);
        rightAnim.ResetTrigger(openParam);
        rightAnim.ResetTrigger(closeParam);
        rightAnim.ResetTrigger(movingParam);
        rightAnim.ResetTrigger(slowAttackParam);
    }

    private IEnumerator StartForceOpenState(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentDoorState = DoorState.ForcedOpen;
    }

    void ProcessDoorOperations()
    {
        if (doorOperateSlowAttack.IsPressed())
        {
            PlaySlowAttack();
        }
    }

    #region Attack Methods

    void PlaySlowAttack()
    {
        if (!isDoorBegunAttack)
        {
            ResetAllLeftAnimTriggers();
            ResetAllRightAnimTriggers();

            leftAnim.SetTrigger(slowAttackParam);
            rightAnim.SetTrigger(slowAttackParam);

            isDoorBegunAttack = true;
            isDoorCompletedAttack = false;
            return;
        }

        bool leftFinished = IsDoorFinishedSlowAttack(leftAnim);
        bool rightFinished = IsDoorFinishedSlowAttack(rightAnim);

        if (isDoorBegunAttack && leftFinished && rightFinished)
        {
            isDoorBegunAttack = false;
            isDoorCompletedAttack = true;
        }
    }

    private bool IsDoorFinishedSlowAttack(Animator anim)
    {
        if (anim == null) return false;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        bool inEmptyState = state.IsName("Empty");
        bool notTransitioning = !anim.IsInTransition(0);

        return inEmptyState && notTransitioning;
    }

    void PlayMediumAttack()
    {
        if (!isDoorBegunAttack)
        {
            ResetAllLeftAnimTriggers();
            ResetAllRightAnimTriggers();

            leftAnim.SetTrigger(slowAttackParam);
            rightAnim.SetTrigger(slowAttackParam);

            isDoorBegunAttack = true;
            isDoorCompletedAttack = false;
            return;
        }

        bool leftFinished = IsDoorFinishedSlowAttack(leftAnim);
        bool rightFinished = IsDoorFinishedSlowAttack(rightAnim);

        if (isDoorBegunAttack && leftFinished && rightFinished)
        {
            isDoorBegunAttack = false;
            isDoorCompletedAttack = true;
        }
    }

    #endregion



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
        playerReadyToMove = false;
        currentDoorState = DoorState.PreMovement;
    }

    public void StartMoving()
    {
        currentDoorState = DoorState.Moving;
    }

    public void HandleFloorSelected()
    {

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