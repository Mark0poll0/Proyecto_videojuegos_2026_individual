using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [SerializeField] private float rotationSpeed = 100f;

    private PlayerControls playerControls;
    private float rotationInput;

    private void Awake()
    {
        // Inicializamos la misma estructura de controles del proyecto
        this.playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        this.playerControls.Enable();
    }

    private void OnDisable()
    {
        this.playerControls.Disable();
    }

    void Update()
    {
        // Leemos el valor del eje balanceado en tiempo real
        // Flecha Izquierda devolverá -1, Flecha Derecha devolverá 1, nada presionado dará 0
        this.rotationInput = this.playerControls.Player.RotateCamera.ReadValue<float>();

        // Si el jugador está presionando alguna de las dos flechas, aplicamos el giro
        if (this.rotationInput != 0f)
        {
            RotatePerspective();
        }
    }

    private void RotatePerspective()
    {
        // Multiplicación explícita: Dirección del eje * Velocidad * Tiempo del frame
        // Invertimos el signo con un menos (-) si queremos que el giro responda de forma natural a la perspectiva
        float angleToRotate = -this.rotationInput * this.rotationSpeed * Time.deltaTime;

        // Rotamos el contenedor padre sobre el eje vertical Y
        this.transform.Rotate(0f, angleToRotate, 0f);
    }
}