using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private PartyManager partyManager;
    private EnemyManager enemyManager;

    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    void Start()
    {
        // Cambiamos "FindFirst" por "FindAny" para cumplir con el estándar de Unity 6
        partyManager = FindAnyObjectByType<PartyManager>();
        enemyManager = FindAnyObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
    }

    private void CreatePartyEntities()
    {
        List<PartyMember> currentParty = new List<PartyMember>();
        currentParty = partyManager.GetCurrentParty();

        for (int i = 0; i < currentParty.Count; i++)
        {
            // Creamos la nueva entidad
            BattleEntities newEntity = new BattleEntities();

            // 2. CORRECCIÓN: Usamos "newEntity" en vez de "tempEntity"
            newEntity.SetEntityValues(
                currentParty[i].MemberName,
                currentParty[i].currentHP,
                currentParty[i].maxHP,
                currentParty[i].Initiative,
                currentParty[i].Str,
                currentParty[i].Level,
                true
            );

            // Agregamos la entidad a las listas
            allBattlers.Add(newEntity);
            playerBattlers.Add(newEntity);
        }
    }
    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = new List<Enemy>();
        currentEnemies = enemyManager.GetCurrentEnemies();
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(
                currentEnemies[i].EnemyName,
                currentEnemies[i].currentHP,
                currentEnemies[i].maxHP,
                currentEnemies[i].Initiative,
                currentEnemies[i].Strength,
                currentEnemies[i].Level,
                false
            );
            allBattlers.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
    }

} 

[System.Serializable]
public class BattleEntities
{
    public string Name;
    public int currentHealth;
    public int maxHealth;
    public int Initiative;
    public int Strength;
    public int level;
    public bool IsPlayer;

    public void SetEntityValues(string name, int currentHealth, int maxHealth, int initiative, int strength, int level, bool isPlayer)
    {
        // 4. CORRECCIÓN: Usamos "this." para indicarle a Unity 
        // cuál es la variable de la clase y cuál es el parámetro recibido.
        this.Name = name;
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
        this.Initiative = initiative;
        this.Strength = strength;
        this.level = level;
        this.IsPlayer = isPlayer;
    }
}