using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberinfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty = new List<PartyMember>();
    [SerializeField] private PartyMemberinfo defaultPartyMember;

    private BattleVisuals visualesFlotantesProta;

    private void Awake()
    {
        if (defaultPartyMember != null)
        {
            AddMemberToParty(defaultPartyMember);
        }
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            this.visualesFlotantesProta = playerObj.GetComponentInChildren<BattleVisuals>();

            if (this.visualesFlotantesProta != null && currentParty.Count > 0)
            {
                this.visualesFlotantesProta.SetStartingValues(
                    currentParty[0].currentHP,
                    currentParty[0].maxHP,
                    currentParty[0].Level
                );
            }
        }
    }

    public void AddMemberToParty(PartyMemberinfo memberInfo)
    {
        PartyMember newPartyMember = new PartyMember();
        newPartyMember.MemberName = memberInfo.MemberName;
        newPartyMember.Level = memberInfo.StartingLevel;
        newPartyMember.currentHP = memberInfo.baseHP;
        newPartyMember.maxHP = memberInfo.baseHP;
        newPartyMember.Str = memberInfo.Str;

        currentParty.Add(newPartyMember);
    }

    public void RecibirDanioParty(int indexMiembro, int cantidadDaño)
    {
        if (currentParty == null || currentParty.Count <= indexMiembro) return;

        int vidaAnterior = currentParty[indexMiembro].currentHP;
        currentParty[indexMiembro].currentHP -= cantidadDaño;

        if (currentParty[indexMiembro].currentHP < 0)
        {
            currentParty[indexMiembro].currentHP = 0;
        }

        int cambioReal = currentParty[indexMiembro].currentHP - vidaAnterior;

        if (this.visualesFlotantesProta != null)
        {
            this.visualesFlotantesProta.ChangeHealth(cambioReal);
        }
    }

    public List<PartyMember> GetCurrentParty()
    {
        return currentParty;
    }
}

[System.Serializable]
public class PartyMember
{
    public string MemberName;
    public int Level;
    public int currentHP;
    public int maxHP;
    public int Str;
    public int CurrExp;
    public int MaxExp;
}