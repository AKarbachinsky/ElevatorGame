using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using static ElevatorDoorMover;
using static EnemyBehavior;

public class ElevatorCartMover : MonoBehaviour
{
    #region Enums
    public enum Floor
    {
        None,
        First,
        Second,
        Third,
        Fourth
    }

    enum FloorSelected
    {
        None = 0,
        FirstFloorSelected,
        SecondFloorSelected,
        ThirdFloorSelected,
        FourthFloorSelected
    }

    public enum ElevatorState
    {
        None = 0,
        Idle,
        PreMovement,
        Moving,
        Arrived,
        CombatPhase,
    }

    #endregion

    #region Fields

    public ElevatorDoorMover elevatorDoorMover;
    public EnemyBehavior enemyBehavior;

    public ElevatorState currentState = ElevatorState.Idle;
    FloorSelected selectedFloor = FloorSelected.None;

    [SerializeField] string idleParam = "TrCartIdle";
    [SerializeField] string movingParam = "TrCartMoving";
    [SerializeField] string emptyParam = "TrCartEmpty";

    [SerializeField] GameObject elevatorCart;

    [SerializeField] GameObject firstFloorButton;
    [SerializeField] GameObject secondFloorButton;
    [SerializeField] GameObject thirdFloorButton;
    [SerializeField] GameObject fourthFloorButton;

    [SerializeField] Material floorSelectedMaterial;
    [SerializeField] Material floorUnselectedMaterial;

    [SerializeField] GameObject leftEyeBall;
    [SerializeField] GameObject rightEyeBall;

    [SerializeField] InputAction cartOperate;

    [SerializeField] InputAction operateToFirstFloor;
    [SerializeField] InputAction operateToSecondFloor;
    [SerializeField] InputAction operateToThirdFloor;
    [SerializeField] InputAction operateToFourthFloor;

    [SerializeField] float cartSpeed = 100f;

    [SerializeField] float arriveThreshold = 0.01f;

    [SerializeField] Animator animator;

    public Vector3 firstFloorLocation = new Vector3(0, 0, 0);
    public Vector3 secondFloorLocation = new Vector3(0, 10, 0);
    public Vector3 thirdFloorLocation = new Vector3(0, 20, 0);
    public Vector3 fourthFloorLocation = new Vector3(0, 30, 0);

    public Vector3 currentTarget;

    public Floor arrivedFloor = Floor.None;

    public bool isMoving = false;
    public bool isArrived = false;

    public bool cartIdleIsPlaying = false;
    bool cartMovingIsPlaying = false;

    public bool isMoveRequested = false;

    #endregion

    private void Awake()
    {
        if (!animator) 
            animator = GetComponentInChildren<Animator>(true);
    }

    void Start()
    {
        EnableInputAction();
        currentTarget = transform.position;
        currentState = ElevatorState.Idle;
    }

    private void EnableInputAction()
    {
        cartOperate.Enable();
        operateToFirstFloor.Enable();
        operateToSecondFloor.Enable();
        operateToThirdFloor.Enable();
        operateToFourthFloor.Enable();
    }

    void DisableInputAction()
    {
        cartOperate.Disable();
        operateToFirstFloor.Disable();
        operateToSecondFloor.Disable();
        operateToThirdFloor.Disable();
        operateToFourthFloor.Disable();
    }

    void Update()
    {
        switch (currentState)
        {
            case ElevatorState.None:
                Debug.Log("NO STATE");
                break;
            case ElevatorState.Idle:
                EnableInputAction();
                WaitForInput();
                PlayIdleAnimation();
                break;

            case ElevatorState.PreMovement:
                // floor select button indicators
                break;

            case ElevatorState.Moving:
                DisableInputAction();
                if (!isArrived)
                {
                    ProcessMovement();
                }
                break;
            case ElevatorState.Arrived:
                PlayIdleAnimation();


                ResetPlayerEyeDirection();
                
                break;
            case ElevatorState.CombatPhase:
                EnableInputAction();
                WaitForInput();
                break;
        }
    }

    #region Idle State Methods

    void WaitForInput()
    {
        if (operateToFirstFloor.WasPressedThisFrame())
        {
            selectedFloor = FloorSelected.FirstFloorSelected;
            StopIdleAnimation();
            FirstFloorEyeballRender();
            isMoveRequested = true;
        }
        else if (operateToSecondFloor.WasPressedThisFrame())
        {
            selectedFloor = FloorSelected.SecondFloorSelected;
            StopIdleAnimation();
            SecondFloorEyeballRender();
            isMoveRequested = true;   
        }
        else if (operateToThirdFloor.WasPressedThisFrame())
        {
            selectedFloor = FloorSelected.ThirdFloorSelected;
            StopIdleAnimation();
            ThirdFloorEyeballRender();
            isMoveRequested = true;
        }
        else if (operateToFourthFloor.WasPressedThisFrame())
        {
            selectedFloor = FloorSelected.FourthFloorSelected;
            StopIdleAnimation();
            FourthFloorEyeballRender();
            isMoveRequested = true;
        }
    }

