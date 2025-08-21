using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorCartMover : MonoBehaviour
{
    [SerializeField] GameObject elevatorCart;

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
    }

    void ProcessMovement()
    {
        if (operateToFirstFloor.WasPressedThisFrame()) 
        { 
            currentTarget = firstFloor; 
            isMoving = true; 
        }
        else if (operateToSecondFloor.WasPressedThisFrame()) 
        { 
            currentTarget = secondFloor; 
            isMoving = true; 
        }
        else if (operateToThirdFloor.WasPressedThisFrame()) 
        { 
            currentTarget = thirdFloor; 
            isMoving = true; 
        }
        else if (operateToFourthFloor.WasPressedThisFrame()) 
        { 
            currentTarget = fourthFloor; 
            isMoving = true; 
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, cartSpeed * Time.deltaTime);

            if ((transform.position - currentTarget).sqrMagnitude <= arriveThreshold * arriveThreshold)
            {
                transform.position = currentTarget;
                isMoving = false;
            }
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