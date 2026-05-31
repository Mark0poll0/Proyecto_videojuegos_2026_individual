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

    private const string IS_WALK_PARAM = "Walk";
    private const string BattleScene = "BattleScene";
    private const float timePerStep = 0.5f;

    private void Awake()
    {
        this.playerControls = new PlayerControls();
        CalculateStepsNextEncounter();
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
            // De esta manera, el "Frente" (+Z) del Rigidbody se alineará magnéticamente con el frente de la cámara.
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

    private void CalculateStepsNextEncounter()
    {
        this.stepsToEncounter = Random.Range(this.minStepsToEncounter, this.maxStepsToEncounter);
    }
}