    private void PlayIdleAnimation()
    {
        if (!cartIdleIsPlaying)
        {
            if (animator != null)
            {
                ResetAllAnimTriggers();
                animator.SetTrigger(idleParam);
                cartIdleIsPlaying = true;
                Debug.Log("Playing Idle Animation");
            }
        }
    }

    private void StopIdleAnimation()
    {
        cartIdleIsPlaying = false;
        ResetAllAnimTriggers();
    }

    private IEnumerator StartIdleState(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentState = ElevatorState.Idle;
    }

    #endregion

    #region Moving State Methods

    void ProcessMovement()
    {
        switch (selectedFloor)
        {
            case FloorSelected.None:
                Debug.Log("No floor selected.");
                break;
            case FloorSelected.FirstFloorSelected:
                currentTarget = firstFloorLocation;
                FirstFloorEyeballRender();
                MoveCartTowardsTarget();
                break;
            case FloorSelected.SecondFloorSelected:
                currentTarget = secondFloorLocation;
                SecondFloorEyeballRender();
                MoveCartTowardsTarget();
                break;
            case FloorSelected.ThirdFloorSelected:
                currentTarget = thirdFloorLocation;
                ThirdFloorEyeballRender();
                MoveCartTowardsTarget();
                break;
            case FloorSelected.FourthFloorSelected:
                currentTarget = fourthFloorLocation;
                FourthFloorEyeballRender();
                MoveCartTowardsTarget();
                break;
        }

        isMoving = true;
    }

    private void FourthFloorEyeballRender()
    {
        fourthFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
        thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
        secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
        firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
    }

    private void ThirdFloorEyeballRender()
    {
        thirdFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
        secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
        firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
        fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
    }

    private void SecondFloorEyeballRender()
    {
        secondFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
        firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
        thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
        fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
    }

    private void FirstFloorEyeballRender()
    {
        firstFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
        secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
        thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
        fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
    }


    private void MoveCartTowardsTarget()
    {
        if (isMoving)
        {
            if (!cartMovingIsPlaying)
            {
                ResetAllAnimTriggers();
                animator.SetTrigger(movingParam);
                cartMovingIsPlaying = true;
            }

            transform.position = Vector3.MoveTowards(transform.position, currentTarget, cartSpeed * Time.deltaTime);
            ProcessEyeballFloorSelect(); 

            if ((transform.position - currentTarget).sqrMagnitude <= arriveThreshold * arriveThreshold)
            {
                transform.position = currentTarget;
                isMoving = false;

                firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;

                ArrivedAtDestination(currentTarget);
            }
        }
    }

    void ArrivedAtDestination(Vector3 target)
    {
        if (target == firstFloorLocation)
        {
            arrivedFloor = Floor.First;
        }
        else if (target == secondFloorLocation)
        {
            arrivedFloor = Floor.Second;
        }
        else if (target == thirdFloorLocation)
        {
            arrivedFloor = Floor.Third;
        }
        else if (target == fourthFloorLocation)
        {
            arrivedFloor = Floor.Fourth;
        }
        else
        {
            arrivedFloor = Floor.None;
        }

        cartMovingIsPlaying = false;
        cartIdleIsPlaying = true;
        isArrived = true;
        
    }

   private void ResetAllAnimTriggers()
    {
        animator.ResetTrigger(idleParam);
        animator.ResetTrigger(movingParam);
        animator.ResetTrigger(emptyParam);

        cartIdleIsPlaying = false;
        cartMovingIsPlaying = false;
    }

    void ProcessEyeballFloorSelect()
    {
        if (isMoving)
        {
            if (transform.position.y < currentTarget.y)
            {
                // Make eyes look up
                leftEyeBall.transform.rotation = Quaternion.Euler(-40, 0, 0);
                rightEyeBall.transform.rotation = Quaternion.Euler(0, 0, 50);
            }
            else if (transform.position.y > currentTarget.y)
            {
                // Make eyes look down
                leftEyeBall.transform.rotation = Quaternion.Euler(30, 0, 0);
                rightEyeBall.transform.rotation = Quaternion.Euler(0, 0, -60);
            }
        }
        else
        {
            leftEyeBall.transform.rotation = Quaternion.Euler(0, 0, 0);
            rightEyeBall.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    #endregion

    #region Arrived State Methods

    void ResetPlayerEyeDirection()
    {
        leftEyeBall.transform.rotation = Quaternion.Euler(0, 0, 0);
        rightEyeBall.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    #endregion

    #region State Transition Handlers

    public void GoIdle()
    {
        currentState = ElevatorState.Idle;
    }

    public void PrepareForMovement()
    {
        currentState = ElevatorState.PreMovement;
    }

    public void StartMoving()
    {
        currentState = ElevatorState.Moving;
    }

    public void HandleArrival()
    {
        currentState = ElevatorState.Arrived;
        isMoveRequested = false;
        isArrived = false;
        ResetAllAnimTriggers();
    }

    public void EnterCombat()
    {
        currentState = ElevatorState.CombatPhase;
    }

    #endregion

    public void OnEnemyHitElevator()
    {
        StartCoroutine(StartIdleState(3f));
    }

    #region Debug

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 120, 200, 20), "Cart State: " + currentState);
    }

    #endregion
}