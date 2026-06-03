using System.Collections;
using UnityEngine;

public class EntidadVida : MonoBehaviour
{
    public int VidaMaxima { get; private set; }
    public int VidaActual { get; private set; }
    public int Fuerza { get; private set; }
    public int Nivel => nivel; // Propiedad pública para que otros scripts lean el nivel actual

    [Header("Configuración Inicial")]
    [SerializeField] private bool esJugador;
    [SerializeField] private int nivel = 1;

    [Header("Sistema de Experiencia (Solo Jugador)")]
    [SerializeField] private int expActual = 0;
    [SerializeField] private int expSiguienteNivel = 100;

    [Header("Fichas de Datos")]
    [SerializeField] private CharacterData datosJugador;
    [SerializeField] private EnemyData datosEnemigo;

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
        // El factor de escala aumenta un 10% por cada nivel por encima del 1
        float factorEscala = 1f + (0.1f * (this.nivel - 1));

        if (this.esJugador && this.datosJugador != null)
        {
            this.VidaMaxima = Mathf.RoundToInt(this.datosJugador.vidaMaxBase * factorEscala);
            this.Fuerza = Mathf.RoundToInt(this.datosJugador.fuerzaBase * factorEscala);

            // Fórmula estándar: la experiencia requerida sube exponencialmente por nivel
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

    // --- SISTEMA DE EXPERIENCIA (Método público para el Prota) ---
    public void GanarExperiencia(int cantidadGanada)
    {
        if (!this.esJugador || this.estaMuerto) return;

        this.expActual += cantidadGanada;
        Debug.Log($"[PROGRESO] ¡Ganaste {cantidadGanada} EXP! Progreso actual: {this.expActual}/{this.expSiguienteNivel}");

        // Bucle por si gana tanta EXP que sube más de un nivel de golpe
        while (this.expActual >= this.expSiguienteNivel)
        {
            EjecutarLevelUp();
        }
    }

    private void EjecutarLevelUp()
    {
        this.expActual -= this.expSiguienteNivel;
        this.nivel++; // Incrementamos el nivel real

        // Recalculamos estadísticas de daño y vida con el nuevo nivel
        InicializarEstadisticas();

        // Curamos completamente al Prota al subir de nivel
        this.VidaActual = this.VidaMaxima;

        Debug.LogWarning($"[¡LEVEL UP!] ¡Felicidades! Has subido al Nivel {this.nivel}. Nueva Vida: {this.VidaMaxima} | Nueva Fuerza: {this.Fuerza}");

        // Opcional: Si tus Visuales flotantes soportan refrescar el texto del nivel, se actualiza automáticamente aquí
        if (this.barraVisual != null)
        {
            this.barraVisual.SetStartingValues(this.VidaActual, this.VidaMaxima, this.nivel);
        }
    }

    // --- SISTEMA DE COMBATE ---
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

    private void Morir()
    {
        this.estaMuerto = true;

        if (this.rb != null)
        {
            this.rb.linearVelocity = Vector3.zero;
            this.rb.isKinematic = true;
        }

        if (this.anim != null) this.anim.SetTrigger("IsDead");

        if (!this.esJugador)
        {
            // ¡Dar recompensa al jugador antes de desaparecer del mapa!
            EntregarRecompensaAlJugador();
            Destroy(gameObject, 1.2f);
        }
    }

    private void EntregarRecompensaAlJugador()
    {
        if (this.datosEnemigo == null) return;

        // Buscamos al Prota en la escena mediante su Tag
        GameObject protaObj = GameObject.FindGameObjectWithTag("Player");
        if (protaObj != null)
        {
            EntidadVida vidaProta = protaObj.GetComponent<EntidadVida>();
            if (vidaProta != null)
            {
                // Escalamos la EXP entregada según el nivel que tenía este enemigo en el mapa
                int expFinal = Mathf.RoundToInt(this.datosEnemigo.experienciaBase * (1f + 0.2f * (this.nivel - 1)));

                // Le inyectamos la experiencia al protagonista
                vidaProta.GanarExperiencia(expFinal);
            }
        }
    }
}