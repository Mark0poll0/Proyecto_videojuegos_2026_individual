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

    // Guardamos la referencia de la cámara principal para calcular la dirección en pantalla
    private Transform mainCameraTransform;

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            this.targetPlayer = playerObj.transform;
        }

        if (Camera.main != null)
        {
            this.mainCameraTransform = Camera.main.transform;
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

        this.direccionMovimientoCalculada = direccionFiltrada.normalized;

        // 🌟 SOLUCIÓN AL GIRO DE CÁMARA (FLIP RELATIVO A LA PANTALLA) 🌟
        if (this.enemySprite != null && this.mainCameraTransform != null)
        {
            // Proyectamos el vector que va del enemigo al jugador en el espacio de la cámara.
            // Esto nos dice si el jugador está a la derecha o izquierda en la pantalla actual.
            Vector3 direccionRelativaCamara = this.mainCameraTransform.InverseTransformDirection(direccionTotal);

            // Si el valor X relativo es menor a 0, significa que el jugador se ve a la izquierda de la pantalla
            this.enemySprite.flipX = (direccionRelativaCamara.x < 0f);
        }

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

        // 🌟 ORIENTACIÓN EN EL ATACANTE ANTES DE HACER DAÑO 🌟
        // Nos aseguramos de que el sprite mire al jugador adecuadamente al iniciar el golpe
        if (this.enemySprite != null && this.targetPlayer != null && this.mainCameraTransform != null)
        {
            Vector3 direccionTotal = (this.targetPlayer.position - this.transform.position);
            Vector3 direccionRelativaCamara = this.mainCameraTransform.InverseTransformDirection(direccionTotal);
            this.enemySprite.flipX = (direccionRelativaCamara.x < 0f);
        }

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