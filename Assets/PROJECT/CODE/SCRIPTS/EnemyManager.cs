using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyManager : MonoBehaviour
{

    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;
    private const float LEVEL_MODIFIER = 0.5f;

    private void Awake()
    {
        GenerateEnemyByGame("EnemySamurai", 10);
    }
    private void GenerateEnemyByGame(string enemyName, int level)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (enemyName == allEnemies[i].name)
            {
                Enemy newEnemy = new Enemy();
                newEnemy.EnemyName = allEnemies[i].EnemyName;
                newEnemy.Level = level;
                float levelModifier = (LEVEL_MODIFIER * newEnemy.Level);

                newEnemy.maxHP = Mathf.RoundToInt(allEnemies[i].baseHP + (allEnemies[i].baseHP * levelModifier));
                newEnemy.currentHP = newEnemy.maxHP;
                newEnemy.Strength = Mathf.RoundToInt(allEnemies[i].baseStr + (allEnemies[i].baseStr * levelModifier));
                newEnemy.Initiative = Mathf.RoundToInt(allEnemies[i].baseInitiative + (allEnemies[i].baseInitiative * levelModifier));
                newEnemy.EnemyBattleVisualPrefab = allEnemies[i].EnemyVisualPrefab;

                currentEnemies.Add(newEnemy);

            }
        }
    }

}


[System.Serializable]
public class Enemy
{
    public string EnemyName;
    public int Level;
    public int currentHP;
    public int maxHP;
    public int Strength;
    public int Initiative;
    public GameObject EnemyBattleVisualPrefab;
    public GameObject EnemyOverWorldVisualPrefab;
}