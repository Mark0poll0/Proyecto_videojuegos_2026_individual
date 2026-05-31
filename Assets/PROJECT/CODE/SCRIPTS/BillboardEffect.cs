using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        // Buscamos y guardamos explícitamente el Transform de la cámara principal al arrancar
        if (Camera.main != null)
        {
            this.mainCameraTransform = Camera.main.transform;
        }
    }

    // Usamos LateUpdate para asegurarnos de que el sprite se acomode DESPUÉS 
    // de que Cinemachine haya movido y rotado la cámara en ese frame. Evita micro-parpadeos.
    private void LateUpdate()
    {
        if (this.mainCameraTransform != null)
        {
            // Forzamos al gráfico a adoptar exactamente la misma rotación de la cámara.
            // Esto garantiza que el sprite plano siempre mire perpendicular a la pantalla.
            this.transform.rotation = this.mainCameraTransform.rotation;
        }
    }
}