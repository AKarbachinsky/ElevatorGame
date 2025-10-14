using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ElevatorDoor"))
        {
            ElevatorSystemManager manager = FindFirstObjectByType<ElevatorSystemManager>();
            
            if (manager != null)
            {
                manager.OnEnemyHit();
            }

            Destroy(gameObject);
        }
    }
}
