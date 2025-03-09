using UnityEngine;
using System.Collections;

namespace shmup
{
    public class EmitterGroup : MonoBehaviour {

        public bool becomeActiveWhenOnScreen;
        public bool playerEmitter;

        public GameObject bullet;
        [Tooltip("particle fx")]
        public GameObject shotWhiffFx;

        [Tooltip("how many bullets shot in one blast")]
        public int blastLeght;

        public float blastFrequency;

        public float pauseBetweenBlasts;
        public bool blastOngoing;
        public bool canShot;

        [HideInInspector]
        public bool blastOnlyOnce;

        Emitter[] myEmitters;
        int emitters;

        [Range(0, 1)]
        public float shotDispersion;
        [Range(0, 45)]
        public float shotDispersionAngle;

        // Use this for initialization
        void Start() {
            SetupEmitters();

    }

    public void Shot()
        {
            for (int i = 0; i < emitters; i++)
                {
                myEmitters[i].canShot = true;
                }
        }

    public void ResetMe()
        {
            for (int i = 0; i < emitters; i++)
            {
                myEmitters[i].blastOnlyOnce = false;
                myEmitters[i].gameObject.SetActive(true);
                myEmitters[i].BlastReset();
                myEmitters[i].blastOnlyOnce = true;
            }
        }

        void SetupEmitters()
        {
            
            emitters = this.transform.childCount;
            myEmitters = new Emitter[emitters];

            for (int i = 0; i < emitters; i++)
                {
                myEmitters[i] = this.transform.GetChild(i).GetComponent<Emitter>();

                myEmitters[i].becomeActiveWhenOnScreen = becomeActiveWhenOnScreen;
                myEmitters[i].playerEmitter = playerEmitter;
                myEmitters[i].bullet = bullet;
                myEmitters[i].shotWhiffFx = shotWhiffFx;
                myEmitters[i].blastLeght = blastLeght;
                myEmitters[i].pauseBetweenBlasts = pauseBetweenBlasts;
                myEmitters[i].blastOngoing = blastOngoing;
                myEmitters[i].canShot = canShot;
                myEmitters[i].blastOnlyOnce = blastOnlyOnce;

                myEmitters[i].shotDispersion = shotDispersion;
                myEmitters[i].shotDispersionAngle = shotDispersionAngle;
                }
        }
    }
}
