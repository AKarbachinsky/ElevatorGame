using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
    }
}
