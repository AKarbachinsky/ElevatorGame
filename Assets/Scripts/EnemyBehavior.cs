using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] ElevatorDoorMover elevatorDoorMover;

    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "ElevatorDoor":
                Destroy(gameObject);
                break;
        }
    }
}
