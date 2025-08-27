using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    bool _active = false;

    public void Activate() { _active = true; }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);


        //if (!_active)
        //{
        //    return;
        //} 
        //else
        //{
        //    transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
        //}
    }
}
