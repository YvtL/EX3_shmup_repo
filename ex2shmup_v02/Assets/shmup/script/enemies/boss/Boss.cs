using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace shmup
{
    [RequireComponent(typeof(Animation))]
    public class Boss : Enemy {


        [Space]
        [Header("Boss")]
        public string bossName;
        public Text bossNameText;

        public Animation secondaryAnim;
        
        [Space]
        [Header("AnimationClips")]
        public AnimationClip introAnimation;
        public AnimationClip dieAnimation;

        [System.Serializable]
        public class tactic
                {
                    [Tooltip("hp % needed to select this tactic. If NextTacticTrigger is bossElemetsDestroyed, you can ignore this")]
                    [Range(1,100)]
                    public float hpTrigger;

                    [Space]

                        public AnimationClip transformationAnim;
                    
                    [Space]

                        public float tacticAnimTransitionDuration;
                        public AnimationClip tacticAnim;

                    [Space]

                    public BossElement[] bossElements;
                }
        public enum NextTacticTrigger
        {
            percentualHp, //change tactic when the hp is at x% from the start hp
            bossElemetsDestroyed //change tactic when all the boss elements of the current tactic are destroyed
        }
        public NextTacticTrigger nextTacticTrigger;
        [HideInInspector]
        public int bossElementsToDamageInThisTactic;

        public tactic[] tactics;
        int currentTactic;

        [System.Serializable]
        public class damageFx
        {
            [Tooltip("hp % needed to select this damage Fx")]
            [Range(0, 100)]
            public float hpTrigger;
            [HideInInspector]
            public bool triggred;
            public GameObject[] objFx;
        }
        public damageFx[] damageFxs;

        bool tranformationIsPlaying;
        Animation anim;


        Vector3 startPosition;
        [Tooltip("Needed to restore the orgininal position on the movable parts of the boss")]
        public Transform[] movableElements;
        Vector3[] originalPositions;
        Quaternion[] originalRotations;

        [Space]
        [Header("Gui")]
        public Slider hpSlider;
        public GameObject hpGui;

	// Use this for initialization
	public override void Start () {

            base.Start();//call start() from Enemy.cs
            enemyType = EnemyType.boss;
            bossNameText.text = bossName;

            deactivateMeWhenDestroyed = true;

            originalPositions = new Vector3[movableElements.Length];
            originalRotations = new Quaternion[movableElements.Length];
            for (int i = 0; i < movableElements.Length; i++)
                {
                originalPositions[i] = movableElements[i].localPosition;
                originalRotations[i] = movableElements[i].localRotation;
                }
            

            //setup animations
            tranformationIsPlaying = false;
            anim = GetComponent<Animation>();
            startHp = 0;
            if (introAnimation)
                anim.AddClip(introAnimation, introAnimation.name);

            if (dieAnimation)
                secondaryAnim.AddClip(dieAnimation, dieAnimation.name);

            for (int i = 0; i < tactics.Length; i++)
                {
                if (tactics[i].transformationAnim != null)
                    secondaryAnim.AddClip(tactics[i].transformationAnim, tactics[i].transformationAnim.name);

                for (int ii = 0; ii < tactics[i].bossElements.Length; ii++)
                    startHp += tactics[i].bossElements[ii].hp;

                anim.AddClip(tactics[i].tacticAnim, tactics[i].tacticAnim.name);
                }
            hp = startHp;
            //die = false;
            vulnerable = false;
            startPosition = transform.position;
            ShumpSceneManager.sceneManager.stageBoss = this;

        }

    public void ResetMe()
        {
            print("Boss reset");
            this.gameObject.SetActive(true);
            CancelInvoke();
            hpGui.SetActive(false);

            currentTactic = 0;

            anim.Stop();
            secondaryAnim.Stop();

            hpSlider.maxValue = startHp;
            hp = startHp;
            vulnerable = false;
            transform.position = startPosition;

            for (int i = 0; i < damageFxs.Length; i++)
            {
                damageFxs[i].triggred = false;
                for (int ii = 0; ii < damageFxs[i].objFx.Length; ii++)
                    damageFxs[i].objFx[ii].SetActive(false);
            }

            for (int i = 0; i < tactics.Length; i++)
            {
                for (int ii = 0; ii < tactics[i].bossElements.Length; ii++)
                    {
                    tactics[i].bossElements[ii].ResetMe();
                    }
            }

            for (int i = 0; i < tactics[0].bossElements.Length; i++)
                tactics[0].bossElements[i].gameObject.SetActive(true);

            for (int i = 0; i < movableElements.Length; i++)
            {
                 movableElements[i].localPosition = originalPositions[i];
                 movableElements[i].localRotation = originalRotations[i];
            }
        }

    public void ActivateMe()
        {
            hpSlider.maxValue = startHp;
            hpGui.SetActive(true);
            currentTactic = 0;

            PlayIntro();


        }

        void PlayDie()
        {
            if (dieAnimation)
                {
                Debug.Log("dieAnimation.length " + dieAnimation.length);
                this.gameObject.SetActive(true);
                ShumpSceneManager.sceneManager.currentSceneStatus = ShumpSceneManager.SceneStatus.sceneEnd;
                secondaryAnim.Play(dieAnimation.name);
                ShumpSceneManager.sceneManager.Invoke("Win", dieAnimation.length);
                }
            else
                ShumpSceneManager.sceneManager.Win();
        }

        void PlayIntro()
        {
            print("PlayIntro()");
            if (introAnimation)
            {
                vulnerable = false;
                destroyable = false;
                anim.wrapMode = WrapMode.Once;
                anim.clip = tactics[currentTactic].tacticAnim;
                anim.Play(introAnimation.name);
                Invoke("PlayTransformationAnim", introAnimation.length);
            }
            else
                PlayTransformationAnim();
        }

        void PlayTransformationAnim()
        {
            print("PlayTransformationAnim");

            if (tranformationIsPlaying)
                return;

            vulnerable = true;
            destroyable = true;

            if (tactics[currentTactic].transformationAnim != null)
            {
                print("PlayIntro: " + currentTactic + " = " + tactics[currentTactic].transformationAnim.name);

                tranformationIsPlaying = true;
                secondaryAnim.wrapMode = WrapMode.Once;
                secondaryAnim.clip = tactics[currentTactic].transformationAnim;
                secondaryAnim.Play(secondaryAnim.clip.name);
                //secondaryAnim.CrossFade(secondaryAnim.clip.name, tactis[currentTactic].introAnimTransitionDuration);
                Invoke("EndTranformation", tactics[currentTactic].transformationAnim.length);
            }
            else
                ActivateCurrentTactic();
        }

        void EndTranformation()
        {
            print("EndTranformation()");
            tranformationIsPlaying = false;
            ActivateCurrentTactic();
        }

        // Update is called once per frame
        void Update () {

            if (!hpGui.activeSelf)
                return;

            hpSlider.value = hp;
		
	}

        public override void TakeDamage(float damage)
        {
            if (vulnerable)
            {
                CheckIfChangeTactic();

                hp -= damage;

                if (hp <= 0 && destroyable)
                    DestroyMe();
            }

        }

        void FxCheck(float percentage)
        {
            for (int i = 0; i < damageFxs.Length; i++)
            {
                if (percentage <= damageFxs[i].hpTrigger && !damageFxs[i].triggred)
                {
                    damageFxs[i].triggred = true;
                    for (int ii = 0; ii < damageFxs[i].objFx.Length; ii++)
                        damageFxs[i].objFx[ii].SetActive(true);
                }
            }
        }

        public void CheckIfChangeTactic()
        {
            //print("CheckIfChangeTactic");

            if (tranformationIsPlaying)
                return;

            if (currentTactic+1 >= tactics.Length)
                return;

            float currentHpPercentage = (hp * 100) / startHp;

            FxCheck(currentHpPercentage);

            if (nextTacticTrigger == NextTacticTrigger.percentualHp)
            {
                print("hp " + currentHpPercentage + "%   <= " + tactics[currentTactic+1].hpTrigger);
                if (currentHpPercentage <= tactics[currentTactic+1].hpTrigger)//trigger next tactic
                {
                    currentTactic++;
                    PlayTransformationAnim();
                }
            }
            else if (nextTacticTrigger == NextTacticTrigger.bossElemetsDestroyed)
            {
                if (bossElementsToDamageInThisTactic == 0)
                    {
                    currentTactic++;
                    PlayTransformationAnim();
                    }
            }
        }

    void ActivateCurrentTactic()
        {
            if (tranformationIsPlaying)
                return;

            if (nextTacticTrigger == NextTacticTrigger.bossElemetsDestroyed)
                bossElementsToDamageInThisTactic = tactics[currentTactic].bossElements.Length;

            print("Boss start tactic: " + currentTactic + " = " + tactics[currentTactic].tacticAnim.name);
            anim.wrapMode = WrapMode.Loop;
            anim.clip = tactics[currentTactic].tacticAnim;
           // anim.Play(anim.clip.name);
            anim.CrossFade(anim.clip.name, tactics[currentTactic].tacticAnimTransitionDuration);

            
            if (tactics[currentTactic].bossElements.Length > 0)
                {
                for (int i = 0; i < tactics[currentTactic].bossElements.Length; i++)
                    tactics[currentTactic].bossElements[i].ActiveteMe();
                }
        }

        public override void DestroyMe()
        {
            base.DestroyMe();
            vulnerable = false;
            PlayDie();
            FxCheck(0);
            //SceneManager.sceneManager.Win();
        }
    }
}
