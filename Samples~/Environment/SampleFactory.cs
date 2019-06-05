using UnityEngine;
using UnityEngine.UI;

public class SampleFactory : MonoBehaviour
{
    public GameObject parent;
    public GameObject prefab;
    public int LimitCount = 2000;
    public Text spawnedObjectsCounter;

    public bool RunTest = true;

    public int internalObjs = 0;
    public int spawnSpeed = 1;
    private Vector3 Min = new Vector3(-20, 0, -20);
    private Vector3 Max = new Vector3(20, 0, 20);
    private GameObject[] spawnedObjects = new GameObject[2000];

    void Update()
    {
        if (RunTest && Time.timeScale != 0.0)
        {
            if (internalObjs == LimitCount)
            {
                return;
            }
            else if (internalObjs < LimitCount)
            {
                SpawnObject();
            }
            else
            {
                DestroyObject();
            }
        }
        spawnedObjectsCounter.text = internalObjs.ToString();
    }

    void DestroyObject()
    {
        internalObjs--;
        Destroy(spawnedObjects[internalObjs]);
        spawnedObjects[internalObjs] = null;
    }

    void SpawnObject()
    {
        for (int i = 0; i < spawnSpeed; i++)
        {
            var _xAxis = UnityEngine.Random.Range(Min.x, Max.x);
            var _yAxis = UnityEngine.Random.Range(Min.y, Max.y);
            var _zAxis = UnityEngine.Random.Range(Min.z, Max.z);
            var _randomPosition = new Vector3(_xAxis, _yAxis, _zAxis);
            spawnedObjects[internalObjs] = Instantiate(prefab, _randomPosition, parent.transform.rotation);
            internalObjs++;
        }
    }

    public void FlushObjects()
    {
        for (int i = internalObjs; i > 0; i--)
        {
            DestroyObject();
        }
    }
}
