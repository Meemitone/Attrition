using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameDataManager;

public class CombatManager : MonoBehaviour
{
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
    public int pcount, ecount;
    public float gap;
    public float oneSidedTime = 0.5f;
    public float clashTime = 0.25f;

    bool sync = false;

    // Start is called before the first frame update
    void Start()
    {
        pcount = players.transform.childCount;
        ecount = enemies.transform.childCount;
        gap = -players.transform.GetChild(0).localPosition.x;
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
            if (held[i].stats.target == null)
            {
                return;//if any player unit has no target stop
            }
        }
        //else find targets for the enemies
        held = enemies.GetComponentsInChildren<UnitHolder>();
        for (int i = 0; i < held.Length; i++)
        {
            //choose targets randomly from all 3 targets (update to less)
            if (pcount == ecount)
            {
                held[i].stats.target = players.transform.GetChild(UnityEngine.Random.Range(Mathf.Max(0, i - 1), Mathf.Min(held.Length, i + 2))).gameObject.GetComponent<UnitHolder>();
            }
            else if (pcount > ecount)
            {
                held[i].stats.target = players.transform.GetChild(UnityEngine.Random.Range(i, i + 1)).gameObject.GetComponent<UnitHolder>();
            }
            else if (pcount < ecount)
            {
                held[i].stats.target = players.transform.GetChild(UnityEngine.Random.Range(Mathf.Max(i - 1, 0), Mathf.Min(i + 1, pcount))).gameObject.GetComponent<UnitHolder>();//sometimes this will be random(1,2) = 1
            }
            held[i].targetArrow();
        }
        StartCoroutine(DamageCleanup());
    }

    //This will need to be refactored hard, damage needs to happen in sync with movements
    IEnumerator DamageCleanup()
    {
        yield return new WaitForSeconds(2);
        Coroutine[] Movements = new Coroutine[ecount + pcount];
        int cCount = 0;
        bool[] oneSided = new bool[ecount + pcount];
        for (int i = 0; i < oneSided.Length; i++)
        {
            oneSided[i] = false;
        }
        int oCount = 0;//one sided count
        UnitHolder[] held = players.GetComponentsInChildren<UnitHolder>();
        float damage;
        sync = false;
        for (int i = 0; i < held.Length; i++)
        {
            damage = Mathf.Max(held[i].stats.Attack - held[i].stats.target.stats.Defense, 1) * baseDamage;
            if (held[i].GiveType() == (UnitType)((int)held[i].stats.target.GiveType() % 3 + 1))
            {
                damage *= 1.5f;
            }
            if (held[i].stats.target.stats.target != held[i])
            {
                damage *= 1.25f;
                Movements[cCount] = StartCoroutine(held[i].gameObject.GetComponent<moveCoroutine>().FullMotion(held[i].stats.target.gameObject.transform.position, oneSidedTime, Quaternion.Euler(0, 0, held[i].arrowSprite.transform.rotation.eulerAngles.z - 90), oneSidedTime, (oneSidedTime*oCount + oneSidedTime*oCount)*2));
                oneSided[cCount] = true;
                cCount++;
                StartCoroutine(DamageAfterTime(damage, held[i].stats.target, oneSidedTime + oneSidedTime + (oneSidedTime * oCount + oneSidedTime * oCount) * 2, false)); //time to rotate + time to bonk (in this case oneSidedTime on both)
                oCount++;
            }
            else
            {
                Movements[cCount] = StartCoroutine(DelayMotion(held[i], -90));
                cCount++;
                StartCoroutine(DamageAfterTime(damage, held[i].stats.target, clashTime + clashTime, true)); //time to rotate + time to bonk (in this case clashTime on both) + time for onesided attacks in full
            }
        }
        held = enemies.GetComponentsInChildren<UnitHolder>();
        for (int i = 0; i < held.Length; i++)
        {
            damage = Mathf.Max(held[i].stats.Attack - held[i].stats.target.stats.Defense, 1) * baseDamage;
            if (held[i].GiveType() == (UnitType)((int)held[i].stats.target.GiveType() % 3 + 1))
            {
                damage *= 1.5f;
            }
            if (held[i].stats.target.stats.target != held[i])
            {
                damage *= 1.25f;
                Movements[cCount] = StartCoroutine(held[i].gameObject.GetComponent<moveCoroutine>().FullMotion(held[i].stats.target.gameObject.transform.position, oneSidedTime, Quaternion.Euler(0, 0, held[i].arrowSprite.transform.rotation.eulerAngles.z + 90), oneSidedTime, (oneSidedTime * oCount + oneSidedTime * oCount) * 2));
                oneSided[cCount] = true;
                cCount++;
                StartCoroutine(DamageAfterTime(damage, held[i].stats.target, oneSidedTime + oneSidedTime + (oneSidedTime * oCount + oneSidedTime * oCount) * 2, false)); //time to rotate + time to bonk (in this case oneSidedTime on both)
                oCount++;
            }
            else
            {
                Movements[cCount] = StartCoroutine(DelayMotion(held[i], 90));
                cCount++;
                StartCoroutine(DamageAfterTime(damage, held[i].stats.target,clashTime + clashTime, true)); //time to rotate + time to bonk (in this case clashTime on both) + time for onesided attacks in full
            }
        }
        for (int i = 0; i < cCount; i++)
        {
            if (oneSided[i] == true)
                yield return Movements[i];
        }
        sync = true;
        for (int i = 0; i < cCount; i++)
        {
            if (oneSided[i] != true)
                yield return Movements[i];
        }
        Debug.Log("motions complete");
        sync = false;
        yield return null;
        for (int i = 0; i < held.Length; i++)
        {
            held[i].stats.target = null;
            held[i].targetArrow();
            held[i].CheckDeath();
        }
        held = players.GetComponentsInChildren<UnitHolder>();
        for (int i = 0; i < held.Length; i++)
        {
            held[i].stats.target = null;
            held[i].targetArrow();
            held[i].CheckDeath();
        }
        yield return null;
        UpdateUI();
    }

    private void UpdateUI()
    {
        UnitHolder[] held;
        if (pcount != players.transform.childCount)
        {
            pcount = players.transform.childCount;
            if (pcount == 1)
            {
                players.transform.GetChild(0).localPosition = Vector3.zero;
            }
            else if (pcount == 2)
            {
                players.transform.GetChild(0).localPosition = new Vector3(-gap / 2, 0, 0);
                players.transform.GetChild(1).localPosition = new Vector3(gap / 2, 0, 0);
            }
            held = players.GetComponentsInChildren<UnitHolder>();
            for (int i = 0; i < held.Length; i++)
            {
                held[i].targetArrow();
            }
        }
        if (ecount != enemies.transform.childCount)
        {
            ecount = enemies.transform.childCount;
            if (ecount == 1)
            {
                enemies.transform.GetChild(0).localPosition = Vector3.zero;
            }
            else if (ecount == 2)
            {
                enemies.transform.GetChild(0).localPosition = new Vector3(-gap / 2, 0, 0);
                enemies.transform.GetChild(1).localPosition = new Vector3(gap / 2, 0, 0);
            }
            held = enemies.GetComponentsInChildren<UnitHolder>();
            for (int i = 0; i < held.Length; i++)
            {
                held[i].targetArrow();
            }
        }
    }

    public void Select(GameObject choice)
    {
        if (state == CombatState.DEFAULT)
        {
            if (choice.transform.parent.gameObject == players)
            {
                source = choice;
                state = CombatState.SELECTING;
            }
        }
        else if (state == CombatState.SELECTING)
        {
            if (choice.transform.parent.gameObject == players)
            {
                source.GetComponent<UnitHolder>().stats.target = null;
                source.GetComponent<UnitHolder>().targetArrow();
                source = null;
                state = CombatState.DEFAULT;
            }
            else if (choice.transform.parent.gameObject == enemies)
            {
                if ((pcount == ecount && Mathf.Abs(choice.transform.GetSiblingIndex() - source.transform.GetSiblingIndex()) < 2)
                    || (pcount > ecount && choice.transform.GetSiblingIndex() - source.transform.GetSiblingIndex() <= 0 && choice.transform.GetSiblingIndex() - source.transform.GetSiblingIndex() >= -1) //target minus me <= 0 means same or less
                    || (pcount < ecount && choice.transform.GetSiblingIndex() - source.transform.GetSiblingIndex() >= 0 && choice.transform.GetSiblingIndex() - source.transform.GetSiblingIndex() <= 1)) //reverse reversed
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

    IEnumerator DamageAfterTime(float damage, UnitHolder target, float time, bool wait)
    {
        while(wait && !sync)
        {
            yield return null;
        }
        yield return new WaitForSeconds(time);
        target.stats.CurrentHP -= damage;
        //Debug.Log("" + target.stats.CurrentHP + "/" + target.stats.MaxHP);

        target.UpdateSelf();
        yield return null;
    }

    IEnumerator DelayMotion(UnitHolder held, float tweak)
    {
        while (!sync)
        {
            yield return null;
        }
        yield return StartCoroutine(held.gameObject.GetComponent<moveCoroutine>().FullMotion((held.stats.target.gameObject.transform.position - held.gameObject.transform.position) / 3 + held.gameObject.transform.position, clashTime, Quaternion.Euler(0, 0, held.arrowSprite.transform.rotation.eulerAngles.z + tweak), clashTime,0));
        yield return null;
    }
}
