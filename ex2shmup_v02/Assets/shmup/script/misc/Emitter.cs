using UnityEngine;
using System.Collections;

namespace shmup
{
    public class Emitter : MonoBehaviour {

        public bool becomeActiveWhenOnScreen;
        public bool playerEmitter;

        public GameObject bullet;
        [Tooltip("How many bullet you want to prewarm in the garbage")]
        public int prewarmQuantity;
        int shotId;
        Transform myTransform;
        [Tooltip("particle fx")]
        public GameObject shotWhiffFx;

        [Tooltip("how many bullets shot in one blast")]
        public int blastLeght;
        int blastCount;
        [Tooltip("time gap between bullets in the same blast")]
        public float blastFrequency;
        float nextShot;
        public float pauseBetweenBlasts; 
        float pauseCount;// how much pause time is left
        public bool blastOngoing;
        public bool canShot;
        public float firstShotDelay;

        [HideInInspector]
        public bool blastOnlyOnce;

        [Range(0, 1)]
        public float shotDispersion;
        [Range(0, 45)]
        public float shotDispersionAngle;

        [Space]
        [Header("Audio")]
        public MyAudioClip myAudioClip;
        public enum MyAudioClip
        {
            Mute,
            Generic, //use the audioclip in SceneManager
            Custom, //use the audio clip in this prefav
        }
        public AudioClip shotAudioFx;
        AudioSource myAudioSource;

        // Use this for initialization
        void Awake () {
            myAudioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            StartMe();
            PrewarmGarbage();
        }

        void PrewarmGarbage()
        {

            //main fire
            shotId = bullet.GetComponent<Bullet>().myId;
            BulletGarbage.THIS.Prewarm(bullet, shotId, prewarmQuantity);
        }

        public void StartMe()
        {
            myTransform = this.transform;

            if (canShot)
                nextShot = Time.timeSinceLevelLoad + firstShotDelay;
            else
                nextShot = 0;

            blastOngoing = false;
            blastCount = 0;

        }

        // Update is called once per frame
        void Update () {
            ShotBlast();
    }

    void OnDrawGizmosSelected()//show shot direction
        {
                Gizmos.color = Color.red;
            Vector3 direction = transform.TransformDirection(Vector3.forward);
            Gizmos.DrawRay(transform.position, direction);

        }

        public void EnableGunshot(bool enableGunshot)
        {
            canShot = enableGunshot;
            blastOngoing = true;
            blastCount = 0;
        }

    public void BlastReset()
        {
            if (blastOnlyOnce)
                this.gameObject.SetActive(false);
            else
            {
                blastOngoing = false;
                nextShot = 0.0f;
                blastCount = 0;
                pauseCount = Time.timeSinceLevelLoad + pauseBetweenBlasts;
            }
        }
    
    void ShotFx()
        {
            if (shotWhiffFx)
                {
                CancelInvoke("ShotFxOff");
                shotWhiffFx.SetActive(true);
                Invoke("ShotFxOff", 0.25f);
                }

            if (myAudioClip == MyAudioClip.Mute || !myAudioSource)
                return;

            if (myAudioClip == MyAudioClip.Custom && shotAudioFx)
                myAudioSource.PlayOneShot(shotAudioFx, ShumpSceneManager.sceneManager.audioEffectsVolume);
            else if (myAudioClip == MyAudioClip.Generic && ShumpSceneManager.sceneManager.enemyShotGenericSfx)
                myAudioSource.PlayOneShot(ShumpSceneManager.sceneManager.enemyShotGenericSfx, ShumpSceneManager.sceneManager.audioEffectsVolume);

        }

    void ShotFxOff()
        {
            shotWhiffFx.SetActive(false);
        }


        void SingleShot(/*GameObject thisBullet*/)
        {
            if (!this.gameObject.activeSelf)
                return;

            //Debug.Log("SHOT!!");

            ShotFx();

            Vector3 fireDispersion = new Vector3(Random.Range(-shotDispersion, shotDispersion), 0, 0);
            Quaternion fireAngle = Quaternion.Euler(0, Random.Range(-shotDispersionAngle, shotDispersionAngle), 0);

            //Instantiate(thisBullet, transform.position + fireDispersion, transform.rotation * fireAngle);
            BulletGarbage.THIS.GetBullet(shotId).InstantiateMe(myTransform.position + fireDispersion,
                                                                myTransform.rotation * fireAngle);

            //count update
            blastCount++;
            nextShot = Time.timeSinceLevelLoad + blastFrequency;
        }

   void ShotBlast()
    {
            if (ShumpSceneManager.sceneManager.currentSceneStatus != ShumpSceneManager.SceneStatus.Playing)
                return;

        if (canShot == true)
            {
            if (blastOngoing == true) 
                {
                if (Time.timeSinceLevelLoad > nextShot)
                    {
                    if (blastCount < blastLeght)//this blast is ongoing 
                        {
                        SingleShot(/*bullet*/);
                        }
                    else //this blast is done
                        {
                        BlastReset();
                        }
                    }
                }
            else //you are in pause between blasts
                {
                if (Time.timeSinceLevelLoad > pauseCount)
                    {
                        blastOngoing = true; //pause end
                    }

                }
            }
    }


    void OnBecameVisible()
        {
            if (becomeActiveWhenOnScreen)
                enabled = true; 
        }

    void OnBecameInvisible()
        {
            if (!playerEmitter)
                enabled = false;
        }
    }
}
