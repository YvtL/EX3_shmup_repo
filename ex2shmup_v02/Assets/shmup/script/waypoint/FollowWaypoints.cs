using UnityEngine;
using System.Collections;

namespace shmup
{
    public class FollowWaypoints : MonoBehaviour {

        public Transform cameraPivot;
        public WaypointInfo[] waypoints;
        public enum cycle
        {
            Once,
            pingPong,
            restartFromZero
        }
        public cycle cycle_selected;
        public bool ignoreRotation;
        public Color GizmoLineColor;

        int nextWaypoint;
        bool backward;
        bool stop;
        bool pause;
        bool canMove;

        // Use this for initialization
        void Start()
        {
            Reset();
        }

        // Update is called once per frame
        void Update()
        {
            if (ShumpSceneManager.sceneManager.currentSceneStatus != ShumpSceneManager.SceneStatus.Playing)
                return;

            if(!stop)
            {
                if (!pause)
                    {
                    //rotate
                    if (!ignoreRotation && waypoints[nextWaypoint-1].rotateToward)
                        {
                        Vector3 targetDir = waypoints[nextWaypoint].transform.position - transform.position;
                        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, waypoints[nextWaypoint-1].rotateSpeed * Time.deltaTime, 0.0F);
                        Debug.DrawRay(transform.position, newDir, Color.red);
                        transform.rotation = Quaternion.LookRotation(newDir);
                        cameraPivot.rotation = Quaternion.LookRotation(newDir);

                        if (waypoints[nextWaypoint-1].waitRotationEndBeforeMove && Vector3.Angle(targetDir, newDir) > 1)
                            canMove = false;
                        else
                            canMove = true;

                        }
                    else
                        canMove = true;

                    //move
                    if (canMove)
                        {
                        transform.position = Vector3.MoveTowards(transform.position,
                                                        waypoints[nextWaypoint].transform.position,
                                                        waypoints[nextWaypoint-1].moveSpeed * Time.deltaTime);
                        }

                    //if waypoint reached, select the next waypoint
                    if (Vector3.Distance(transform.position, waypoints[nextWaypoint].transform.position) <= 0.1f)
                            GoToNextWaypoint();
                    }
            }
        }

        void PauseEnd()
        {
            pause = false;
        }

        public void Reset()
            {
            nextWaypoint = 0;
            pause = false;
            stop = false;
            transform.position = waypoints[nextWaypoint].transform.position;
            if (!ignoreRotation)
                transform.rotation = waypoints[nextWaypoint].transform.rotation;

            GoToNextWaypoint();
        }

        void GoToNextWaypoint()
        {
            //decide next step
            if (backward)
                nextWaypoint--;
            else
                nextWaypoint++;

            if (nextWaypoint >= 0)
                {
                if (nextWaypoint < waypoints.Length)//there is a next waypoint
                    {
                    if (waypoints[nextWaypoint-1].pause > 0)
                        {
                        pause = true;
                        Invoke("PauseEnd", waypoints[nextWaypoint-1].pause);
                        }

                    if (backward)
                        {
                        }
                    else
                        {
                        }
                    }
                else //no next waypoint
                    {
                    if (cycle_selected == cycle.pingPong)
                    {
                        //nextWaypoint--;
                        backward = true;
                        GoToNextWaypoint();
                    }
                    else if (cycle_selected == cycle.restartFromZero)
                    {
                        Reset();
                    }
                    else if (cycle_selected == cycle.Once)
                        stop = true;
                }
                }   
            else
                {
                backward = false;
                transform.position = waypoints[0].transform.position;
                nextWaypoint = 0;
                GoToNextWaypoint();
            }
        }





        void OnDrawGizmos()
        {
            Gizmos.color = GizmoLineColor;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
            }

        }

        void OnDrawGizmosSelected()
        {
            
            for (int i = 0; i <= waypoints.Length - 1; i++)
            {
                if (i == 0)
                    waypoints[i].name = "Start";
                else if (i == waypoints.Length - 1)
                    waypoints[i].name = "End";
                else
                    waypoints[i].name = i.ToString();
            }

            

        }
    }
}