using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private Animator anim;

    private PlayerControls playerControls;

    // Propiedad pública que el PlayerController leerá para saber si debe congelar las físicas
    public bool IsAttacking { get; private set; } = false;

    // Constantes explícitas
    private const string IS_ATTACK_PARAM = "IsAttack";
    private const float ATTACK_DURATION = 0.7f; // Duración exacta calculada: 7 frames / 10 samples

    private void Awake()
    {
        this.playerControls = new PlayerControls();

        if (this.anim == null)
        {
            this.anim = GetComponentInChildren<Animator>();
        }
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
        // Si ya está atacando, ignoramos el clic para evitar que rompa el bucle físico
        if (this.IsAttacking) return;

        // Iniciamos la rutina de ataque estático
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        this.IsAttacking = true;

        // Disparar la animación del tajo dorado
        if (this.anim != null)
        {
            this.anim.SetTrigger(IS_ATTACK_PARAM);
        }

        // Suspendemos la ejecución de este bloque de código durante la duración exacta de la animación
        yield return new WaitForSeconds(ATTACK_DURATION);

        // Al terminar el tiempo, liberamos el control de movimiento
        this.IsAttacking = false;
    }
}