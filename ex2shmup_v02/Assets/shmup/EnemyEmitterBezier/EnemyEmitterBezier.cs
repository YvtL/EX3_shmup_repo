using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shmup
{
    public class EnemyEmitterBezier : MonoBehaviour
{
    public bool activeMeAtStart;
    PathCreation.PathCreator myPath;
        public EnemyBezierFollow enemy;
    GameObject[] enemiesObj;

    public float enemySpeed;
    //public float enemyRotationSpeed;
    [Tooltip("the chain lenght")]
    public int enemyNumber;
    public float emissionRate;
    float nextEnemy;
    int enemyCount;
    [HideInInspector]
    public int enemyOnScreen;
    Transform myTransform;

    //chain
    [HideInInspector]
    public int enemyDestroyed;
    [Tooltip("If the player destroy all the enemy of the chain, give this:")]
    public GameObject chainBonus;

    enum MyState
    {
        waiting,
        on,
        off
    }
    MyState currentState;

    [HideInInspector]
    public Vector3[] waypoints;

        private void Awake()
        {
            myPath = GetComponent<PathCreation.PathCreator>();
            myTransform = this.transform;
        }

        // Use this for initialization
        void Start()
        {


        if (activeMeAtStart)
            ActivateMe();
        }



    public void ResetMe()
    {
        if (enemiesObj != null)
        {
            for (int i = 0; i < enemiesObj.Length; i++)
            {
                if (enemiesObj[i] != null)
                    Destroy(enemiesObj[i]);
            }
        }

        enemiesObj = new GameObject[enemyNumber];
        enemyDestroyed = 0;
        enemyCount = 0;
        currentState = MyState.waiting;

    }

    public void ActivateMe()
    {
        ResetMe();
        Transform myTransform = this.gameObject.transform;

        if (currentState == MyState.waiting)
        {
            nextEnemy = Time.timeSinceLevelLoad + emissionRate;
            currentState = MyState.on;
        }
    }

    // Update is called once per frame
    void Update()
    {


        if (currentState != MyState.on)
            return;

        if (Time.timeSinceLevelLoad > nextEnemy)
        {
            nextEnemy = Time.timeSinceLevelLoad + emissionRate;


                EnemyBezierFollow enemyTemp = (EnemyBezierFollow)Instantiate(enemy, this.transform.position, Quaternion.identity);
                enemiesObj[enemyCount] = enemyTemp.gameObject;
                enemyTemp.transform.SetParent(myTransform.parent);

                enemyTemp.StartMe(myPath, enemySpeed/*, this*/);

                //enemyTemp.GetComponent<Enemy>().myEnemyEmitter = this;

                //EventHandler eventHandler = enemyTemp.GetComponent<Enemy>().myEnemyEmitter();
                enemyTemp.GetComponent<Enemy>().OnEnemyDestroyByPlayer += OnEnemyDestroyByPlayer;

                enemyCount++;
                //enemyOnScreen++;

            if (enemyCount == enemyNumber)
                currentState = MyState.off;


        }

    }

    void OnEnemyDestroyByPlayer(object sender, Enemy.OnEnemyDestroyByPlayerArgs args)
    {
            enemyDestroyed++;

            args.thisEnemy.UpdateChainScore(enemyDestroyed);

            if (enemyDestroyed >= enemyNumber)
                Instantiate(chainBonus, args.thisEnemy.GetMyPosition(), Quaternion.identity);

            args.thisEnemy.OnEnemyDestroyByPlayer -= OnEnemyDestroyByPlayer;


    }



}
}

