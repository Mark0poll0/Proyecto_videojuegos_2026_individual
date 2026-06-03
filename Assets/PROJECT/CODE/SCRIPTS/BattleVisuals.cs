using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleVisuals : MonoBehaviour
{
    [SerializeField] private Slider healthbar;
    [SerializeField] private TextMeshProUGUI LevelText;

    private int currentHealth;
    private int maxhealth;
    private int level;

    private Animator anim;
    private const string LEVEL_ABB = "Lv. ";

    private const string IS_ATTACK_PARAM = "IsAttack";
    private const string IS_HIT_PARAM = "IsHit";
    private const string IS_DEAD_PARAM = "IsDead";

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();

        // Si te olvidaste de arrastrar el slider en el inspector, 
        if (this.healthbar == null)
        {
            this.healthbar = GetComponentInChildren<Slider>();
        }
    }

    public void SetStartingValues(int currentHealth, int maxHealth, int level)
    {
        this.maxhealth = maxHealth;
        this.currentHealth = currentHealth;
        this.level = level;

        if (LevelText != null)
        {
            LevelText.text = LEVEL_ABB + this.level.ToString();
        }

        UpdateHealth();
    }

    public void ChangeHealth(int healthChange)
    {
        this.currentHealth += healthChange;
        this.currentHealth = Mathf.Clamp(this.currentHealth, 0, this.maxhealth);

        if (this.currentHealth <= 0)
        {
            PlayDeathAnimation();
            Destroy(gameObject, 1f);
        }
        else if (healthChange < 0)
        {
            PlayHitAnimation();
        }

        UpdateHealth();
    }

    public void UpdateHealth()
    {
        if (healthbar != null)
        {
            healthbar.maxValue = maxhealth;
            healthbar.value = currentHealth;
        }
        else
        {
            Debug.LogError($"[BattleVisuals] No hay ningún Slider asignado en {gameObject.name}");
        }
    }

    public void PlayAttackAnimation() { anim.SetTrigger(IS_ATTACK_PARAM); }
    public void PlayHitAnimation() { anim.SetTrigger(IS_HIT_PARAM); }
    public void PlayDeathAnimation() { anim.SetTrigger(IS_DEAD_PARAM); }
}