using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameDataManager;

public class UnitHolder : MonoBehaviour
{
    [SerializeField] private Sprite[] Sprites;
    [SerializeField] public CombatUnit stats;
    [SerializeField] public Animator[] anims;
    [SerializeField] private SpriteRenderer cardImage;
    public TextMeshProUGUI Type;
    public TextMeshProUGUI ATK;
    public TextMeshProUGUI DEF;
    public Slider HP;
    public bool UsePrimaryType = true;
    public GameObject arrowSprite;

    // Start is called before the first frame update
    void Start()
    {
        if(stats.PrimType == UnitType.NULL)
        {
            Randomize();
        }
        UpdateSelf();
        cardImage.sprite = Sprites[(int)GiveType()];
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void Randomize()
    {
        stats.Attack = Random.Range(15, 26);
        stats.Defense = Random.Range(15, 26);
        stats.MaxHP = Random.Range(80, 121);
        stats.CurrentHP = stats.MaxHP;
        stats.PrimType = (UnitType)Random.Range(1, 4);
    }

    public void UpdateSelf()
    {
        ATK.text = ((int)stats.Attack).ToString();
        DEF.text = ((int)stats.Defense).ToString();
        Type.text = (stats.PrimType).ToString();
        HP.value = stats.CurrentHP / stats.MaxHP;
    }

    internal void targetArrow()
    {
        if(stats.target == null)
        {
            arrowSprite.transform.localScale = new Vector3(1,arrowSprite.transform.localScale.y);
            arrowSprite.GetComponent<HardToMove>().lockedPos = transform.position; //.transform.localPosition = Vector3.zero;
        }
        else
        {
            arrowSprite.GetComponent<HardToMove>().lockedPos = (stats.target.gameObject.transform.position - gameObject.transform.position) / 2 + gameObject.transform.position;
            arrowSprite.transform.localScale = new Vector3((stats.target.gameObject.transform.position - gameObject.transform.position).magnitude*.9f, arrowSprite.transform.localScale.y);
            arrowSprite.GetComponent<HardToMove>().lockedRot = Quaternion.FromToRotation(transform.right, (stats.target.gameObject.transform.position - gameObject.transform.position));
        }
    }

    internal UnitType GiveType()
    {
        if(UsePrimaryType)
        {
            return stats.PrimType;
        }
        else
        {
            return stats.SecType;
        }
    }

    internal void CheckDeath()
    {
        if (stats.CurrentHP <= 0)
        {
            //RemoveSelf (ideally with animation)
            Destroy(gameObject);
        }
    }
}
