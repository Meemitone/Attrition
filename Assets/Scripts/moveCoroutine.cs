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
    public Vector3 originalPos;

    public bool test = false;

    public Quaternion targetRotation;
    public Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
        originalPos = transform.position;

        //StartCoroutine(myCoroutine(target));
        if (test)
        {
            StartCoroutine(FullMotion(target.position, moveTime, targetRotation, rotateTime, 0));
        }
    }

    IEnumerator myCoroutine(Transform target)
    {
        //MOVE AND ROTATE CARD TO FACE ENEMY ONE
        while (Vector3.Distance(transform.position, target.position) > 3.5f && transform.rotation != targetRotation)
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


    IEnumerator Rotater(Quaternion r, float t)//r for target rotation
    {
        float timePassed = 0;
        Quaternion start = transform.rotation;
        while (timePassed + Time.deltaTime < t)
        {
            timePassed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(start, r, timePassed / t);
            yield return null;
        }
        transform.rotation = r;
    }

    IEnumerator Mover(Vector3 p, float t)
    {
        float timePassed = 0;
        Vector3 start = transform.position;
        while (timePassed + Time.deltaTime < t)
        {
            timePassed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, p, timePassed / t);
            yield return null;
        }
        transform.position = p;
    }

    public IEnumerator FullMotion(Vector3 targetPos, float moveTime, Quaternion targetRot, float turnTime, float delayTime)
    {
        originalPos = transform.position;
        originalRotation = transform.rotation;
        yield return new WaitForSeconds(delayTime);
        yield return StartCoroutine(Rotater(targetRot, turnTime));
        yield return StartCoroutine(Mover(targetPos, moveTime));
        yield return StartCoroutine(Mover(originalPos, moveTime));
        yield return StartCoroutine(Rotater(originalRotation, turnTime));
    }
}
