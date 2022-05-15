using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKJoint : MonoBehaviour
{
    public IKJoint Child;

    IKJoint GetChild()
    {
        return Child;
    }

    public void Rotate(float angle)
    {
        transform.Rotate(Vector3.up * angle);
    }

    public void LookAt(Transform target)
    {
        transform.LookAt(target, Vector3.forward);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
