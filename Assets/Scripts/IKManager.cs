using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour
{
    public IKJoint Root;

    public IKJoint End;

    public GameObject Target;

    public float DistanceThreshold = 0.05f;

    public float MovementSpeed = 5f;

    public float GetDistance(Vector3 p1, Vector3 p2)
    {
        return Vector3.Distance(p1, p2);
    }

    public float CalculateSlope(IKJoint joint)
    {
        float deltaTheta = 0.01f;
        float distBefore = GetDistance(End.transform.position, Target.transform.position);

        joint.Rotate(deltaTheta);

        float distAfter = GetDistance(End.transform.position, Target.transform.position);

        joint.Rotate(-deltaTheta);

        return (distAfter - distBefore) / deltaTheta;
    }

    // Start is called before the first frame update
    void Update()
    {
        for (int i = 0; i < 20; i++)
        {
                
            if (GetDistance(End.transform.position, Target.transform.position) > DistanceThreshold)
            {
                var current = Root;
                while (current != null)
                {
                    float slope = CalculateSlope(current);
                    //Root.LookAt(Target.transform);
                    current.Rotate(-slope * MovementSpeed);

                    current = current.Child;
                }
            }

        }
    }
}
