using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageManager : MonoBehaviour
{
    public static GarbageManager THIS;
    
    [System.Serializable]
    public class PrewarmGameObject
    {
        public GameObject gameObject;
        public int quantity = 1;
        List<GameObject> myGameObjects = new List<GameObject>();


        public void PrewarmMe()
        {
            for (int i = 0; i < quantity; i++)
            {
                GameObject item = Instantiate(gameObject);
                item.SetActive(false);
                myGameObjects.Add(item);
            }
        }

        public GameObject GetFirstAvaibleGameObject()
        {
            if (myGameObjects.Count > 0)
            {
                for (int i = myGameObjects.Count-1; i >= 0; i--)
                {
                    if (myGameObjects[i] == null)
                    {
                        myGameObjects.RemoveAt(i);
                        continue;
                    }


                    if (!myGameObjects[i].activeSelf)
                            return myGameObjects[i];
                }
            }

            GameObject obj = Instantiate(gameObject);
            obj.SetActive(false);
            myGameObjects.Add(obj);

            return obj;
        }
    }
    public List<PrewarmGameObject> prewarmedGameObjects;
    Dictionary<string, PrewarmGameObject> pool;

    void Awake()
    {
        THIS = this;
    }

    private void Start()
    {
        Prewarm();
    }


    void Prewarm()
    {
        pool = new Dictionary<string, PrewarmGameObject>();
        for (int i = 0; i < prewarmedGameObjects.Count; i++)
        {
            prewarmedGameObjects[i].PrewarmMe();
            pool.Add(prewarmedGameObjects[i].gameObject.name, prewarmedGameObjects[i]);
        }
    }

    public GameObject GetGameObject(GameObject prefab, int prewarmQuantity = 1)
    {
        if (prefab == null)
            return null;

        if (pool.ContainsKey(prefab.name))
        {
            return pool[prefab.name].GetFirstAvaibleGameObject();
        }
        else//add a new PrewarmGameObject
        {
            PrewarmGameObject newPrewarmGameObject = new PrewarmGameObject();
            newPrewarmGameObject.gameObject = prefab;
            newPrewarmGameObject.quantity = prewarmQuantity;
            pool.Add(prefab.name, newPrewarmGameObject);
            return newPrewarmGameObject.GetFirstAvaibleGameObject();
        }

    }

    public GameObject InstantiateGameObj(GameObject prefab, Vector3 targetPosition, int prewarmQuantity = 1)
    {
        if (prefab == null)
            return null;


            GameObject temp = GarbageManager.THIS.GetGameObject(prefab, prewarmQuantity);
            if (temp != null)
            {
            temp.transform.position = targetPosition;
            temp.SetActive(true);
            }

        return temp;


    }

    public GameObject GetGameObjectByName(string prefabName, int prewarmQuantity = 5)
    {

        if (pool.ContainsKey(prefabName))
            return pool[prefabName].GetFirstAvaibleGameObject();

        return null;

    }


}
