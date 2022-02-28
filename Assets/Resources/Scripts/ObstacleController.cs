using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public float startZIndex,stopZIndex,zIndexDiff,startXIndex,stopXIndex,xIndexDiff;
    void Start()
    {
        InitializeObstacles();
    }
    public void InitializeObstacles()
    {
        float randomXIndexForStop, tempStartXIndex, tempStopXIndex;
        while(startZIndex <= stopZIndex)
        {
            randomXIndexForStop = GetRandomXIndex(startXIndex, stopXIndex);
            tempStartXIndex = startXIndex;
            tempStopXIndex = stopXIndex;
            while(tempStartXIndex <= tempStopXIndex)
            {
                var gameObject = Instantiate(GetRandomPrefab(tempStartXIndex == randomXIndexForStop),
                    new Vector3(tempStartXIndex,0.31f,startZIndex),
                    new Quaternion(0,0,0,0),
                    transform);

                if(tempStartXIndex == randomXIndexForStop)
                    gameObject.transform.localScale = new Vector3(5f,5f,5f);
                else                    
                    gameObject.transform.localScale = new Vector3(3f,3f,3f);
                
                

                tempStartXIndex += xIndexDiff;
            }

            startZIndex += zIndexDiff;
        }
    }

    public float GetRandomXIndex(float minimumXIndex, float maximumXIndex)
    {
        return Random.Range((int)minimumXIndex,(int)maximumXIndex);
    }

    public GameObject GetRandomPrefab(bool isStopObstacle)
    {
        if(isStopObstacle)
            return (GameObject)Resources.Load("Prefabs/Syringe_1", typeof(GameObject));

        return (GameObject)Resources.Load("Prefabs/bodypart_0" + Random.Range(1,9), typeof(GameObject));
    }
}
