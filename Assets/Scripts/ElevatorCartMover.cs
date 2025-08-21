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

    Vector3 firstFloor = new Vector3(0, 0, 0);
    Vector3 secondFloor = new Vector3(0, 10, 0);
    Vector3 thirdFloor = new Vector3(0, 20, 0);
    Vector3 fourthFloor = new Vector3(0, 30, 0);

    bool isMoving = false;

    void Start()
    {
        cartOperate.Enable();
        operateToFirstFloor.Enable();
        operateToSecondFloor.Enable();
        operateToThirdFloor.Enable();
        operateToFourthFloor.Enable();
    }
    
    void Update()
    {
        ProcessMovement();
    }

    void ProcessMovement()
    {
       if (operateToFirstFloor.IsPressed())
       {
            MoveToFloor(firstFloor);
            Debug.Log("I'm going to 1st floor!");
       }
       else if (operateToSecondFloor.IsPressed())
       {
            MoveToFloor(secondFloor);
            Debug.Log("I'm going to 2nd floor!");
       }
       else if (operateToThirdFloor.IsPressed())
       {
            MoveToFloor(thirdFloor);
            Debug.Log("I'm going to 3rd floor!");
       }
       else if (operateToFourthFloor.IsPressed())
       {
            MoveToFloor(fourthFloor);
            Debug.Log("I'm going to 4th floor!");
       }
    }

    void MoveToFloor(Vector3 vector3floorPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, vector3floorPosition, cartSpeed * Time.deltaTime);
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