using System.Collections;
using UnityEngine;

public class EntidadVida : MonoBehaviour
{
    public int VidaMaxima { get; private set; }
    public int VidaActual { get; private set; }
    public int Fuerza { get; private set; }
    public int Nivel => nivel;

    [Header("Configuración Inicial")]
    [SerializeField] private bool esJugador;
    [SerializeField] private int nivel = 1;

    [Header("Sistema de Experiencia (Solo Jugador)")]
    [SerializeField] private int expActual = 0;
    [SerializeField] private int expSiguienteNivel = 100;

    [Header("Fichas de Datos")]
    [SerializeField] private CharacterData datosJugador;
    [SerializeField] private EnemyData datosEnemigo;

    [Header("Efectos Especiales")]
    [SerializeField] private ParticleSystem particulasLevelUp;

    private BattleVisuals barraVisual;
    private Animator anim;
    private Rigidbody rb;
    private bool estaMuerto = false;
    private bool esInmune = false;

    private void Awake()
    {
        this.anim = GetComponentInChildren<Animator>();
        this.rb = GetComponent<Rigidbody>();
        this.barraVisual = GetComponentInChildren<BattleVisuals>();

        InicializarEstadisticas();
    }

    private void InicializarEstadisticas()
    {
        float factorEscala = 1f + (0.1f * (this.nivel - 1));

        if (this.esJugador && this.datosJugador != null)
        {
            this.VidaMaxima = Mathf.RoundToInt(this.datosJugador.vidaMaxBase * factorEscala);
            this.Fuerza = Mathf.RoundToInt(this.datosJugador.fuerzaBase * factorEscala);
            this.expSiguienteNivel = Mathf.RoundToInt(100 * Mathf.Pow(1.2f, this.nivel - 1));
        }
        else if (!this.esJugador && this.datosEnemigo != null)
        {
            this.VidaMaxima = Mathf.RoundToInt(this.datosEnemigo.vidaMaxBase * factorEscala);
            this.Fuerza = Mathf.RoundToInt(this.datosEnemigo.fuerzaBase * factorEscala);
        }

        this.VidaActual = this.VidaMaxima;

        if (this.barraVisual != null)
        {
            this.barraVisual.SetStartingValues(this.VidaActual, this.VidaMaxima, this.nivel);
        }
    }

    // --- SISTEMA DE EXPERIENCIA ---
    public void GanarExperiencia(int cantidadGanada)
    {
        if (!this.esJugador || this.estaMuerto) return;

        this.expActual += cantidadGanada;
        Debug.Log($"[PROGRESO] ¡Ganaste {cantidadGanada} EXP! Progreso actual: {this.expActual}/{this.expSiguienteNivel}");

        while (this.expActual >= this.expSiguienteNivel)
        {
            EjecutarLevelUp();
        }
    }

    private void EjecutarLevelUp()
    {
        this.expActual -= this.expSiguienteNivel;
        this.nivel++;

        InicializarEstadisticas();
        this.VidaActual = this.VidaMaxima;

        if (this.particulasLevelUp != null)
        {
            this.particulasLevelUp.Stop();
            this.particulasLevelUp.Play();
        }

        Debug.LogWarning($"[¡LEVEL UP!] ¡Felicidades! Has subido al Nivel {this.nivel}. Nueva Vida: {this.VidaMaxima} | Nueva Fuerza: {this.Fuerza}");

        if (this.barraVisual != null)
        {
            this.barraVisual.SetStartingValues(this.VidaActual, this.VidaMaxima, this.nivel);
        }
    }

    // --- SISTEMA DE COMBATE Y DAÑO ---
    public void RecibirDanio(int cantidad, Vector3 puntoImpacto)
    {
        if (this.estaMuerto || this.esInmune) return;

        this.VidaActual -= cantidad;
        this.VidaActual = Mathf.Clamp(this.VidaActual, 0, this.VidaMaxima);

        Debug.Log($"[COMBATE] {gameObject.name} recibió {cantidad} de daño. HP: {this.VidaActual}/{this.VidaMaxima}");

        if (this.barraVisual != null)
        {
            this.barraVisual.ChangeHealth(-cantidad);
        }

        if (this.VidaActual <= 0)
        {
            Morir();
        }
        else
        {
            StartCoroutine(RutinaInmunidadHurt());
        }
    }

    private IEnumerator RutinaInmunidadHurt()
    {
        this.esInmune = true;
        if (this.anim != null) this.anim.SetTrigger("IsHit");

        yield return new WaitForSeconds(0.3f);
        this.esInmune = false;
    }

    // --- GESTIÓN DE MUERTE Y VICTORIA ---
    // --- GESTIÓN DE MUERTE, VICTORIA Y DERROTA ---
    private void Morir()
    {
        this.estaMuerto = true;

        if (this.rb != null)
        {
            this.rb.linearVelocity = Vector3.zero;
            this.rb.isKinematic = true;
        }

        if (this.anim != null) this.anim.SetTrigger("IsDead");

        // --- DETECTOR DE MUERTE DEL JUGADOR (GAME OVER) ---
        if (this.esJugador)
        {
            TerminarJuegoDerrota();
            return; // Cortamos el código aquí porque el prota no se destruye del mapa
        }

        // Si no es el jugador, significa que es un enemigo
        if (!this.esJugador)
        {
            EntregarRecompensaAlJugador();

            // Verificamos si la ficha técnica del enemigo se llama exactamente "JEFE"
            if (this.datosEnemigo != null && this.datosEnemigo.name == "JEFE")
            {
                TerminarJuegoVictoria();
            }
            else
            {
                Destroy(gameObject, 1.2f);
            }
        }
    }

    private void EntregarRecompensaAlJugador()
    {
        if (this.datosEnemigo == null) return;

        GameObject protaObj = GameObject.FindGameObjectWithTag("Player");
        if (protaObj != null)
        {
            EntidadVida vidaProta = protaObj.GetComponent<EntidadVida>();
            if (vidaProta != null)
            {
                int expFinal = Mathf.RoundToInt(this.datosEnemigo.experienciaBase * (1f + 0.2f * (this.nivel - 1)));
                vidaProta.GanarExperiencia(expFinal);
            }
        }
    }
    private void TerminarJuegoVictoria()
    {
        TutorialManager tutorial = FindAnyObjectByType<TutorialManager>();
        if (tutorial != null)
        {
            tutorial.MostrarPantallaVictoria();
        }
        else
        {
            CerrarAplicacionForzado();
        }
    }
    // --- NUEVO: GESTIÓN DE DERROTA ---
    private void TerminarJuegoDerrota()
    {
        TutorialManager tutorial = FindAnyObjectByType<TutorialManager>();
        if (tutorial != null)
        {
            // Usaremos el mismo método de la UI pero pasándole el texto de fracaso
            tutorial.MostrarPantallaDerrota();
        }
        else
        {
            CerrarAplicacionForzado();
        }
    }

    private void CerrarAplicacionForzado()
    {
        Debug.LogWarning("[FIN DEL JUEGO] Aplicación cerrada...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}