using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PatrollingData
{
    [SerializeField] public List<Transform> PatrollingPositions = new List<Transform>();
    [SerializeField] public Transform EscapePoint = null;

    private int PatrollingCount = 0;

    public Transform GetClosestPatrollingPos(Vector3 pos)
    {
        int closestIndex = 0;
        float dist;
        float closDist = float.MaxValue;

        for (int i = 0; i < PatrollingPositions.Count; i++)
        {
            dist = Vector3.SqrMagnitude(pos - PatrollingPositions[i].position);
            if (dist < closDist)
            {
                closDist = dist;
                closestIndex = i;
            }
        }

        PatrollingCount = closestIndex;
        return PatrollingPositions[closestIndex];
    }

    public Transform GetNextPoint()
    {
        return PatrollingPositions[PatrollingCount];
    }
}
