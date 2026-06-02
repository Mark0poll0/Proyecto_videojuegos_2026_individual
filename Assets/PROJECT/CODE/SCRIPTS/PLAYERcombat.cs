using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;

    [Header("Configuración de Hitbox 2.5D")]
    [SerializeField] private Vector3 dimensionesHitbox = new Vector3(1.5f, 2f, 1.5f);
    [SerializeField] private float distanciaOffset = 1.2f;
    [SerializeField] private LayerMask capaEnemigos;

    public bool IsAttacking { get; private set; } = false;

    private PlayerControls playerControls;
    private PartyManager partyManager; // Referencia al gestor de datos de la party

    private const string IS_ATTACK_PARAM = "IsAttack";
    private const float ATTACK_DURATION = 0.7f;

    private void Awake()
    {
        this.playerControls = new PlayerControls();

        if (this.anim == null)
        {
            this.anim = GetComponentInChildren<Animator>();
        }
        if (this.playerSprite == null)
        {
            this.playerSprite = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        // Buscamos el cerebro de datos de la escena
        this.partyManager = FindAnyObjectByType<PartyManager>();
    }

    private void OnEnable()
    {
        this.playerControls.Enable();
        this.playerControls.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        this.playerControls.Player.Attack.performed -= OnAttackPerformed;
        this.playerControls.Disable();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (this.IsAttacking) return;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        this.IsAttacking = true;

        if (this.anim != null)
        {
            this.anim.SetTrigger(IS_ATTACK_PARAM);
        }

        yield return new WaitForSeconds(0.2f);

        DetectarYGolpearEnemigos();

        yield return new WaitForSeconds(ATTACK_DURATION - 0.2f);

        this.IsAttacking = false;
    }

    private void DetectarYGolpearEnemigos()
    {
        // Obtenemos el daño real del líder de la party (Índice 0)
        int dañoReal = 1; // Valor base por si no encuentra el componente
        if (this.partyManager != null && this.partyManager.GetCurrentParty().Count > 0)
        {
            dañoReal = this.partyManager.GetCurrentParty()[0].Str;
        }

        Transform pivot = this.transform.Find("CameraPivot");
        Vector3 direccionPantallaDerecha = pivot != null ? pivot.right : Vector3.right;

        float factorDireccion = (this.playerSprite != null && this.playerSprite.flipX) ? -1f : 1f;
        Vector3 direccionAtaque = direccionPantallaDerecha * factorDireccion;

        Vector3 centroHitbox = this.transform.position + (direccionAtaque * this.distanciaOffset) + Vector3.up;
        Quaternion rotacionHitbox = pivot != null ? pivot.rotation : Quaternion.identity;

        Collider[] enemigosGolpeados = Physics.OverlapBox(centroHitbox, this.dimensionesHitbox / 2f, rotacionHitbox, this.capaEnemigos);

        foreach (Collider col in enemigosGolpeados)
        {
            EnemyIA enemigo = col.GetComponent<EnemyIA>();
            if (enemigo != null)
            {
                enemigo.RecibirDanioEnemigo(dañoReal);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform pivot = this.transform.Find("CameraPivot");
        Vector3 direccionPantallaDerecha = pivot != null ? pivot.right : Vector3.right;

        float factorDireccion = (this.playerSprite != null && this.playerSprite.flipX) ? -1f : 1f;
        Vector3 direccionAtaque = direccionPantallaDerecha * factorDireccion;

        Vector3 centroHitbox = this.transform.position + (direccionAtaque * this.distanciaOffset) + Vector3.up;

        Gizmos.color = Color.red;
        Matrix4x4 matrizOriginal = Gizmos.matrix;

        if (pivot != null)
        {
            Gizmos.matrix = Matrix4x4.TRS(centroHitbox, pivot.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, this.dimensionesHitbox);
        }
        else
        {
            Gizmos.DrawWireCube(centroHitbox, this.dimensionesHitbox);
        }

        Gizmos.matrix = matrizOriginal;
    }
}