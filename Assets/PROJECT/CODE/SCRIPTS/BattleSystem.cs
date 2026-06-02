using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private PartyManager partyManager;
    private EnemyManager enemyManager;

    [Header("Entidades presentes en la escena")]
    [SerializeField] private List<BattleEntities> playerEntities = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyEntities = new List<BattleEntities>();

    void Start()
    {
        partyManager = FindAnyObjectByType<PartyManager>();
        enemyManager = FindAnyObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
    }

    private void CreatePartyEntities()
    {
        List<PartyMember> currentParty = partyManager.GetCurrentParty();

        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities newEntity = new BattleEntities();
            newEntity.SetEntityValues(
                currentParty[i].MemberName,
                currentParty[i].currentHP,
                currentParty[i].maxHP,
                currentParty[i].Str,
                currentParty[i].Level,
                true
            );
            playerEntities.Add(newEntity);
        }
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = enemyManager.GetCurrentEnemies();
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(
                currentEnemies[i].EnemyName,
                currentEnemies[i].currentHP,
                currentEnemies[i].maxHP,
                currentEnemies[i].Strength,
                currentEnemies[i].Level,
                false
            );
            enemyEntities.Add(tempEntity);
        }
    }
}

[System.Serializable]
public class BattleEntities
{
    public string Name;
    public int currentHealth;
    public int maxHealth;
    public int Strength;
    public int level;
    public bool IsPlayer;

    public void SetEntityValues(string name, int currentHealth, int maxHealth, int strength, int level, bool isPlayer)
    {
        this.Name = name;
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
        this.Strength = strength;
        this.level = level;
        this.IsPlayer = isPlayer;
    }
}