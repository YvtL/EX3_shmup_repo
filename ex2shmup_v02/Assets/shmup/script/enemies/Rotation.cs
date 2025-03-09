using UnityEngine;
using System.Collections;

namespace shmup
{
    public class Rotation : MonoBehaviour {

        public float rotationSpeed;

        public bool randomStartRotationAngle;
        Vector3 startRotation;
        public bool rotationLimit;
        public float maxAngle;
        Vector3 maxTargetRotation;
        public float minAngle;
        Vector3 minTargetRotation;
        bool rotateTowardMaxAngle;
        [Tooltip("How much close you must be to target angle in order to inver the rotation direction")]
        public float approximation = 0.5f;

        Quaternion originalRotation;

    // Use this for initialization
        void Start () {

            if (GetComponent<PermamentElementOnMap>())
                ShumpSceneManager.sceneManager.rotationToReset.Add(this);

            originalRotation = transform.rotation;

            maxTargetRotation = new Vector3(0, maxAngle, 0);
            minTargetRotation = new Vector3(0, minAngle, 0);

            if (randomStartRotationAngle)
            {
                float y = 0;
                if (rotationLimit)
                    y = Random.Range(minAngle, maxAngle);
                else
                    y = Random.Range(0, 360);

                startRotation = new Vector3(0, y, 0);
                transform.rotation = Quaternion.Euler(startRotation);
            }

        }

        public void ResetMe()
        {
            transform.rotation = originalRotation;
        }

        // Update is called once per frame
        void Update () {

            if (ShumpSceneManager.sceneManager.currentSceneStatus != ShumpSceneManager.SceneStatus.Playing)
                return;

            if (rotationLimit)
                {
                Quaternion rotate = Quaternion.identity;
                if (rotateTowardMaxAngle)
                    rotate.eulerAngles = maxTargetRotation;
                else
                    rotate.eulerAngles = minTargetRotation;

                transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * rotationSpeed);

                if (rotateTowardMaxAngle)
                    {
                    if (Quaternion.Angle(transform.rotation, rotate) < approximation)
                        rotateTowardMaxAngle = false;
                    }
               else
                    {
                    if (Quaternion.Angle(transform.rotation, rotate) < approximation)
                        rotateTowardMaxAngle = true;
                    }

            }
            else
                transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);

        }
}
}