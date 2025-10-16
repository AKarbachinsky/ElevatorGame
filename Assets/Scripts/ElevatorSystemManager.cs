using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class ElevatorSystemManager : MonoBehaviour
{
    [SerializeField] private ElevatorCartMover cart;
    [SerializeField] private ElevatorDoorMover doors;

    public enum GameState
    {
        Idle,
        PreMovement,
        Moving,
        Arrived,
        Combat
    }

    public GameState currentState = GameState.Idle;

    private void Start()
    {
        SetState(GameState.Idle);
    }

    private void Update()
    {
        if (cart.isMoveRequested && doors.isIdlePlaying)
        {
            SetState(GameState.PreMovement);
        }

        if (doors.isDoorsClosed && cart.isMoveRequested)
        {
            SetState(GameState.Moving);
        }

        if (cart.isArrived)
        {
            SetState(GameState.Arrived);
        }

        if (doors.isDoorOpen && !doors.isDoorsMoving && cart.cartIdleIsPlaying && currentState == GameState.Arrived)
        {
            SetState(GameState.Combat);
        }

        if (currentState == GameState.Combat && cart.isMoveRequested)
        {
            SetState(GameState.PreMovement);
        }
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.Idle:
                cart.GoIdle();
                doors.GoIdle();
                break;

            case GameState.PreMovement:
                cart.PrepareForMovement();
                doors.PrepareForMovement();
                break;

            case GameState.Moving:
                cart.StartMoving();
                doors.StartMoving();
                break;

            case GameState.Arrived:
                cart.HandleArrival();
                doors.HandleArrival();
                break;

            case GameState.Combat:
                cart.EnterCombat();
                doors.EnterCombat();
                break;
        }
    }

    public void OnEnemyHit()
    {
       
        SetState(GameState.Idle);
    }

    #region Debug

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 80, 200, 20), "Game State: " + currentState);
    }

    #endregion
}
