using UnityEngine;

// Interfaz para cualquier objeto que pueda realizar un tiro de penal.
public interface IKickable
{
    // Cargar la potencia del tiro.
    float CurrentPower { get; set; }

    // Dirección del tiro 
    int KickDirection { get; set; }
}
