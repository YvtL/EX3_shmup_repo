using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace shmup
{
    public class Bullet : Weapon
    {
        [Tooltip("Each different bullet prefab MUST have a different Id")]
        public int myId;//for garbage

        [Space]
        [Tooltip("You can have multiple bullet speeds subsequent, in order to create special behavior")]
        [SerializeField] float[] speed = new float[1];
        int speedSelected;
        [Tooltip("If you have mutiple speeds, set here the duration of everyone here")]
        [SerializeField] float[] speedDuration = new float[1];
        [SerializeField] float acceleration = 0;
        float currentAcceleration;
        float scaledAcceleration;

        //Rigidbody rb;
        Transform myTransform;

        [Tooltip("You can use it only if harm == Harm.player")]
        [SerializeField] bool aimed = false;
        bool targetAcquired;
        Transform target;
        Vector3 targetPosition;
        [SerializeField] float aimingDelay = 0;
        [Tooltip("The rotation speed of the autoAiming player missile")]
        [SerializeField] float maneuverability;
        float maneuverabilityIncrement = 0.01f;//to avoid infinite orbit
        Color debugColor;
        List<Transform> enemyTargets; //avaible enemy targets on the screen

        [Tooltip("Partilce FX when the bullet hit something")]
        public GameObject sparkFX;
        

        void Start()
        {
            myTransform = this.transform;
        }



        void AimNow()
        {
            if (harm == Harm.enemy)
                FindEnemies();
            else
                AimTarget(1000);
        }

        void FindEnemies()
        {
            enemyTargets.Clear();

            //find all avaible enemies
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
                {

                if (!enemy.activeSelf)
                    continue;

                EnemyAvatar enemyAvatar = enemy.GetComponent<EnemyAvatar>();
                BossElement bossElement = enemy.GetComponent<BossElement>();


                if (enemyAvatar != null && enemyAvatar.enemy.enabled && enemyAvatar.enemy.vulnerable)//if this enemy is avaible
                    {
                    enemyTargets.Add(enemy.transform);
                    continue;
                    }
                else if (bossElement != null && bossElement.myBoss.vulnerable)
                    {
                    enemyTargets.Add(enemy.transform);
                    continue;
                    }
                }

            
            if (target == null && (enemyTargets.Count > 0))
            {
                //print("enemyTargets.Count: " + enemyTargets.Count);
                
                    //sort enemies by distance
                    enemyTargets.Sort(delegate (Transform t1, Transform t2)
                    {
                    return (Vector3.Distance(t1.position, myTransform.position).CompareTo(Vector3.Distance(t2.position, myTransform.position)));
                    });

                //pick most close enemy
                target = enemyTargets[0];
                targetAcquired = true;

            }

           
        }

        void ChangeSpeed()
        {
            if (speedSelected+1 < speed.Length)
            {
               Invoke("ChangeSpeed", speedDuration[speedSelected]);
               speedSelected++;
            }
        }



        // Update is called once per frame
        void Update () {

            if (targetAcquired)
               AimTarget(maneuverability);

               MoveForward();
           
    }

        void MoveForward()
        {
            float move = speed[speedSelected] * Time.deltaTime;

		    if (acceleration>0)
			    {
                currentAcceleration += scaledAcceleration;
                myTransform.Translate(Vector3.forward* (move * currentAcceleration));
                }
		    else
                myTransform.Translate(Vector3.forward* move);
        }

        void AimTarget(float rotationSpeed)
        {
            if (target == null || !target.gameObject.activeSelf)
            {
                //change target
                targetAcquired = false;
                target = null;
                FindEnemies();
                return;
            }

            Debug.DrawLine(target.position, myTransform.position, debugColor);
            //aim
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);

            maneuverability += maneuverabilityIncrement;

        }

        public override void OnTriggerEnter(Collider coll)
        {
            base.OnTriggerEnter(coll);
                
            //don't destroy if hit another bullet
            /*
            Bullet bullet = coll.gameObject.GetComponentInParent<Bullet>();
            if (bullet == null)
                GarbageMe();*/
        }

        public override void DestroyMe()
        {
            GarbageMe();

            GarbageManager.THIS.InstantiateGameObj(sparkFX, this.transform.position, 10);
            /*
            if (sparkFX)
            {
                GameObject fx = GarbageManager.THIS.GetGameObject(sparkFX, 10);
                if (fx != null)
                {
                    fx = Instantiate(fx, this.transform.position, Quaternion.identity);
                    fx.SetActive(true);
                }
            }*/

        }

        public void ExitFromScreen() 
        {
            GarbageMe();
        }

        void GarbageMe()
        {
            CancelInvoke();
            this.gameObject.SetActive(false);
            BulletGarbage.THIS.AddBulletToGarbage(myId, this);
        }

        public void InstantiateMe(Vector3 startPosition, Quaternion startRotation)
        {

            destroyMeAtContact = true;
            speedSelected = 0;
            currentAcceleration = 0;
            targetAcquired = false;
            target = null;

            if (myTransform == null)
                myTransform = this.transform;

            myTransform.position = startPosition;
            myTransform.rotation = startRotation;


            if (aimed)
            {
                if (harm == Harm.player)
                {
                    debugColor = Color.yellow;
                    target = ShumpSceneManager.sceneManager.playerTransform;
                    myTransform.SetParent(ShumpSceneManager.sceneManager.bulletsParent);
                }
                else if (harm == Harm.enemy)
                {
                    debugColor = Color.blue;
                    enemyTargets = new List<Transform>();
                }
            }

            //Start
            if (acceleration > 0)
            {
                scaledAcceleration = acceleration * 0.1f;
                currentAcceleration = scaledAcceleration;
            }

            this.gameObject.SetActive(true);

            if (speed.Length > 1)
                Invoke("ChangeSpeed", speedDuration[speedSelected]);

            if (aimed)
                Invoke("AimNow", aimingDelay);
        }

        /*
        void DestroyMe(bool showFX)
        {
            if (showFX && sparkFX)
            {
            if (ShumpSceneManager.sceneManager.sparksGarbage.childCount == 0)
                Instantiate(sparkFX, this.transform.position, Quaternion.identity);
            else
                ShumpSceneManager.sceneManager.sparksGarbage.GetChild(0).GetComponent<Spark>().RecycleMe(this.transform.position);
            }

            GarbageMe();
        }*/
    }
}