using UnityEngine;
using System.Collections;
using System;

namespace shmup
{
    public class EnemyEmitter : MonoBehaviour {

    public bool activeMeAtStart;
    public EmenyFollowWaypoints enemy;
        GameObject[] enemies;
    //public GameObject enemyObj;
    public float enemySpeed;
    public float enemyRotationSpeed;
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

    [Space]
    public bool showGizmos;
    public Color gizmosColor;


    // Use this for initialization
        void Start () {
           

            if (activeMeAtStart)
                ActivateMe();
        }

    

    public void ResetMe()
    {
            if (enemies != null)
            {
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i] != null)
                        Destroy(enemies[i]);
                }
            }

        enemies = new GameObject[enemyNumber];
        enemyDestroyed = 0;
        enemyCount = 0;
        currentState = MyState.waiting;
    }

    public void ActivateMe()
    {
        ResetMe();
        Transform myTransform = this.gameObject.transform;
        CalculateWaypoints();
        if (currentState == MyState.waiting)
            {
            nextEnemy = Time.timeSinceLevelLoad + emissionRate;
            currentState = MyState.on;
            }
    }

    // Update is called once per frame
    void Update () {

        if (enemyOnScreen > 0)
            CalculateWaypoints();

        if (currentState != MyState.on)
            return;

        if (Time.timeSinceLevelLoad > nextEnemy)
            {
            nextEnemy = Time.timeSinceLevelLoad + emissionRate;

           // if (myTransform == null)
               // return;

            //print(enemyCount + "... " + myTransform.position);
            EmenyFollowWaypoints enemyTemp = (EmenyFollowWaypoints)Instantiate(enemy, this.transform.position, Quaternion.identity);
                enemies[enemyCount] = enemyTemp.gameObject;
                enemyTemp.transform.SetParent(myTransform.parent);

                //enemyTemp.waypoints = waypoints;
                enemyTemp.myEnemyEmitter = this;
                enemyTemp.speed = enemySpeed;
                enemyTemp.rotationSpeed = enemyRotationSpeed;

            //enemyTemp.GetComponent<Enemy>().myEnemyEmitter = this;
                enemyTemp.GetComponent<Enemy>().OnEnemyDestroyByPlayer += OnEnemyDestroyByPlayer;

                enemyCount++;
            enemyOnScreen++;

            if (enemyCount == enemyNumber)
                currentState = MyState.off;
            
               
            }

    }

    void CalculateWaypoints()
    {
        myTransform = this.transform;
        waypoints = new Vector3[myTransform.childCount + 1];
        waypoints[0] = myTransform.position;
        for (int i = 1; i < waypoints.Length; i++)
            waypoints[i] = myTransform.GetChild(i-1).gameObject.transform.position;
    }

    void OnDrawGizmos()
    {
            if (!showGizmos)
                return;

        CalculateWaypoints();

        Gizmos.color = gizmosColor;

        for (int i = 0; i < waypoints.Length-1; i++)
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
    }
        /*
    public int CalculateChainScore()
        {
            enemyDestroyed++;
            return enemyDestroyed;
        }*/
        /*
    public bool ChainDestroyed()
        {
            enemyOnScreen--;

            if (enemyDestroyed == enemyNumber)
                return true;
            else
                return false;
        }*/



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
