using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{

    [SerializeField] private PartyMemberinfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty;
    [SerializeField] private PartyMemberinfo defaultPartyMember;

    private void Awake()
    {
        // 🌟 CORRECCIÓN: Si hay un miembro por defecto asignado, lo añadimos directamente
        if (defaultPartyMember != null)
        {
            AddMemberToParty(defaultPartyMember);
        }
    }

    // Cambiamos el método para que reciba la ficha técnica completa directamente
    public void AddMemberToParty(PartyMemberinfo memberInfo)
    {
        PartyMember newPartyMember = new PartyMember();
        newPartyMember.MemberName = memberInfo.MemberName;
        newPartyMember.Level = memberInfo.StartingLevel;
        newPartyMember.currentHP = memberInfo.baseHP;
        newPartyMember.maxHP = memberInfo.baseHP; // Usamos el baseHP directo
        newPartyMember.Strength = memberInfo.baseSTR;
        newPartyMember.Initiative = memberInfo.baseIniciative;
        newPartyMember.MemberBattleVisualPrefab = memberInfo.MemberBattleVisualPrefab;
        newPartyMember.MemberOverWorldVisualPrefab = memberInfo.MemberOverworldVisualPrefab;

        currentParty.Add(newPartyMember);
    }

    [System.Serializable]
    public class PartyMember
    {
        public string MemberName;
        public int Level;
        public int currentHP;
        public int maxHP;
        public int Strength;
        public int Initiative;
        public int CurrExp;
        public int MaxExp;
        public GameObject MemberBattleVisualPrefab;
        public GameObject MemberOverWorldVisualPrefab;
    }
}