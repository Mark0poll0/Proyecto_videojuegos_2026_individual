using UnityEngine;

[CreateAssetMenu(fileName = "NuevoEnemigo", menuName = "RPG/Nuevo Enemigo")]
public class EnemyData : ScriptableObject
{
    public string nombreEnemigo;
    public int vidaMaxBase;
    public int fuerzaBase;
    [Header("Recompensa")]
    public int experienciaBase = 10;
}