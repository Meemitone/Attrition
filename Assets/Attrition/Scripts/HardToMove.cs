using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardToMove : MonoBehaviour
{
    public Vector3 lockedPos;
    public Quaternion lockedRot;
    // Start is called before the first frame update
    void Start()
    {
        lockedPos = transform.position;
        lockedRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = lockedPos;
        transform.rotation = lockedRot;
    }
}
