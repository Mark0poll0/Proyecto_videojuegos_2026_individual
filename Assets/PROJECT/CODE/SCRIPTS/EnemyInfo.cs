using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Info")]
public class EnemyInfo : ScriptableObject 
{
    public string EnemyName;
    public int baseHP;
    public int baseSTR;
    public int baseIniciative;
    public GameObject EnemyVisualPrefrab ; 
}
