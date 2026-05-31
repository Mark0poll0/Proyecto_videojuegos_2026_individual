using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 1. CRÍTICO: Debe heredar de MonoBehaviour para poder usarse como componente en Unity
public class BattleVisuals : MonoBehaviour
{
    [SerializeField] private Slider healthbar;
    [SerializeField] private TextMeshProUGUI LevelText;

    private int currentHealth;
    private int maxhealth;
    private int level;

    private Animator anim ; 
    private const string LEVEL_ABB = "Lv. ";

    private const string IS_ATTACK_PARAM = "IsAttack";
    private const string IS_HIT_PARAM = "IsHit";
    private const string IS_DEAD_PARAM = "IsDead";
    void Start()
    {
       
        anim = gameObject.GetComponent<Animator>();
  
    }

    public void SetStartingValues(int currentHealth, int maxHealth, int level)
    {
        this.maxhealth = maxHealth;
        this.currentHealth = currentHealth;
        this.level = level;

        LevelText.text = LEVEL_ABB + this.level.ToString(); // Resultado: "Lv. 5"

     
        UpdateHealth();
    }

    public void ChangeHealth(int healthChange)
    {
        // 1. Sumamos el cambio (si es daño, será un número negativo)
        this.currentHealth += healthChange;

        // 2. LÍMITES: Evitamos que la vida baje de 0 o supere el máximo.
        this.currentHealth = Mathf.Clamp(this.currentHealth, 0, this.maxhealth);

        // 3. LÓGICA DE MUERTE - Verificamos si, después del golpe, la vida llegó a 0
        if (this.currentHealth <= 0)
        {
            PlayDeathAnimation();

            // Destruye el objeto (fantasma) después de 1 segundo (1f). , esto por mis 10 frames en mi animacion 
            Destroy(gameObject, 1f);
        }
        else if (healthChange < 0)
        {
            //  Si la vida NO es 0, pero el cambio fue negativo (recibió daño),
    
            PlayHitAnimation();
        }

        UpdateHealth();
    }

    public void UpdateHealth()
    {
        healthbar.maxValue = maxhealth;
        healthbar.value = currentHealth;
    }

    public void PlayAttackAnimation()
    {
        anim.SetTrigger(IS_ATTACK_PARAM);
    }

    public void PlayHitAnimation()
    {
        anim.SetTrigger(IS_HIT_PARAM);
    }

    public void PlayDeathAnimation()
    {
        anim.SetTrigger(IS_DEAD_PARAM);
    }
}