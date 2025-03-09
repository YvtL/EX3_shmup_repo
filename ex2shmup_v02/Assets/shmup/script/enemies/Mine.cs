using UnityEngine;
using System.Collections;

namespace shmup
{
    public class Mine : Enemy {

    public bool damageTriggerMine;
    public enum MineType
    {
        timer,
        proximity
    }
    public MineType mineType;
        public float proximityTrigger;
        public float timerTrigger;
        float startTime;

        bool mineActive;
        bool mineTriggered;
        public Color triggerColor;
        public float explosionDelay;

        public EmitterGroup emitterGroup;

        // Use this for initialization
        public override void Start() {

            base.Start();//call start() from Enemy.cs
            enemyType = EnemyType.mine;
            mineActive = true;

        }

        public override void RestoreMe()
        {
            base.RestoreMe();

            CancelInvoke();
            avatarColliderObj.SetActive(true);
            mineTriggered = false;
            mineActive = true;
            emitterGroup.gameObject.SetActive(false);
            emitterGroup.ResetMe();

        }

        // Update is called once per frame
        void Update () {

            if (!mineActive)
                return;

            if (mineTriggered)
                {
                maxPulses = Mathf.Infinity;
                PulseColor(triggerColor, 0.5f);
                }
            else
                {
                switch (mineType)
                    {
                    case MineType.timer:
                        if (Time.timeSinceLevelLoad > startTime + timerTrigger)
                            TriggerMine();
                     break;

                    case MineType.proximity:
                        if (Vector3.Distance(ShumpSceneManager.sceneManager.playerTransform.position, transform.position) < proximityTrigger)
                            TriggerMine();
                    break;

                    }
            }


        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            if (damageTriggerMine && damage > 0 && hp > 0)
                TriggerMine();
            
        }

        void TriggerMine()
        {
            if (mineTriggered)
                return;

           mineTriggered = true;
                Invoke("MineExplode", explosionDelay);



        }

        void MineExplode()
        {
            if (ShumpSceneManager.sceneManager.currentSceneStatus != ShumpSceneManager.SceneStatus.Playing)
                return;

            mineTriggered = false;

            //explosion splinters
            emitterGroup.blastOnlyOnce = true;
            emitterGroup.canShot = true;
            emitterGroup.gameObject.SetActive(true);
            Shot();

            mineActive = false;
            avatarColliderObj.SetActive(false);

        }

        public void Shot()
        {
            emitterGroup.Shot();
        }

        void OnBecameVisible()
        {
            if (timerTrigger > 0)
                startTime = Time.timeSinceLevelLoad;
        }

        void OnDrawGizmosSelected()
        {
            if (mineType == MineType.proximity)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.position, proximityTrigger);
            }
        }
    }
}