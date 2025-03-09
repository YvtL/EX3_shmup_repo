using UnityEngine;
using System.Collections;

namespace shmup
{
    public class AutoAiming : MonoBehaviour {


        public Transform target;
        public float rotationSpeed = 2.0f;
        public bool smooth = true;
        public bool rotationLimit;
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
        public float minZ;
        public float maxZ;

        void Awake()
        {
            //target = SceneManager.sceneManager.playerTransform;
        }

        // Use this for initialization
        void Start () {
            target = ShumpSceneManager.sceneManager.playerTransform;
        }

        void LateUpdate()
        {
            if (ShumpSceneManager.sceneManager.currentSceneStatus != ShumpSceneManager.SceneStatus.Playing)
                return;

            if (target)
            {
                Debug.DrawLine(target.transform.position, transform.position, Color.yellow);
                if (smooth)
                    {
                    Quaternion rotate = Quaternion.LookRotation(target.transform.position - transform.position);

                    if (rotationLimit)
                        {
                        rotate.eulerAngles = new Vector3(
                                                         Mathf.Clamp(rotate.eulerAngles.x, minX, maxX),
                                                         Mathf.Clamp(rotate.eulerAngles.y, minY, maxY),
                                                         Mathf.Clamp(rotate.eulerAngles.z, minZ, maxZ)
                                                         );
                        }   


                    transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * rotationSpeed);
                    }
                else
                    {
                    // Just lookat
                    transform.LookAt(target.transform);
                    }
            }
        }
    }
}
