using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [Serializable]
    public struct CombatUnit
    {
        public float Attack, Defense, CurrentHP, MaxHP;
        public UnitType PrimType, SecType;
        public UnitHolder target;
    }

    public enum UnitType
    {
        NULL,
        KNIGHT_R,
        MAGE_P,
        ASSASSIN, //S for Scissors
    }

    CombatUnit[] approaching;
    CombatUnit[] playersUnits;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void CreateNewEnemies(float Level)
    {
        approaching = new CombatUnit[3];

        for(int i = 0; i < approaching.Length; i++)
        {
            //Note, these ranges should adjust based on level
            approaching[i].Attack = UnityEngine.Random.Range(15, 26);
            approaching[i].Defense = UnityEngine.Random.Range(15, 26);
            approaching[i].MaxHP = UnityEngine.Random.Range(80, 121);
            approaching[i].CurrentHP = approaching[i].MaxHP;
            approaching[i].PrimType = (UnitType)UnityEngine.Random.Range(1, 4);
        }
    }
}
