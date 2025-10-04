using UnityEngine;
using System.Collections;

public class EyeController : MonoBehaviour
{
    [SerializeField] Transform eyeBone;      // drag in the eye bone
    [SerializeField] float turnSpeed = 5f;   // how quickly to rotate
    [SerializeField] float holdMin = 1f;     // min seconds to stare
    [SerializeField] float holdMax = 3f;     // max seconds to stare

    private Quaternion defaultRotation;
    private Quaternion targetRotation;

    void Start()
    {
        defaultRotation = eyeBone.localRotation;
        targetRotation = defaultRotation;
        StartCoroutine(RandomGazeRoutine());
    }

    void Update()
    {
        // Smoothly rotate eye toward target
        eyeBone.localRotation = Quaternion.Slerp(eyeBone.localRotation,targetRotation,Time.deltaTime * turnSpeed);
    }

    IEnumerator RandomGazeRoutine()
    {
        while (true)
        {
            // Wait random time before switching direction
            yield return new WaitForSeconds(Random.Range(holdMin, holdMax));

            // Pick one of 4 directions (up, down, left, right)
            int dir = Random.Range(0, 4);
            switch (dir)
            {
                case 0: targetRotation = Quaternion.Euler(0, -30, 0) * defaultRotation; break; // left
                case 1: targetRotation = Quaternion.Euler(0, 30, 0) * defaultRotation; break;  // right
                case 2: targetRotation = Quaternion.Euler(-20, 0, 0) * defaultRotation; break; // up
                case 3: targetRotation = Quaternion.Euler(20, 0, 0) * defaultRotation; break;  // down
            }
        }
    }

    public void ResetGaze()
    {
        targetRotation = defaultRotation;
    }
}
