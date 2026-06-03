using UnityEngine;

[CreateAssetMenu(fileName = "NuevoPersonaje", menuName = "RPG/Nuevo Personaje")]
public class CharacterData : ScriptableObject
{
    public string nombre;
    public int vidaMaxBase;
    public int fuerzaBase;
}