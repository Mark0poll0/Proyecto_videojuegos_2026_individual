using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Referencias de Interfaz")]
    [SerializeField] private CanvasGroup canvasGroupPanel;
    [SerializeField] private TextMeshProUGUI textoTutorial;
    [SerializeField] private Button botonCerrarJuego;

    [Header("Configuración del Desvanecimiento")]
    [SerializeField] private float duracionFade = 0.3f;
    [SerializeField] private float retrasoLectura = 1.2f;

   
    private enum FaseTutorial { Camara, Movimiento, Deslizar, Ataque, Finalizado }
    private FaseTutorial faseActual = FaseTutorial.Camara;

    private PlayerControls inputs;
    private bool cambiandoFase = false;

    private void Awake()
    {
        this.inputs = new PlayerControls();
    }

    private void OnEnable()
    {
        this.inputs.Enable();
        this.inputs.Player.Attack.performed += AlAtacar;
    }

    private void OnDisable()
    {
        this.inputs.Player.Attack.performed -= AlAtacar;
        this.inputs.Disable();
    }

    private void Start()
    {
        if (this.textoTutorial != null)
        {
            this.textoTutorial.text = "Gire la cámara con Flecha Izquierda y Flecha Derecha";
        }

        if (this.canvasGroupPanel != null)
        {
            this.canvasGroupPanel.alpha = 1f;
        }
        
        if (this.botonCerrarJuego != null)
        {
            this.botonCerrarJuego.gameObject.SetActive(false);

            this.botonCerrarJuego.onClick.AddListener(CerrarAplicacion);
        }
    }

    private void Update()
    {
        if (this.cambiandoFase || this.faseActual == FaseTutorial.Finalizado) return;

        switch (this.faseActual)
        {
            case FaseTutorial.Camara:
                if (Keyboard.current != null &&
                   (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame))
                {
                    StartCoroutine(TransicionSuaveTexto("Muévase usando las teclas W, A, S, D", FaseTutorial.Movimiento));
                }
                break;

            case FaseTutorial.Movimiento:
                Vector2 inputMov = this.inputs.Player.Move.ReadValue<Vector2>();
                if (inputMov.magnitude > 0.1f)
                {
                    // Al movernos, pasamos a pedir el deslizamiento lateral con Shift
                    StartCoroutine(TransicionSuaveTexto("Presiona Shift para deslizarte a un lado", FaseTutorial.Deslizar));
                }
                break;

            case FaseTutorial.Deslizar:
                // Detecta si se presiona la tecla Shift (ya sea el izquierdo o el derecho)
                if (Keyboard.current != null &&
                   (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame))
                {
                    // Una vez que se desliza, recién le enseñamos a atacar con Espacio
                    StartCoroutine(TransicionSuaveTexto("Golpee usando la barra Espaciadora", FaseTutorial.Ataque));
                }
                break;
        }
    }

    private void AlAtacar(InputAction.CallbackContext context)
    {
        // Solo permitimos avanzar si el jugador ya superó la fase de deslizamiento y está en la de ataque
        if (this.cambiandoFase || this.faseActual != FaseTutorial.Ataque) return;

        StartCoroutine(TransicionSuaveTexto("¡Excelente! Llega a la cima MATA AL JEFE para acabar el tutorial", FaseTutorial.Finalizado, true));
    }

    private IEnumerator TransicionSuaveTexto(string nuevoTexto, FaseTutorial siguienteFase, bool esElFinal = false)
    {
        this.cambiandoFase = true;

        if (this.canvasGroupPanel != null)
        {
            yield return StartCoroutine(RutinaFade(this.canvasGroupPanel.alpha, 0f));
        }

        yield return new WaitForSeconds(this.retrasoLectura);

        if (this.textoTutorial != null)
        {
            this.textoTutorial.text = nuevoTexto;
        }

        this.faseActual = siguienteFase;

        if (this.canvasGroupPanel != null)
        {
            yield return StartCoroutine(RutinaFade(this.canvasGroupPanel.alpha, 1f));
        }

        this.cambiandoFase = false;

        if (esElFinal)
        {
            StartCoroutine(OcultarTutorialDefinitivo());
        }
    }

    private IEnumerator RutinaFade(float inicio, float fin)
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < this.duracionFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            this.canvasGroupPanel.alpha = Mathf.Lerp(inicio, fin, tiempoTranscurrido / this.duracionFade);
            yield return null;
        }

        this.canvasGroupPanel.alpha = fin;
    }

    private IEnumerator OcultarTutorialDefinitivo()
    {
        yield return new WaitForSeconds(2.5f);

        if (this.canvasGroupPanel != null)
        {
            yield return StartCoroutine(RutinaFade(this.canvasGroupPanel.alpha, 0f));
            this.canvasGroupPanel.gameObject.SetActive(false);
        }

        this.enabled = false;
    }
    public void MostrarPantallaVictoria()
    {
        if (this.canvasGroupPanel != null && this.textoTutorial != null)
        {
            // Forzamos a que el panel del tutorial aparezca al 100% de opacidad
            this.canvasGroupPanel.gameObject.SetActive(true);
            this.canvasGroupPanel.alpha = 1f;

            // Cambiamos el texto al mensaje de cierre de la demo
            this.textoTutorial.text = "¡VICTORIA!\nHas derrotado al Jefe Samurái y completado el tutorial.";
            if (this.botonCerrarJuego != null) this.botonCerrarJuego.gameObject.SetActive(true);
            // Opcional: Congela los movimientos de fondo para darle un toque dramático de fin
            Time.timeScale = 0.2f;
        }
    }
    public void MostrarPantallaDerrota()
    {
        if (this.canvasGroupPanel != null && this.textoTutorial != null)
        {
            // Forzamos a que el panel aparezca de golpe en la pantalla
            this.canvasGroupPanel.gameObject.SetActive(true);
            this.canvasGroupPanel.alpha = 1f;

            // Cambiamos el texto a un tono de derrota RPG clásico
            this.textoTutorial.text = "<color=red>¡HAS MUERTO!</color>\nLos Samuráis te han derrotado. Fin de la partida.";
            if (this.botonCerrarJuego != null) this.botonCerrarJuego.gameObject.SetActive(true);
            // Congelamos el tiempo del juego por completo para que nadie se mueva
            Time.timeScale = 0f;
        }
    }
    public void CerrarAplicacion()
    {
        Debug.LogWarning("[FIN DEL JUEGO] Saliendo de la aplicación de forma controlada...");

        // Restablecemos el TimeScale por si acaso para futuros comportamientos o transiciones de escena
        Time.timeScale = 1f;

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
