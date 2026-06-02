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
        newPartyMember.Str = memberInfo.Str;
        newPartyMember.Initiative = memberInfo.Initiative;
        newPartyMember.MemberBattleVisualPrefab = memberInfo.MemberBattleVisualPrefab;
        newPartyMember.MemberOverWorldVisualPrefab = memberInfo.MemberOverworldVisualPrefab;

        currentParty.Add(newPartyMember);
    }
    // ⚔️ MÉTODO NUEVO: Recibir daño en tiempo real en el Overworld
    // ⚔️ MÉTODO ACTUALIZADO: Sin caracteres especiales para evitar errores de compilador
    public void RecibirDanioParty(int indexMiembro, int cantidadDaño)
    {
        if (currentParty == null || currentParty.Count <= indexMiembro) return;

        // Restamos el daño al HP actual del miembro de la party
        currentParty[indexMiembro].currentHP -= cantidadDaño;

        // Blindamos para que la vida no baje de cero
        if (currentParty[indexMiembro].currentHP < 0)
        {
            currentParty[indexMiembro].currentHP = 0;
        }

        Debug.Log($"[PARTY] {currentParty[indexMiembro].MemberName} recibió {cantidadDaño} de daño. HP restante: {currentParty[indexMiembro].currentHP}/{currentParty[indexMiembro].maxHP}");
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
        public int Initiative;
        public int CurrExp;
        public int MaxExp;
        public GameObject MemberBattleVisualPrefab;
        public GameObject MemberOverWorldVisualPrefab;
    }
