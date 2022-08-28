using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "Metroidvania/PlayerMovementStats")]
public class PlayerMovementStatsSO : ScriptableObject
{
    public float Speed = 10f;
    public float SprintSpeed = 10f;
    public float TurnSmoothTime = 0.1f;
    public float JumpHeight = 1f;
    public float Gravity = -9.81f;
    public bool AllowMidAirTurning = false;
}
