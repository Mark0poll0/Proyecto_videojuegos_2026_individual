using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;

    [Header("Hitbox 2.5D")]
    [SerializeField] private Vector3 dimensionesHitbox = new Vector3(1.5f, 2f, 1.5f);
    [SerializeField] private float distanciaOffset = 1.2f;
    [SerializeField] private LayerMask capaEnemigos;

    private PlayerControls playerControls;
    private EntidadVida misEstadisticas;
    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;
    private void Awake()
    {
        this.playerControls = new PlayerControls();
        this.misEstadisticas = GetComponent<EntidadVida>();

        if (this.anim == null) this.anim = GetComponentInChildren<Animator>();
        if (this.playerSprite == null) this.playerSprite = GetComponentInChildren<SpriteRenderer>();
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
        if (this.isAttacking) return;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        this.isAttacking = true;
        if (this.anim != null) this.anim.SetTrigger("IsAttack");

        yield return new WaitForSeconds(0.2f); // Sincronía del tajo visual
        EjecutarGolpe();

        yield return new WaitForSeconds(0.4f);
        this.isAttacking = false;
    }

    private void EjecutarGolpe()
    {
        int danioCalculado = (this.misEstadisticas != null) ? this.misEstadisticas.Fuerza : 5;

        // Buscamos el pivote para mantener la rotación de la caja, pero calculamos la dirección de forma absoluta
        Transform pivot = this.transform.Find("CameraPivot");

       
        Vector3 dirBase = Vector3.right;

        float factorDireccion = (this.playerSprite != null && this.playerSprite.flipX) ? -1f : 1f;
        Vector3 dirAtaque = dirBase * factorDireccion;

        // El centro de la Hitbox ahora se mantiene fiel al cuerpo del prota, sin importar cómo gires la cámara
        Vector3 centroHitbox = this.transform.position + (dirAtaque * this.distanciaOffset) + Vector3.up;
        Quaternion rotHitbox = pivot != null ? pivot.rotation : Quaternion.identity;

        Collider[] enemigosGolpeados = Physics.OverlapBox(centroHitbox, this.dimensionesHitbox / 2f, rotHitbox, this.capaEnemigos);

        foreach (Collider col in enemigosGolpeados)
        {
            EntidadVida vidaEnemigo = col.GetComponent<EntidadVida>();
            if (vidaEnemigo != null)
            {
                // Pasamos la posición real exacta del Prota para que la IA enemiga calcule su frente correctamente
                vidaEnemigo.RecibirDanio(danioCalculado, this.transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform pivot = this.transform.Find("CameraPivot");
        Vector3 dirBase = Vector3.right;
        float factorDireccion = (this.playerSprite != null && this.playerSprite.flipX) ? -1f : 1f;
        Vector3 centroHitbox = this.transform.position + (dirBase * factorDireccion * this.distanciaOffset) + Vector3.up;

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