using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shmup
{
    public class BossElement : Enemy {

    [Space]
    [Header("Boss Setup")]
    public Boss myBoss;
    public GameObject[] objToActivate;

        public override void Start()
        {
            //print(name + "  Start");
            base.Start();//call start() from Enemy.cs
            enemyType = EnemyType.bossElement;
            deactivateMeWhenDestroyed = true;
            ResetMe();

        }

        public void ResetMe()
        {
           // print("reset: " + name);
            vulnerable = false;
            destroyable = false;
            RestoreMe();
            for (int i = 0; i < objToActivate.Length; i++)
                {
                objToActivate[i].SetActive(false);
                Emitter emitter = objToActivate[i].GetComponent<Emitter>();
                if (emitter)
                    emitter.StartMe();
                }
        }

        public void ActiveteMe()
        {
            vulnerable = resetVulnerable;
            destroyable = resetDestroyable;

            RestoreMe();

            if (avatarColliderObj)
                avatarColliderObj.SetActive(true);
            for (int i = 0; i < objToActivate.Length; i++)
                    objToActivate[i].SetActive(true);
            
        }

        public override void TakeDamage(float damage)
        {
            if (damage > hp)
                damage = hp;

            base.TakeDamage(damage);

            if (vulnerable)
                myBoss.TakeDamage(damage);
    
            if (myBoss.nextTacticTrigger == Boss.NextTacticTrigger.bossElemetsDestroyed && hp <= 0)
                {
                myBoss.bossElementsToDamageInThisTactic--;
                myBoss.CheckIfChangeTactic();
                }

        }
    }
}
