using UnityEngine;

// Interfaz para cualquier arquero que pueda realizar un salto.
public interface IKeepable
{

    // Dirección del salto
    int JumpDirection { get; set; }
}
