using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseData : MonoBehaviour
{
    //type, xPos, yPos
    public Vector3[] TargetData;
    public Vector3[] WallData;

    [ContextMenu("Save Phase")]
    void SavePhase()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("target");
        TargetData = new Vector3[objects.Length];
        for (int i = 0; i < TargetData.Length; i++)
        {
            TargetData[i].x = objects[i].GetComponent<indexer>().INDEX;
            TargetData[i].y = objects[i].transform.position.x;
            TargetData[i].z = objects[i].transform.position.y;
        }

        objects = GameObject.FindGameObjectsWithTag("wall");
        WallData = new Vector3[objects.Length];
        for (int i = 0; i < WallData.Length; i++)
        {
            WallData[i].x = objects[i].GetComponent<indexer>().INDEX;
            WallData[i].y = objects[i].transform.position.x;
            WallData[i].z = objects[i].transform.position.y;
        }
    }

    [ContextMenu("Load Phase")]
    void LoadPhase()
    {
        TargetsManager.instance.LoadTargets(this);
    }
}
