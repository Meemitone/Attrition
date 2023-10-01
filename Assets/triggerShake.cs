using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerShake : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Enemy")
        {
            GameObject.Find("Main Camera").SendMessage("doShake");

            Debug.Log("The camera trigger has hit");
        }
    }
}
