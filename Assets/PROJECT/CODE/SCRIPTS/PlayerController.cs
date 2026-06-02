using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;

    [Header("Configuración del Dash")]
    [SerializeField] private float dashSpeed = 30f;        // Velocidad explosiva del deslizamiento
    [SerializeField] private float dashTime = 0.22f;       // Cuánto dura el impulso (en segundos)
    [SerializeField] private float dashCooldown = 0.8f;    // Tiempo de espera para volver a usarlo

    [Header("Sistema de Pasto (Zona del Jefe)")]
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int minStepsToEncounter;
    [SerializeField] private int maxStepsToEncounter;

    private PlayerControls playerControls;
    private PlayerCombat playerCombat; // Referencia explícita para leer el estado del tajo
    private Rigidbody rb;
    private Vector3 movement;
    private bool movimingInGrass;
    private float stepTimer;
    private int stepsToEncounter;

    // Estados lógicos del Dash
    private bool isDashing;
    private bool canDash = true;

    private const string IS_WALK_PARAM = "Walk";
    private const string BattleScene = "BattleScene";
    private const float timePerStep = 0.5f;

    private void Awake()
    {
        this.playerControls = new PlayerControls();
        CalculateStepsNextEncounter();

        // 🌟 SUSCRIPCIÓN AL INPUT SYSTEM: Vinculamos la nueva acción "Dash" al método de ejecución
        this.playerControls.Player.Dash.performed += ctx => OnDashPerformed();
    }

    private void OnEnable()
    {
        this.playerControls.Enable();
    }

    private void OnDisable()
    {
        this.playerControls.Disable();
    }

    private void Start()
    {
        this.rb = gameObject.GetComponent<Rigidbody>();
        this.playerCombat = gameObject.GetComponent<PlayerCombat>();

        // Blindaje físico para evitar que el sprite rote erráticamente al colisionar
        if (this.rb != null)
        {
            this.rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void Update()
    {
        // 🌟 CANDADO DE DASH: Si se está deslizando, congelamos la actualización de inputs normales
        if (this.isDashing) return;

        // 🌟 COMPROMISO DE COMBATE: Si está atacando, congelamos las lecturas y apagamos la caminata
        if (this.playerCombat != null && this.playerCombat.IsAttacking)
        {
            this.movement = Vector3.zero;
            this.anim.SetBool(IS_WALK_PARAM, false);
            return; // Interrumpimos el flujo del Update de forma síncrona
        }

        float x = this.playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = this.playerControls.Player.Move.ReadValue<Vector2>().y;

        this.movement = new Vector3(x, 0f, z).normalized;

        this.anim.SetBool(IS_WALK_PARAM, this.movement != Vector3.zero);

        if (x != 0f)
        {
            this.playerSprite.flipX = (x < 0f);
        }
    }

    private void FixedUpdate()
    {
        // 🌟 CANDADO FÍSICO DE DASH: Si la corrutina del dash tomó el control del Rigidbody, detenemos el flujo del FixedUpdate
        if (this.isDashing) return;

        // 🌟 CONGELAMIENTO FÍSICO: Si está atacando, clavamos al Rigidbody en el sitio
        if (this.playerCombat != null && this.playerCombat.IsAttacking)
        {
            this.rb.linearVelocity = Vector3.zero;
            this.rb.angularVelocity = Vector3.zero;
            return;
        }

        // Buscamos el objeto CameraPivot que creamos dentro del Player
        Transform pivot = this.transform.Find("CameraPivot");
        Vector3 relativeMovement = this.movement;

        if (pivot != null)
        {
            // 🌟 MATRIZ DE DIRECCIÓN RELATIVA:
            // Tomamos el vector de movimiento plano (x, 0, z) y lo multiplicamos por la rotación del pivote.
            relativeMovement = pivot.TransformDirection(this.movement);
        }

        // Aplicamos el movimiento físico usando el nuevo vector corregido basado en la pantalla
        this.rb.MovePosition(this.transform.position + relativeMovement * this.speed * Time.fixedDeltaTime);

        // Lógica de detección e incremento de pasos para la zona del Jefe (se mantiene igual)
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1f, this.grassLayer);
        this.movimingInGrass = colliders.Length != 0 && this.movement != Vector3.zero;

        if (this.movimingInGrass == true)
        {
            this.stepTimer += Time.fixedDeltaTime;
            if (this.stepTimer > timePerStep)
            {
                this.stepsInGrass++;
                this.stepTimer = 0f;
                if (this.stepsInGrass >= this.stepsToEncounter)
                {
                    SceneManager.LoadScene(BattleScene);
                }
            }
        }
    }

    private void OnDashPerformed()
    {
        // Verificamos si el personaje está habilitado para realizar el dash y si no está atacando
        bool isAttacking = this.playerCombat != null && this.playerCombat.IsAttacking;

        if (!this.canDash || this.isDashing || isAttacking) return;

        StartCoroutine(ExecuteDashRoutine());
    }

    private IEnumerator ExecuteDashRoutine()
    {
        this.canDash = false;
        this.isDashing = true;

        // 🌟 ANIMACIÓN: Disparamos el trigger de tu Animator instantáneamente al arrancar
        if (this.anim != null)
        {
            this.anim.SetTrigger("isDash");
        }

        // Determinamos la dirección del Dash usando tu sistema de dirección relativa de cámara
        Transform pivot = this.transform.Find("CameraPivot");
        Vector3 dashDirection = this.movement;

        if (pivot != null)
        {
            dashDirection = pivot.TransformDirection(this.movement);
        }

        // Si el jugador presiona el botón de Dash estando completamente estático, 
        // lo impulsamos automáticamente hacia adelante basándonos en hacia dónde mire su sprite
        if (dashDirection == Vector3.zero)
        {
            float directionX = this.playerSprite.flipX ? -1f : 1f;
            dashDirection = pivot != null ? pivot.TransformDirection(new Vector3(directionX, 0f, 0f)) : new Vector3(directionX, 0f, 0f);
        }

        dashDirection = dashDirection.normalized;

        float elapsedTime = 0f;

        // 🌟 BUCLE CORREGIDO: Sincronizado estrictamente con el reloj de la física (FixedUpdate)
        // Esto evita que pierda fuerza o recorrido cuando mueves la cámara con las direccionales.
        while (elapsedTime < this.dashTime)
        {
            this.rb.MovePosition(this.transform.position + dashDirection * this.dashSpeed * Time.fixedDeltaTime);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate(); // Forzamos a la corrutina a esperar el pulso de física constante
        }

        this.isDashing = false;

        // Esperamos el tiempo de recarga antes de permitir el siguiente deslizamiento
        yield return new WaitForSeconds(this.dashCooldown);
        this.canDash = true;
    }

    private void CalculateStepsNextEncounter()
    {
        this.stepsToEncounter = Random.Range(this.minStepsToEncounter, this.maxStepsToEncounter);
    }
}