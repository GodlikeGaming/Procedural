using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FastIKFabric : MonoBehaviour
{
    public int ChainLength;

    public Transform Target, Pole;


    public int Iterations = 10;

    public float Delta = 0.001f;

    [Range(0, 1)]
    public float SnapbackStrenght = 1f;

    float[] BonesLength;
    float CompleteLength;
    Transform[] Bones;
    Vector3[] Positions;

    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    void Init()
    {
        Bones = new Transform[ChainLength + 1];
        Positions = new Vector3[ChainLength + 1];
        BonesLength = new float[ChainLength + 1];

        CompleteLength = 0;


        // init data

        var curr = transform;

        for (int i = Bones.Length - 1; i >= 0; i--)
        {
            Bones[i] = curr;

            if(i == Bones.Length - 1)
            {

            } else
            {
                BonesLength[i] = (Bones[i + 1].position - curr.position).magnitude;
                CompleteLength += BonesLength[i];
            }

            curr = curr.parent;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (Target == null) return;

        if (Bones.Length != ChainLength)
        {
            Init();
        }

        for (int i = 0; i < Bones.Length; i++)
        {
            Positions[i] = Bones[i].position;
        }

        // calc it all

            // is reachable?
        if ((Target.position -Bones[0].position).sqrMagnitude >= CompleteLength * CompleteLength)
        {
            // stretch it 
            var direction = (Target.position - Positions[0]).normalized;

            for (int i = 1; i < Positions.Length; i++)
            {
                Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
            }
        } else
        {
            for (int iter = 0; iter < Iterations; iter++)
            {
                //back
                for (int i = Positions.Length - 1; i > 0; i--)
                {
                    if (i == Positions.Length - 1)
                    {
                        Positions[i] = Target.position;
                    } else
                    {
                        Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];
                    }
                }

                // forward
                for (int i = 1; i < Positions.Length; i++)
                {
                    Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1];
                }


                // close enough
                if ((Positions.Last() - Target.position).sqrMagnitude < Delta * Delta)
                {
                    break;
                }
            }
        }

        if (Pole != null)
        {
            for (int i = 1; i < Positions.Length - 1; i++)
            {
                var plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(Pole.position);
                var projectedBone = plane.ClosestPointOnPlane(Positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - Positions[i - 1], projectedPole - Positions[i - 1], plane.normal);

                Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
            }
        }
        

        for (int i = 0; i < Positions.Length; i++)
        {
            Bones[i].position = Positions[i];
        }
    }

    private void OnDrawGizmos()
    {
        var curr = transform;

        for (int i = 0; i < ChainLength; i++)
        {
            if (curr == null && curr.parent == null) break;

            var scale = Vector3.Distance(curr.position, curr.parent.position) * 0.1f;

            Handles.matrix = Matrix4x4.TRS(
                curr.position,
                Quaternion.FromToRotation(Vector3.up, curr.parent.position - curr.position),
                new Vector3(scale, Vector3.Distance(curr.parent.position, curr.position), scale)
                );
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);

            curr = curr.parent;
        } 
    }
}
