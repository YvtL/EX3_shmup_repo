using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace shmup
{
    public class EmenyFollowWaypoints : MonoBehaviour {

    [HideInInspector]
    public EnemyEmitter myEnemyEmitter;
    //public Vector3[] waypoints;
    int nextWaypoint;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float rotationSpeed;
    Transform myTransform;

        [SerializeField] UnityEvent OnPathEnd = null;

        // Use this for initialization
        void Start () {
        nextWaypoint = 1;
        myTransform = transform;

    }
	
	// Update is called once per frame
	void Update () {

        if (myEnemyEmitter.waypoints.Length < 1)
            return;

        //move
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(myTransform.position, myEnemyEmitter.waypoints[nextWaypoint], step);
        //rotate
        float s = 1;
        if (nextWaypoint == 1)
            s = 100;
        
        Vector3 targetDir = myEnemyEmitter.waypoints[nextWaypoint] - myTransform.position;
        float rotStep = rotationSpeed * Time.deltaTime * s;
        Vector3 newDir = Vector3.RotateTowards(myTransform.forward, targetDir, rotStep, 0.0F);
        myTransform.rotation = Quaternion.LookRotation(newDir);
        

        float dist = Vector3.Distance(myEnemyEmitter.waypoints[nextWaypoint], myTransform.position);
        if (dist < 0.01f)
            {
                if (nextWaypoint < myEnemyEmitter.waypoints.Length - 1)
                    nextWaypoint++;
                else
                    {
                    myEnemyEmitter.enemyOnScreen--;
                    OnPathEnd?.Invoke();
                    //this.gameObject.SetActive(false);
                    }
            }

    }


}
}