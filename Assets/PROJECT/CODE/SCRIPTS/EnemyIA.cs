using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyIA : MonoBehaviour
{
    [Header("Configuración de Rangos")]
    [SerializeField] private float rangoDeteccion = 8f;
    [SerializeField] private float rangoAtaque = 1.1f;
    [SerializeField] private float velocidadEnemigo = 4f;

    [Header("Configuración de Ataque")]
    [SerializeField] private float cooldownAtaque = 1.5f;
    private float tiempoSiguienteAtaque;

    [Header("Componentes Visuales")]
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Animator anim;

    // Datos estadísticos proporcionales de tu factoría
    private Enemy statsEnemigo;
    private int vidaActual;
    private int dañoAtaque;

    private Transform targetPlayer;
    private Rigidbody rb;
    private bool estaAtacando;

    [Header("Ajuste Visual de Rangos")]
    [SerializeField] private Vector3 centroOffsetRangos = new Vector3(0f, 1f, 0f);

    [Header("Alineación 2.5D")]
    [SerializeField] private float toleranciaAlineacionZ = 0.3f;

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            this.targetPlayer = playerObj.transform;
        }

        this.rb.constraints = RigidbodyConstraints.FreezeRotation;

        EnemyManager manager = FindAnyObjectByType<EnemyManager>();
        if (manager != null && manager.GetCurrentEnemies().Count > 0)
        {
            this.statsEnemigo = manager.GetCurrentEnemies()[0];
            this.vidaActual = this.statsEnemigo.currentHP;
            this.dañoAtaque = this.statsEnemigo.Strength;

            Debug.Log($"[IA] {this.statsEnemigo.EnemyName} cargado físicamente con {this.vidaActual} HP y {this.dañoAtaque} de Daño.");
        }
    }

    private void Update()
    {
        if (this.targetPlayer == null || this.estaAtacando) return;

        float distanciaEnX = Mathf.Abs(this.targetPlayer.position.x - this.transform.position.x);
        float distanciaEnZ = Mathf.Abs(this.targetPlayer.position.z - this.transform.position.z);
        float distanciaTridimensional = Vector3.Distance(this.transform.position, this.targetPlayer.position);

        bool estaAlineadoEnZ = distanciaEnZ <= this.toleranciaAlineacionZ;

        if (distanciaEnX <= this.rangoAtaque && estaAlineadoEnZ && distanciaTridimensional <= this.rangoDeteccion)
        {
            // ⚔️ ESTADO 1: ATACANDO (Muro sólido cinemático)
            if (!this.rb.isKinematic)
            {
                this.rb.linearVelocity = Vector3.zero; // Frenado legal antes del cambio
                this.rb.isKinematic = true;            // Bloqueo total a empujones
            }
            IntentarAtacar();
        }
        else if (distanciaTridimensional <= this.rangoDeteccion)
        {
            // 🏃‍♂️ ESTADO 2: PERSIGUIENDO (Cuerpo dinámico móvil)
            if (this.rb.isKinematic)
            {
                this.rb.isKinematic = false; // Liberamos para que acepte velocidad
            }
            PerseguirPlayer();
        }
        else
        {
            // 🛑 ESTADO 3: IDLE / FUERA DE VISTA
            if (!this.rb.isKinematic)
            {
                this.rb.linearVelocity = Vector3.zero;
                this.rb.isKinematic = true;
            }

            if (this.anim != null) this.anim.SetBool("walk", false);
        }
    }

    private void PerseguirPlayer()
    {
        if (this.rb.isKinematic) return; // Cláusula de seguridad para evitar advertencias

        Vector3 direccionTotal = (this.targetPlayer.position - this.transform.position);
        direccionTotal.y = 0f;

        float distanciaEnZ = Mathf.Abs(direccionTotal.z);
        Vector3 direccionMovimiento = direccionTotal.normalized;

        if (distanciaEnZ <= this.toleranciaAlineacionZ)
        {
            direccionMovimiento.z = 0f;
        }

        if (direccionTotal.x != 0f && this.enemySprite != null)
        {
            this.enemySprite.flipX = (direccionTotal.x < 0f);
        }

        Vector3 velocidadFinal = direccionMovimiento.normalized * this.velocidadEnemigo;
        this.rb.linearVelocity = new Vector3(velocidadFinal.x, this.rb.linearVelocity.y, velocidadFinal.z);

        if (this.anim != null) this.anim.SetBool("walk", true);
    }

    private void IntentarAtacar()
    {
        if (this.anim != null) this.anim.SetBool("walk", false);

        if (Time.time >= this.tiempoSiguienteAtaque)
        {
            StartCoroutine(RutinaAtaqueEnemigo());
            this.tiempoSiguienteAtaque = Time.time + this.cooldownAtaque;
        }
    }

    private IEnumerator RutinaAtaqueEnemigo()
    {
        this.estaAtacando = true;

        if (this.anim != null)
        {
            this.anim.SetTrigger("IsAttack");
        }

        Debug.Log($"¡El {this.statsEnemigo?.EnemyName ?? "Old Samurai"} te ha lanzado un tajo!");

        PartyManager party = FindAnyObjectByType<PartyManager>();
        if (party != null && party.GetCurrentParty().Count > 0)
        {
            party.RecibirDanioParty(0, this.dañoAtaque);
        }

        yield return new WaitForSeconds(0.5f);

        this.estaAtacando = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 posicionCentroReal = this.transform.position + this.centroOffsetRangos;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(posicionCentroReal, this.rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(posicionCentroReal, this.rangoAtaque);
    }

    public void AsignarEstadisticasFisicas(Enemy datosCalculados)
    {
        this.statsEnemigo = datosCalculados;
        this.vidaActual = datosCalculados.currentHP;
        this.dañoAtaque = datosCalculados.Strength;

        Debug.Log($"[SÍNCRONO] {this.statsEnemigo.EnemyName} nivel {this.statsEnemigo.Level} listo en el Overworld con {this.vidaActual} de HP.");
    }
}