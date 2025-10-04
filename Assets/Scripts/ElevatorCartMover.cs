using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using static ElevatorDoorMover;

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
        Moving,
        Arrived,
        CombatPhase,
    }

    #endregion

    #region Fields

    public ElevatorDoorMover elevatorDoorMover;

    public ElevatorState currentState = ElevatorState.Idle;
    FloorSelected selectedFloor = FloorSelected.None;

    [SerializeField] GameObject elevatorCart;

    [SerializeField] GameObject firstFloorButton;
    [SerializeField] GameObject secondFloorButton;
    [SerializeField] GameObject thirdFloorButton;
    [SerializeField] GameObject fourthFloorButton;

    [SerializeField] Material floorSelectedMaterial;
    [SerializeField] Material floorUnselectedMaterial;

    [SerializeField] GameObject leftEyeBall;
    [SerializeField] GameObject rightEyeBall;

    //[SerializeField] GameObject upArrow; //DELETE
    //[SerializeField] GameObject downArrow; //DELETE

    //[SerializeField] Material arrowActiveMaterial; //DELETE
    //[SerializeField] Material arrowInactiveMaterial; //DELETE

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
    public bool completedDestination = false;

    bool cartIdleIsPlaying = false;
    bool cartMovingIsPlaying = false;

    #endregion

    private void Awake()
    {
        if (!animator) 
            animator = GetComponentInChildren<Animator>(true);
        Debug.Log($"{name}: bound animator = {(animator ? animator.name : "NULL")}");
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
                elevatorDoorMover.isEnemyHit = false;
                break;
            case ElevatorState.Moving:
                ProcessMovement();
                DisableInputAction();
                break;
            case ElevatorState.Arrived:
                ResetPlayerEyeDirection(); //DELETE
                ForceOpenDoors();
                if (elevatorDoorMover.currentDoorFlag == DoorFlags.Opened)
                {
                    currentState = ElevatorState.CombatPhase;
                }
                break;
            case ElevatorState.CombatPhase:
                if (elevatorDoorMover.isEnemyHit == true)
                {
                    StartCoroutine(StartIdleState(3f));
                }
                break;
        }
    }

    private void PlayIdleAnimation()
    {
        if (elevatorDoorMover.currentDoorState == DoorState.Idle && !cartIdleIsPlaying)
        {
            if (animator != null)
            {
                animator.ResetTrigger("TrCartEmpty");
                animator.SetTrigger("TrCartIdle");
                cartIdleIsPlaying = true;
                Debug.Log("Playing Idle Animation");
            }
        }
    }

    private IEnumerator StartIdleState(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentState = ElevatorState.Idle;
    }

    private void StartIdleState()
    {
        currentState = ElevatorState.Idle;
    }

    void WaitForInput()
    {
        if (operateToFirstFloor.WasPressedThisFrame())
        {
            selectedFloor = FloorSelected.FirstFloorSelected;
            cartIdleIsPlaying = false;
            animator.ResetTrigger("TrCartIdle");
           
            currentState = ElevatorState.Moving;
        }
        else if (operateToSecondFloor.WasPressedThisFrame())
        {
            selectedFloor = FloorSelected.SecondFloorSelected;
            cartIdleIsPlaying = false;
            animator.ResetTrigger("TrCartIdle");
            
            currentState = ElevatorState.Moving;
        }
        else if (operateToThirdFloor.WasPressedThisFrame())
        {
            selectedFloor = FloorSelected.ThirdFloorSelected;
            cartIdleIsPlaying = false;
            animator.ResetTrigger("TrCartIdle");
            
            currentState = ElevatorState.Moving;
        }
        else if (operateToFourthFloor.WasPressedThisFrame())
        {
            selectedFloor = FloorSelected.FourthFloorSelected;
            cartIdleIsPlaying = false;
            animator.ResetTrigger("TrCartIdle");
         
            currentState = ElevatorState.Moving;
        }
    }

    void ProcessMovement()
    {
        if (elevatorDoorMover.currentDoorFlag == DoorFlags.Closed)
        {
            switch (selectedFloor)
            {
                case FloorSelected.None:
                    //Do nothing
                    break;
                case FloorSelected.FirstFloorSelected:
                    currentTarget = firstFloorLocation;
                    firstFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
                    secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    MoveCartTowardsTarget();
                    break;
                case FloorSelected.SecondFloorSelected:
                    currentTarget = secondFloorLocation;
                    secondFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
                    firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    MoveCartTowardsTarget();
                    break;
                case FloorSelected.ThirdFloorSelected:
                    currentTarget = thirdFloorLocation;
                    thirdFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
                    secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    MoveCartTowardsTarget();
                    break;
                case FloorSelected.FourthFloorSelected:
                    currentTarget = fourthFloorLocation;
                    fourthFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
                    thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                    MoveCartTowardsTarget();
                    break;
            }

            isMoving = true;
            completedDestination = false;

        }
    }

    private void MoveCartTowardsTarget()
    {
        if (isMoving)
        {
            if (elevatorDoorMover.currentDoorFlag == DoorFlags.Closed)
            {
                if (!cartMovingIsPlaying)
                {
                    animator.SetTrigger("TrCartMoving");
                    animator.ResetTrigger("TrCartIdle");
                    cartMovingIsPlaying = true;
                }
            }
           
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, cartSpeed * Time.deltaTime);
            ProcessArrows(); //DELETE

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
        animator.SetTrigger("TrCartIdle");
        animator.ResetTrigger("TrCartMoving");
        completedDestination = true;
        currentState = ElevatorState.Arrived;
    }

    void ProcessArrows()
    {
        if (isMoving)
        {
            if (transform.position.y < currentTarget.y)
            {
                //upArrow.GetComponent<Renderer>().material = arrowActiveMaterial;
                //downArrow.GetComponent<Renderer>().material = arrowInactiveMaterial;

                // Make eyes look up
                leftEyeBall.transform.rotation = Quaternion.Euler(-40, 0, 0);
                rightEyeBall.transform.rotation = Quaternion.Euler(0, 0, 50);
            }
            else if (transform.position.y > currentTarget.y)
            {
                //upArrow.GetComponent<Renderer>().material = arrowInactiveMaterial;
                //downArrow.GetComponent<Renderer>().material = arrowActiveMaterial;

                // Make eyes look down
                leftEyeBall.transform.rotation = Quaternion.Euler(30, 0, 0);
                rightEyeBall.transform.rotation = Quaternion.Euler(0, 0, -60);
            }
        }
        else
        {
            //upArrow.GetComponent<Renderer>().material = arrowInactiveMaterial;
            //downArrow.GetComponent<Renderer>().material = arrowInactiveMaterial;

            leftEyeBall.transform.rotation = Quaternion.Euler(0, 0, 0);
            rightEyeBall.transform.rotation = Quaternion.Euler(0, 0, 0);

        }
    }

    void DisableInputAction()
    {
        cartOperate.Disable();
        operateToFirstFloor.Disable();
        operateToSecondFloor.Disable();
        operateToThirdFloor.Disable();
        operateToFourthFloor.Disable();
    }

    void ResetPlayerEyeDirection()
    {
        leftEyeBall.transform.rotation = Quaternion.Euler(0, 0, 0);
        rightEyeBall.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void ForceOpenDoors()
    {
        // Placeholder for door opening logic
        // After doors are opened, transition to CombatPhase or ClosingDoors as needed
        Debug.Log("Arrived at " + arrivedFloor.ToString() + ". Doors should open now.");
    }



    #region Debug

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 120, 200, 20), "Cart State: " + currentState);
    }

    #endregion
}

//float cartInput = cartOperate.ReadValue<float>();
//rbElevatorCart.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

//if (cartInput < 0)
//{
//    rbElevatorCart.AddForce(Vector3.up * cartSpeed * Time.fixedDeltaTime);
//    Debug.Log("Moving Elevator Cart Up");
//}
//else
//{
//    rbElevatorCart.AddForce(Vector3.down * cartSpeed * Time.fixedDeltaTime);
//    Debug.Log("Moving Elevator Cart Down");
//}