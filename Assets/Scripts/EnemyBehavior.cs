using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
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
