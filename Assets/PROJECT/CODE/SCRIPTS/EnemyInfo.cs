using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy")]
public class EnemyInfo : ScriptableObject 
{
    public string EnemyName;
    public int baseHP;
    public int baseStr;
    public int baseInitiative;
    public GameObject EnemyVisualPrefab ; 
}
