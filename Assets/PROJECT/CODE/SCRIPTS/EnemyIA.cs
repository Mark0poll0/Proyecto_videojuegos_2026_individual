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

    private Transform targetPlayer;
    private Rigidbody rb;
    private bool estaAtacando;

    [Header("Ajuste Visual de Rangos")]
    [SerializeField] private Vector3 centroOffsetRangos = new Vector3(0f, 1f, 0f);

    [Header("Alineación 2.5D")]
    [SerializeField] private float toleranciaAlineacionZ = 0.3f;

    private Vector3 direccionMovimientoCalculada;
    private bool procesandoPersecucion;

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            this.targetPlayer = playerObj.transform;
        }

        this.rb.constraints = RigidbodyConstraints.FreezeRotation;

        Debug.Log($"[IA Activa] Inteligencia de {gameObject.name} inicializada correctamente.");
    }

    private void Update()
    {
        if (this.targetPlayer == null || this.estaAtacando)
        {
            this.direccionMovimientoCalculada = Vector3.zero;
            this.procesandoPersecucion = false;
            return;
        }

        float distanciaEnX = Mathf.Abs(this.targetPlayer.position.x - this.transform.position.x);
        float distanciaEnZ = Mathf.Abs(this.targetPlayer.position.z - this.transform.position.z);
        float distanciaTridimensional = Vector3.Distance(this.transform.position, this.targetPlayer.position);

        bool estaAlineadoEnZ = distanciaEnZ <= this.toleranciaAlineacionZ;

        if (distanciaEnX <= this.rangoAtaque && estaAlineadoEnZ && distanciaTridimensional <= this.rangoDeteccion)
        {
            this.direccionMovimientoCalculada = Vector3.zero;
            this.procesandoPersecucion = false;

            IntentarAtacar();
        }
        else if (distanciaTridimensional <= this.rangoDeteccion)
        {
            this.procesandoPersecucion = true;
            CalcularDireccionPersecucion();
        }
        else
        {
            this.direccionMovimientoCalculada = Vector3.zero;
            this.procesandoPersecucion = false;

            if (this.anim != null) this.anim.SetBool("walk", false);
        }
    }

    private void FixedUpdate()
    {
        if (this.estaAtacando)
        {
            if (this.rb != null && !this.rb.isKinematic)
            {
                this.rb.linearVelocity = Vector3.zero;
                this.rb.angularVelocity = Vector3.zero;
            }
            this.rb.isKinematic = true;
            return;
        }

        if (this.procesandoPersecucion && this.direccionMovimientoCalculada != Vector3.zero)
        {
            if (this.rb.isKinematic) this.rb.isKinematic = false;

            Vector3 nuevaPosicion = this.transform.position + this.direccionMovimientoCalculada * this.velocidadEnemigo * Time.fixedDeltaTime;
            this.rb.MovePosition(nuevaPosicion);
        }
        else
        {
            if (this.rb != null && !this.rb.isKinematic)
            {
                this.rb.linearVelocity = Vector3.zero;
                this.rb.angularVelocity = Vector3.zero;
            }
            this.rb.isKinematic = true;
        }
    }

    private void CalcularDireccionPersecucion()
    {
        Vector3 direccionTotal = (this.targetPlayer.position - this.transform.position);
        direccionTotal.y = 0f;

        float distanciaEnZ = Mathf.Abs(direccionTotal.z);
        Vector3 direccionFiltrada = direccionTotal.normalized;

        if (distanciaEnZ <= this.toleranciaAlineacionZ)
        {
            direccionFiltrada.z = 0f;
        }

        if (direccionTotal.x != 0f && this.enemySprite != null)
        {
            this.enemySprite.flipX = (direccionTotal.x < 0f);
        }

        this.direccionMovimientoCalculada = direccionFiltrada.normalized;

        if (this.anim != null) this.anim.SetBool("walk", this.direccionMovimientoCalculada != Vector3.zero);
    }

    private void IntentarAtacar()
    {
        if (this.anim != null) this.anim.SetBool("walk", false);

        if (Time.time >= this.tiempoSiguienteAtaque && !this.estaAtacando)
        {
            this.tiempoSiguienteAtaque = Time.time + this.cooldownAtaque;
            StartCoroutine(RutinaAtaqueEnemigo());
        }
    }

    private IEnumerator RutinaAtaqueEnemigo()
    {
        this.estaAtacando = true;

        if (this.rb != null)
        {
            if (!this.rb.isKinematic)
            {
                this.rb.linearVelocity = Vector3.zero;
                this.rb.angularVelocity = Vector3.zero;
            }
            this.rb.isKinematic = true;
        }

        if (this.anim != null) this.anim.SetTrigger("IsAttack");

        // Buscamos nuestra propia fuerza local unificada en el script nuevo
        EntidadVida misEstadisticas = GetComponent<EntidadVida>();
        int danioEnemigo = (misEstadisticas != null) ? misEstadisticas.Fuerza : 2;

        if (this.targetPlayer != null)
        {
            EntidadVida vidaProta = this.targetPlayer.GetComponent<EntidadVida>();
            if (vidaProta != null)
            {
                vidaProta.RecibirDanio(danioEnemigo, this.transform.position);
            }
        }

        yield return new WaitForSeconds(0.7f);
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
}