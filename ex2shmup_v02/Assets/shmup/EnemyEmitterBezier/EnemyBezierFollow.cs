using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace shmup
{
public class EnemyBezierFollow : MonoBehaviour
{

        [SerializeField] UnityEvent OnPathEnd = null;

        EnemyEmitterBezier myEnemyEmitter;
        PathCreation.PathCreator pathCreator;
        PathCreation.EndOfPathInstruction endOfPathInstruction = PathCreation.EndOfPathInstruction.Stop;
        float speed = 5;
        float distanceTravelled;
        bool isMoving = false;

        public void StartMe(PathCreation.PathCreator myPath, float mySpeed/*, EnemyEmitterBezier myEmitter*/)
        {
                pathCreator = myPath;
                speed = mySpeed;
                //myEnemyEmitter = myEmitter;

                isMoving = true;
        }

        void Update()
        {
            if (!isMoving)
                return;

            if (Vector3.Distance(transform.position, pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1)) < 0.1f)
            {
                isMoving = false;
                OnPathEnd?.Invoke();
                return;
            }


            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        /*
        public void DestroyMe()
        {
            isMoving = false;
            //myEnemyEmitter.enemyOnScreen--;
            this.gameObject.SetActive(false);
        }*/

    }
}
