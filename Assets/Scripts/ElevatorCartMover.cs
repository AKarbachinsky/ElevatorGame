using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorCartMover : MonoBehaviour
{
    [SerializeField] GameObject elevatorCart;

    [SerializeField] GameObject firstFloorButton;
    [SerializeField] GameObject secondFloorButton;
    [SerializeField] GameObject thirdFloorButton;
    [SerializeField] GameObject fourthFloorButton;

    [SerializeField] GameObject upArrow;
    [SerializeField] GameObject downArrow;

    [SerializeField] Material floorSelectedMaterial;
    [SerializeField] Material floorUnselectedMaterial;

    [SerializeField] Material arrowActiveMaterial;
    [SerializeField] Material arrowInactiveMaterial;

    [SerializeField] InputAction cartOperate;

    [SerializeField] InputAction operateToFirstFloor;
    [SerializeField] InputAction operateToSecondFloor;
    [SerializeField] InputAction operateToThirdFloor;
    [SerializeField] InputAction operateToFourthFloor;

    [SerializeField] float cartSpeed = 100f;

    [SerializeField] float arriveThreshold = 0.01f;

    Vector3 firstFloor = new Vector3(0, 0, 0);
    Vector3 secondFloor = new Vector3(0, 10, 0);
    Vector3 thirdFloor = new Vector3(0, 20, 0);
    Vector3 fourthFloor = new Vector3(0, 30, 0);

    Vector3 currentTarget;

    bool isMoving = false;

    void Start()
    {
        cartOperate.Enable();
        operateToFirstFloor.Enable();
        operateToSecondFloor.Enable();
        operateToThirdFloor.Enable();
        operateToFourthFloor.Enable();

        currentTarget = transform.position;
    }
    
    void Update()
    {
        ProcessMovement();
        ProcessArrows();


    }

    void ProcessMovement()
    {
        if (operateToFirstFloor.WasPressedThisFrame()) 
        { 
            currentTarget = firstFloor;
            firstFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
            secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            isMoving = true; 
        }
        else if (operateToSecondFloor.WasPressedThisFrame()) 
        { 
            currentTarget = secondFloor; 
            secondFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
            firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            isMoving = true; 
        }
        else if (operateToThirdFloor.WasPressedThisFrame()) 
        { 
            currentTarget = thirdFloor; 
            thirdFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
            secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            isMoving = true; 
        }
        else if (operateToFourthFloor.WasPressedThisFrame()) 
        { 
            currentTarget = fourthFloor; 
            fourthFloorButton.GetComponent<Renderer>().material = floorSelectedMaterial;
            thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            isMoving = true; 
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, cartSpeed * Time.deltaTime);

            if ((transform.position - currentTarget).sqrMagnitude <= arriveThreshold * arriveThreshold)
            {
                transform.position = currentTarget;
                isMoving = false;

                firstFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                secondFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                thirdFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
                fourthFloorButton.GetComponent<Renderer>().material = floorUnselectedMaterial;
            }
        }
    }
    void ProcessArrows()
    {
        if (isMoving)
        {
            if (transform.position.y < currentTarget.y)
            {
                upArrow.GetComponent<Renderer>().material = arrowActiveMaterial;
                downArrow.GetComponent<Renderer>().material = arrowInactiveMaterial;
            }
            else if (transform.position.y > currentTarget.y)
            {
                upArrow.GetComponent<Renderer>().material = arrowInactiveMaterial;
                downArrow.GetComponent<Renderer>().material = arrowActiveMaterial;
            }
        }
        else
        {
            upArrow.GetComponent<Renderer>().material = arrowInactiveMaterial;
            downArrow.GetComponent<Renderer>().material = arrowInactiveMaterial;
        }
    }
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