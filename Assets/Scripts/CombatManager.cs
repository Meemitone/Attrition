using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Serializable]public struct CombatUnit
    {
        public float Attack, Defense, CurrentHP, MaxHP;
        public UnitType PrimType, SecType;
        public UnitHolder target;
    }

    public enum UnitType
    {
        NULL,
        ROCK,
        PAPER,
        SCISSORS,
    }

    public enum CombatState
    {
        DEFAULT,
        SELECTING,
    }

    public GameObject players;
    public GameObject enemies;
    GameObject source, target;
    CombatState state;
    public float baseDamage = 5;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResolveCombat()
    {
        //if all player units have targets resolve battle
        state = CombatState.DEFAULT;//not selecting now
        UnitHolder[] held = players.GetComponentsInChildren<UnitHolder>();
        for (int i = 0; i < held.Length; i++)
        {
            if(held[i].stats.target == null)
            {
                return;//if any player unit has no target stop
            }
        }
        //else find targets for the enemies
        held = enemies.GetComponentsInChildren<UnitHolder>();
        for(int i = 0; i < held.Length; i++)
        {
            held[i].stats.target = players.transform.GetChild(UnityEngine.Random.Range(Mathf.Max(0,i-1),Mathf.Min(held.Length, i+2))).gameObject.GetComponent<UnitHolder>();
            held[i].targetArrow();
        }
        StartCoroutine(DamageCleanup());
    }

    IEnumerator DamageCleanup()
    {
        yield return new WaitForSeconds(2);

        UnitHolder[] held = players.GetComponentsInChildren<UnitHolder>();
        float damage;

        for (int i = 0; i < held.Length; i++)
        {
            damage = Mathf.Max(held[i].stats.Attack - held[i].stats.target.stats.Defense, 1) * baseDamage;//20 is the base damage here
            if(held[i].GiveType() == (UnitType)((int)held[i].stats.target.GiveType()%3+1))
            {
                damage *= 1.5f;
            }
            if(held[i].stats.target.stats.target!=this)
            {
                damage *= 1.25f;
            }
            held[i].stats.target.stats.CurrentHP -= damage;
        }
        held = enemies.GetComponentsInChildren<UnitHolder>();
        for (int i = 0; i < held.Length; i++)
        {
            damage = Mathf.Max(held[i].stats.Attack - held[i].stats.target.stats.Defense, 1) * baseDamage;//20 is the base damage here
            if (held[i].GiveType() == (UnitType)((int)held[i].stats.target.GiveType() % 3 + 1))
            {
                damage *= 1.5f;
            }
            if (held[i].stats.target.stats.target != this)
            {
                damage *= 1.25f;
            }
            held[i].stats.target.stats.CurrentHP -= damage;
            held[i].stats.target = null;
            held[i].targetArrow();
            held[i].UpdateSelf();
        }
        held = players.GetComponentsInChildren<UnitHolder>();
        for (int i = 0; i < held.Length; i++)
        {
            held[i].stats.target = null;
            held[i].targetArrow();
            held[i].UpdateSelf();
        }
    }

    public void Select(GameObject choice)
    {
        if(state == CombatState.DEFAULT)
        {
            if(choice.transform.parent.gameObject == players)
            {
                source = choice;
                state = CombatState.SELECTING;
            }
        }
        else if(state == CombatState.SELECTING)
        {
            if(choice.transform.parent.gameObject == players)
            {
                source.GetComponent<UnitHolder>().stats.target = null;
                source.GetComponent<UnitHolder>().targetArrow();
                source = null;
                state = CombatState.DEFAULT;
            }
            else if (choice.transform.parent.gameObject == enemies)
            {
                if (Mathf.Abs(choice.transform.GetSiblingIndex() - source.transform.GetSiblingIndex())<2)
                {
                    target = choice;
                    source.GetComponent<UnitHolder>().stats.target = target.GetComponent<UnitHolder>();
                    source.GetComponent<UnitHolder>().targetArrow();
                }
                else
                {
                    source.GetComponent<UnitHolder>().stats.target = null;
                    source = null;
                }
                state = CombatState.DEFAULT;
            }
        }
    }
}
