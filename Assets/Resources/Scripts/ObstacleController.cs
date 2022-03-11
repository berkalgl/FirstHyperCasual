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
        //instantiate black hole every 5 level and its multiple
        int currentLevel = PlayerPrefs.GetInt("currentLevel");
        if (((currentLevel + 1) % 5) == 0)
        {
            Instantiate(GetPrefab(3),
                new Vector3(Random.Range(startXIndex, stopXIndex), 0.31f, Random.Range(startZIndex, stopZIndex)),
                new Quaternion(0, 0, 0, 0),
                transform);
        }

        //instantiate coin
        Instantiate(GetPrefab(4),
            new Vector3(Random.Range(startXIndex, stopXIndex), 0.75f, Random.Range(startZIndex, stopZIndex)),
            new Quaternion(0, 0, 0, 0),
            transform);

        float randomXIndexForStop, tempStartXIndex, tempStopXIndex;
        while(startZIndex <= stopZIndex)
        {
            randomXIndexForStop = GetRandomIndex(startXIndex, stopXIndex);
            tempStartXIndex = startXIndex;
            tempStopXIndex = stopXIndex;
            while(tempStartXIndex <= tempStopXIndex)
            {
                bool isItTimeToSpawnStoppingObstacle = tempStartXIndex == randomXIndexForStop;
                var gameObject = Instantiate(GetPrefab(isItTimeToSpawnStoppingObstacle ? 1 : 2),
                    new Vector3(tempStartXIndex,0.31f,startZIndex),
                    new Quaternion(0,0,0,0),
                    transform);

                if(isItTimeToSpawnStoppingObstacle)
                    gameObject.transform.localScale = new Vector3(5f,5f,5f);
                else                    
                    gameObject.transform.localScale = new Vector3(3f,3f,3f);

                tempStartXIndex += xIndexDiff;
            }

            startZIndex += zIndexDiff;
        }

    }

    public float GetRandomIndex(float minValue, float maxValue)
    {
        return Random.Range((int)minValue, (int)maxValue);
    }

    public GameObject GetPrefab(int obstacleType)
    {
        //1 --> stopping obstacle syringe
        //2 --> random feeding parts
        //3 --> black hole to die
        //4 --> coin
        switch (obstacleType)
        {
            case 1:
                return (GameObject)Resources.Load("Prefabs/Syringe_1", typeof(GameObject));
            case 2:
                return (GameObject)Resources.Load("Prefabs/BodyParts/bodypart_0" + Random.Range(1, 9), typeof(GameObject));
            case 3:
                return (GameObject)Resources.Load("Prefabs/BlackHolePrefab", typeof(GameObject));
            case 4:
                return (GameObject)Resources.Load("Prefabs/Coin", typeof(GameObject));
            default:
                return null;
        }
    }
}
