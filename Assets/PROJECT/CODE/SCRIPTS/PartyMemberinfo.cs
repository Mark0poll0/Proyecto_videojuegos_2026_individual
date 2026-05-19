using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Party/Member Info")]
public class PartyMemberinfo : ScriptableObject
{
    public string MemberName;
    public int StartingLevel;
    public int baseHP;
    public int Str;
    public int Initiative;
    public GameObject MemberBattleVisualPrefab;
    public GameObject MemberOverworldVisualPrefab;


}
