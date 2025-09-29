using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player,
    AI
}

public enum PlayerType
{
    Kicker,
    Keeper
}


[CreateAssetMenu(fileName = "Actor", menuName = "Stats/Actor", order = 0)]
public class ActorStats : ScriptableObject
{
    [SerializeField] private ActorStatsValues _actorStatsValues;

    // Expose values
    public Team Team => _actorStatsValues.Team;
    public PlayerType PlayerType => _actorStatsValues.PlayerType;

    public float Speed => _actorStatsValues.Speed;
    public float Turn => _actorStatsValues.Turn;
    public float KickPowerMin => _actorStatsValues.KickPowerMin;
    public float KickPowerMax => _actorStatsValues.KickPowerMax;
    public float JumpSpeed => _actorStatsValues.JumpSpeed;
}


[System.Serializable]
public struct ActorStatsValues
{
    public Team Team;
    public PlayerType PlayerType;

    // Gameplay stats
    public float Speed;         // movement speed
    public float Turn;          // turning speed
    public float KickPowerMin;  // min kick power
    public float KickPowerMax;  // max kick power
    public float JumpSpeed;     // keeper jump speed
}

