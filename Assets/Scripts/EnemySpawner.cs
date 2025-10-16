using UnityEngine;
using static ElevatorCartMover;

public class EnemySpawner : MonoBehaviour
{
    public ElevatorCartMover elevatorCartMover;

    [SerializeField] GameObject enemy;

    Vector3 firstFloorEnemySpawn = new Vector3(0, 0, -10f);
    Vector3 secondFloorEnemySpawn = new Vector3(0, 11f, -10f);
    Vector3 thirdFloorEnemySpawn = new Vector3(0, 21f, -10f);
    Vector3 fourthFloorEnemySpawn = new Vector3(0, 31f, -10f);

    [SerializeField] Transform firstFloorSpawnPos;
    [SerializeField] Transform secondFloorSpawnPos;
    [SerializeField] Transform thirdFloorSpawnPos;
    [SerializeField] Transform fourthFloorSpawnPos;

    [SerializeField] private float spawnRate = 5f;

    public bool enemySpawned = false;
    private float spawnTimer = 999f;

    void Start()
    {
        
    }

    void Update()
    {
        ProcessEnemySpawning();
    }

    private void ProcessEnemySpawning()
    {
        if (elevatorCartMover.currentState == ElevatorState.CombatPhase)
        {
            Vector3 spawnPos = firstFloorEnemySpawn; // default

            switch (elevatorCartMover.arrivedFloor)
            {
                case Floor.First:
                    spawnPos = firstFloorEnemySpawn;
                    SpawnEnemy(firstFloorSpawnPos);
                    break;
                case Floor.Second:
                    spawnPos = secondFloorEnemySpawn;
                    SpawnEnemy(secondFloorSpawnPos);
                    break;
                case Floor.Third:
                    spawnPos = thirdFloorEnemySpawn;
                    SpawnEnemy(thirdFloorSpawnPos);
                    break;
                case Floor.Fourth:
                    spawnPos = fourthFloorEnemySpawn;
                    SpawnEnemy(fourthFloorSpawnPos);
                    break;
            }
        }

        if (elevatorCartMover.currentState != ElevatorState.CombatPhase)
        {
            enemySpawned = false;
        }
    }

    private void SpawnEnemy(Transform spawnPosition)
    {
        // Update timer
        spawnTimer += Time.deltaTime;

        // Wait until enough time has passed
        if (spawnTimer < spawnRate)
            return;

        // Reset timer
        spawnTimer = 0f;

        var go = Instantiate(enemy, spawnPosition.position, Quaternion.identity);
    }
}
