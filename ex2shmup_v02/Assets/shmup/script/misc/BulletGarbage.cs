using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shmup
{
    public class BulletGarbage : MonoBehaviour {

    public static BulletGarbage THIS;

    public Dictionary<int, List<Bullet>> garbageDictionary;//garbage id, bullet script

    private void Awake()
        {
            THIS = this;
            garbageDictionary = new Dictionary<int, List<Bullet>>();
        }



    public void Prewarm(GameObject newBullet,  int bulletId, int quantity)
        {
            GameObject tempBullet = null;

            if (!garbageDictionary.ContainsKey(bulletId))
            {
                tempBullet = Instantiate(newBullet);
                tempBullet.SetActive(false);

                List<Bullet> mainFireList = new List<Bullet>();
                mainFireList.Add(tempBullet.GetComponent<Bullet>());

                garbageDictionary.Add(bulletId, mainFireList);
            }


            for (int i = 1; i < quantity; i++)
            {
                tempBullet = Instantiate(newBullet);
                tempBullet.SetActive(false);
                AddBulletToGarbage(bulletId, tempBullet.GetComponent<Bullet>());
            }
        }

    public Bullet GetBullet(int bulletId)
        {
            
            Bullet returnThis = null;

            foreach (Bullet b in garbageDictionary[bulletId])
            {
                if (b.gameObject.activeSelf)
                    continue;

                return b;
            }
            /*
            if (garbageDictionary[bulletId].Count > 1)
            {
                returnThis = garbageDictionary[bulletId][1];
                garbageDictionary[bulletId].RemoveAt(1);
            }
            else
            {*/
                GameObject tempBullet = Instantiate(garbageDictionary[bulletId][0].gameObject);
                //tempBullet.name = "bullet " + garbageDictionary[bulletId].Count;
                tempBullet.SetActive(false);
                returnThis = tempBullet.GetComponent<Bullet>();
           // }

            if (returnThis == null)
                Debug.LogWarning("Missing bullet !!!!!! id: " + bulletId);


            return returnThis;

            //return GarbageManager.THIS.GetGameObjectByName(garbageDictionary[bulletId][0].gameObject.name, 500).GetComponent<Bullet>();
        }
    

    public void AddBulletToGarbage(int bulletId, Bullet thisBullet)
        {
            garbageDictionary[bulletId].Add(thisBullet);
        }

    }
}
