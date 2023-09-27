using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCoroutine : MonoBehaviour
{
    public Transform target;
    public float moveTime;
    public float rotateTime;
    public float angle;  //NOT USED CURRENTLY!!!!!
    public Vector3 pos;

    public Quaternion targetRotation;
    public Quaternion originalRotation;

    void Start()
    {
        StartCoroutine(myCoroutine(target));
    }

    IEnumerator myCoroutine(Transform target)
    {
        //MOVE AND ROTATE CARD TO FACE ENEMY ONE
        while(Vector3.Distance(transform.position, target.position) > 3.5f && transform.rotation != targetRotation)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * moveTime);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateTime);

            // transform.localEulerAngles = new Vector3(0, 0, 180);

             yield return null;
        }

        print("Object moved!");

        yield return new WaitForSeconds(3f);

        //MOVE AND ROTATE CARD BACK TO STARTING POSITION   ?????
         transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * moveTime);
     //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateTime);

        yield return new WaitForSeconds(3f);

        print("Coroutine is now completed!");
    }

}
