using System;
using UnityEngine;
using System.Collections;

namespace shmup
{
    public class Enemy : MonoBehaviour {

    public EventHandler<OnEnemyDestroyByPlayerArgs> OnEnemyDestroyByPlayer;
        public class OnEnemyDestroyByPlayerArgs : EventArgs
        {
            public Enemy thisEnemy;
        }

        [Tooltip("can take damage")]
    public bool vulnerable;
        [HideInInspector] public bool resetVulnerable;
        [Tooltip("can be destroyed")]
    public bool destroyable;
        [HideInInspector] public bool resetDestroyable;
        bool dying;
    public float hp;
        [HideInInspector]
        public float startHp;
    public Color damageColor;
    public int score;
    public float timeBonus;
        public GameObject avatarColliderObj;
        public Renderer avatarRenderer;
        [HideInInspector]
        public Color avatarOriginalColor;
       // [HideInInspector]
        //public Renderer avatarRender;
        public GameObject explosionObj;//when destroyed

    public enum EnemyType
    {
        ship,
        mine,
        boss,
        bossElement
    }
    [HideInInspector]
    public EnemyType enemyType;

   // public EmitterGroup emitterGroup; //from where shot
   // public GameObject bullet; //what fire

    //[HideInInspector] public EnemyEmitter myEnemyEmitter;
    [HideInInspector]
    public bool deactivateMeWhenDestroyed;

        [Space]
        [Header("Audio")]
        public MyAudioClip myAudioClip;
        public enum MyAudioClip
        {
            Mute,
            Generic, //use the audioclip in SceneManager
            Custom, //use the audio clip in this prefav
        }
        public AudioClip getHitAudioFx;
        AudioSource myAudioSource;

        private void Awake()
        {
            resetVulnerable = vulnerable;
            resetDestroyable = destroyable;
        }

        // Use this for initialization
        public virtual void Start () {

            myAudioSource = GetComponent<AudioSource>();

            if (avatarRenderer)
                {
                //avatarRender = avatar.GetComponent<Renderer>();
                avatarOriginalColor = avatarRenderer.material.GetColor("_EmissionColor");
                avatarRenderer.material.shader = Shader.Find("Standard");
                }

            if (GetComponent<PermamentElementOnMap>() != null)
                deactivateMeWhenDestroyed = true;

            startHp = hp;
            dying = false;
        }


        public virtual  void OnTriggerEnter(Collider otherObject)
        {
            if (otherObject.tag == "Player")
            {
                ShumpSceneManager.sceneManager.playerTransform.GetComponent<PlayerShip>().HitMe();
            }
        }

        bool isFlashing = false;
        IEnumerator FlashColor(Color targetColor, float smooth = 10f)
        {
            if (isFlashing || avatarRenderer == null)
                yield break;

            isFlashing = true;
            bool startA = false;
            bool startB = false;
            Color lerpedColor = avatarOriginalColor;
            avatarRenderer.material.SetColor("_EmissionColor", avatarOriginalColor);
            

            //print("start a");
            startA = true;
            while (startA)
                {
                lerpedColor = Color.Lerp(lerpedColor, targetColor, Time.deltaTime * smooth);

                avatarRenderer.material.SetColor("_EmissionColor", lerpedColor);


                if (Vector4.Distance(lerpedColor, targetColor) <= 0.5f)
                    {
                    avatarRenderer.material.SetColor("_EmissionColor", targetColor);
                    //print("end a");
                    startA = false;
                    startB = true;
                    break;
                    }

                yield return null;

            }


            //print("start b");
            while (startB)
            {
                lerpedColor = Color.Lerp(lerpedColor, avatarOriginalColor, Time.deltaTime * smooth);
                avatarRenderer.material.SetColor("_EmissionColor", lerpedColor);

                yield return null;

                if (Vector4.Distance(lerpedColor, avatarOriginalColor) <= 0.1f)
                    {
                    avatarRenderer.material.SetColor("_EmissionColor", avatarOriginalColor);
                    //print("end b");
                    startB = false;
                    break;
                    }
            }

            isFlashing = false;


        }

        [HideInInspector]public float maxPulses = 1;
        public void PulseColor(Color color, float maxIntensity)
        {
            if (maxPulses <= 0)
                {
                avatarRenderer.material.SetColor("_EmissionColor", avatarOriginalColor);
                return;
                }

            float intensity = Mathf.PingPong(Time.time*2f, maxIntensity+0.1f);
            if (intensity >= maxIntensity)
                maxPulses--;

            Color finalColor = color * Mathf.LinearToGammaSpace(intensity);

            avatarRenderer.material.SetColor("_EmissionColor", finalColor);
        }

        public virtual void TakeDamage(float damage)
        {
            if (dying)
                return;

            //print(gameObject.name + " Take damage: " + damage);
            if (vulnerable)
            {
                if (avatarRenderer)
                    StartCoroutine(FlashColor(damageColor));

                PlayGetHit();
                hp -= damage;
                if (hp <= 0 && destroyable)
                    DestroyMe();
               
                    
            }

        }

        public Vector3 GetMyPosition()
        {
            return transform.position;
        }


        public void UpdateChainScore(int chainCount)
        {
            tempScore *= chainCount;
        }

        int tempScore;
        public virtual void DestroyMe()
        {
            tempScore = score;
            dying = true;

            OnEnemyDestroyByPlayer?.Invoke(this, new OnEnemyDestroyByPlayerArgs { thisEnemy = this });


            if (tempScore > 0)
                ShumpSceneManager.sceneManager.GainScore(tempScore, transform);

            if (TimerManager.timerManager.useTimer && timeBonus > 0)
                TimerManager.timerManager.GainTime(timeBonus, transform.position);

            RemoveMe();

            //explosion fx
            if (explosionObj)
            {
                GameObject temp = GarbageManager.THIS.InstantiateGameObj(explosionObj, avatarRenderer.transform.position);
                temp.transform.rotation = avatarRenderer.transform.rotation;
            }

        }

        public void RemoveMe()
        {

            if (deactivateMeWhenDestroyed)
            {
                this.gameObject.SetActive(false);
                if (avatarColliderObj)
                    avatarColliderObj.SetActive(false);
            }
            else
                Destroy(this.gameObject);
        }

        public virtual void RestoreMe()
        {
            if (avatarRenderer == null) //player is dead before see this enemy, so there is no need to restore
                return;

            hp = startHp;
            dying = false;
            avatarRenderer.material.SetColor("_EmissionColor", avatarOriginalColor);
            isFlashing = false;
            this.gameObject.SetActive(true);
            if (avatarColliderObj)
                avatarColliderObj.SetActive(true);
        }

        void PlayGetHit()
        {

            if (myAudioClip == MyAudioClip.Mute || !myAudioSource)
                return;

            if (myAudioClip == MyAudioClip.Custom && getHitAudioFx)
                myAudioSource.PlayOneShot(getHitAudioFx, ShumpSceneManager.sceneManager.audioEffectsVolume);
            else if (myAudioClip == MyAudioClip.Generic && ShumpSceneManager.sceneManager.enemyGetHitGenericSfx)
                myAudioSource.PlayOneShot(ShumpSceneManager.sceneManager.enemyGetHitGenericSfx, ShumpSceneManager.sceneManager.audioEffectsVolume);

        }

    }
}
