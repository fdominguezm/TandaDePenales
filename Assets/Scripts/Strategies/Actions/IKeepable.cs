public interface IKeepable
{
    int JumpDirection { get; set; }
    bool HasJumped { get; set; } // nueva propiedad para controlar si ya saltó
}
