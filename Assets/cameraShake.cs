using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    public bool startShaking = false;
    public AnimationCurve curve;
    public float duration = 1f;
    public float magnitude = 20f;

    /*
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            startShaking = true;
            Debug.Log("Start shaking is TRUE");
        }

    }
    */

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            startShaking = true;
            Debug.Log("Start shaking is TRUE");
        }
    }
    */

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            startShaking = true;
            Debug.Log("Start shaking is TRUE");
        }
    }
    

    void Update()
    {
        if (startShaking)
        {
            startShaking = false;
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            //wait to shake
          //  yield return new WaitForSeconds(1.5f);

            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration) * magnitude;
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }
}
