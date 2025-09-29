using UnityEngine;

public class AIKickable : MonoBehaviour, IKickable
{
    public float CurrentPower { get; set; }
    public int KickDirection { get; set; }
}
