using UnityEngine;
using TMPro;
using System;

public class ContadorRendimiento : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private TextMeshProUGUI textoFPS;
    [SerializeField] private TextMeshProUGUI textoFrameMs;
    [SerializeField] private TextMeshProUGUI textoTiempoRender;
    [SerializeField] private TextMeshProUGUI textoMemoriaRAM;

    [Header("Configuración")]
    [SerializeField] private float intervaloActualizacion = 0.5f;

    private float acumuladorTiempo = 0f;
    private int acumuladorFrames = 0;
    private float tiempoSiguienteActualizacion;

    void Start()
    {
        this.tiempoSiguienteActualizacion = Time.realtimeSinceStartup + this.intervaloActualizacion;
    }

    void Update()
    {
        this.acumuladorFrames++;
        this.acumuladorTiempo += Time.unscaledDeltaTime;

        
        if (Time.realtimeSinceStartup >= this.tiempoSiguienteActualizacion)
        {
            
            float fps = this.acumuladorFrames / this.acumuladorTiempo;
            if (this.textoFPS != null)
                this.textoFPS.text = $"FPS: {fps:F1}";

           
            float msPerFrame = (this.acumuladorTiempo / this.acumuladorFrames) * 1000f;
            if (this.textoFrameMs != null)
                this.textoFrameMs.text = $"Frame Time: {msPerFrame:F2} ms";

           
            float renderTime = Time.deltaTime * 1000f;
            if (this.textoTiempoRender != null)
                this.textoTiempoRender.text = $"Render Clock: {renderTime:F2} ms";

          
            long memoriaUsadaBytes = GC.GetTotalMemory(false);
            

            float memoriaEnMB = memoriaUsadaBytes / (1024f * 1024f);
            if (this.textoMemoriaRAM != null)
                this.textoMemoriaRAM.text = $"RAM: {memoriaEnMB:F1} MB";

            // Reseteamos contadores para el siguiente intervalo
            this.acumuladorFrames = 0;
            this.acumuladorTiempo = 0f;
            this.tiempoSiguienteActualizacion = Time.realtimeSinceStartup + this.intervaloActualizacion;
        }
    }
}