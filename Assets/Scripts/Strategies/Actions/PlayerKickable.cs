using UnityEngine;

public class PlayerKickable : MonoBehaviour, IKickable
{
    public float CurrentPower { get; set; }
    public int KickDirection { get; set; }
}