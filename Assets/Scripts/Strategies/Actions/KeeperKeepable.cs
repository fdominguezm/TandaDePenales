using UnityEngine;

public class KeeperKeepable : MonoBehaviour, IKeepable
{
    public int JumpDirection { get; set; } = 0;
    public bool HasJumped { get; set; } = false;
}
