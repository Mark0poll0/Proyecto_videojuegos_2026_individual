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

    [Header("Ficha del Enemigo (ScriptableObject)")]
    [SerializeField] private EnemyInfo informacionBase;
    [SerializeField] private int nivelEnemigo = 1;
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

        // Ignoramos el EnemyManager para las pruebas locales y calculamos todo con el Inspector
        if (this.informacionBase != null)
        {
            float modificadorNivel = 0.5f * this.nivelEnemigo;

            // Calculamos Vida Máxima basada en el nivel del Inspector
            int maxHP = Mathf.RoundToInt(this.informacionBase.baseHP + (this.informacionBase.baseHP * modificadorNivel));
            this.vidaActual = maxHP;

            // Calculamos Fuerza basada en el nivel del Inspector
            this.dañoAtaque = Mathf.RoundToInt(this.informacionBase.baseStr + (this.informacionBase.baseStr * modificadorNivel));

            // Inicializamos la barra de vida flotante con los valores correctos calculados
            BattleVisuals misVisuales = GetComponentInChildren<BattleVisuals>();
            if (misVisuales != null)
            {
                misVisuales.SetStartingValues(this.vidaActual, maxHP, this.nivelEnemigo);
            }

            Debug.Log($"[IA Unificada] {this.informacionBase.EnemyName} inicializado en Overworld. Nivel: {this.nivelEnemigo} | HP: {this.vidaActual} | Daño: {this.dañoAtaque}");
        }
        else
        {
            Debug.LogError($"[IA Error] No tienes asignado el ScriptableObject 'Informacion Base' en el Inspector de {gameObject.name}");
        }
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
        direccionTotal.y = 0f; // Ignoramos desfases verticales

        float distanciaEnZ = Mathf.Abs(direccionTotal.z);
        Vector3 direccionFiltrada = direccionTotal.normalized;

        // Si ya cumplimos con la alineación del plano en Z, nos concentramos puramente en X
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

        yield return new WaitForSeconds(0.7f);

        this.estaAtacando = false;
    }
    public void RecibirDanioEnemigo(int cantidadDanio)
    {
        if (this.vidaActual <= 0) return;

        this.vidaActual -= cantidadDanio;
        Debug.Log($"[OVERWORLD] {this.statsEnemigo?.EnemyName} recibió {cantidadDanio} de daño. HP restante: {this.vidaActual}");

        BattleVisuals misVisuales = GetComponentInChildren<BattleVisuals>();
        if (misVisuales != null)
        {
            misVisuales.ChangeHealth(-cantidadDanio);
        }

        // Si la vida llega a 0, el cuerpo del enemigo maneja la muerte
        if (this.vidaActual <= 0)
        {
            StartCoroutine(RutinaMuerteCuerpo());
        }
    }

    private IEnumerator RutinaMuerteCuerpo()
    {
        // Bloqueamos la IA y la dejamos quieta en su lugar exacto
        this.estaAtacando = true;
        this.procesandoPersecucion = false;
        this.direccionMovimientoCalculada = Vector3.zero;

        if (this.rb != null)
        {
            this.rb.linearVelocity = Vector3.zero;
            this.rb.isKinematic = true;
        }

        // Disparamos la animación de muerte en el sprite del propio Samurai
        if (this.anim != null)
        {
            this.anim.SetTrigger("IsDead");
        }

        // Esperamos el tiempo necesario para que termine de caer (ej: 1 segundo)
        yield return new WaitForSeconds(1f);

        // Destruimos todo el GameObject del enemigo de la escena
        Destroy(gameObject);
    }

    public void AsignarEstadisticasFisicas(Enemy datosCalculados)
    {
        this.statsEnemigo = datosCalculados;
        this.vidaActual = datosCalculados.currentHP;
        this.dañoAtaque = datosCalculados.Strength;

        Debug.Log($"[SÍNCRONO] {this.statsEnemigo.EnemyName} nivel {this.statsEnemigo.Level} listo en el Overworld con {this.vidaActual} de HP.");
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
