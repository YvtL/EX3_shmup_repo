using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace shmup
{
    public class Arsenal : MonoBehaviour {

    public enum FireButtonDown
        {
        fireForever,//fire without press button
        autofire,   //fire until keep button pressed
        chargeShot  //charge when keep button pressed
        }
        [Header("Main fire")]
        public FireButtonDown fireButtonDown;

    //fire emitters
    bool pressFire;
        public GameObject shot;
        int shotId;
        [Tooltip("How many bullet you want to prewarm in the garbage")]
        public int prewarmQuantity;
        public AudioClip mainFireSfx;
        public float fireRate = 0.5f;
        float nextFire = 0.0f;
        [Range(0,1)]
        public float mainFireDispersion;
        [Range(0, 45)]
        public float mainFireDispersionAngle;
        //secondary emitters
        [Space]
        [Header("Main emitters")]
        [Tooltip("The main emitter to trigger for each level of power gained")]
        bool pressSecondaryFire;
        public PlayerEmitter[] mainEmitters;//like: single shot, double shot, 4 shot...
        public int currentMainEmitterLevel;
        /*
        [Header("Secondary emitters")]
        public float speed_rotation_secondary_emitters;
        [System.Serializable]
        public class secondary_emitter
            {
            public GameObject my_obj;
            public bool enable;
            public Transform pivot;
            public Renderer my_renderer;
            Vector3 current_rotation_angle;
            }
        public secondary_emitter secondary_emitter_R;
        public secondary_emitter secondary_emitter_L;*/


    [Space]
    [Header("Cherged shot")]
    //public Slider[] chargeBar;
    public Color fullBarSpritePulseColor;
    public float charging_unit = 0.5f;
    [Tooltip("What shot if you relase the fire button before a full charge. If this is empty, ship will use the normal shot instead")]
    public GameObject miniShot;
    int miniId;
    [System.Serializable]
    public class charging_shot
    {
        public Slider chargeBar;
        float charging_time;
        float start_charging_time;
        bool shot_ready;
        public GameObject bullet;
        [HideInInspector] public int bulletId;
        public int prewarmQuantity = 5;
        }
    public charging_shot[] chargin_shot;
        int maxChargeBarValue = 100;
        Image fullBarSprite; //pulse when at 100%
        Color fullBarSpriteColor;
        int currentChargeLevel; //what bar is filling now
        bool chargingShot;

        [Space]
    [Header("Secondary fire")]
        public Slider energyBar;
        public float maxEnergyChargersForSecondayWeapon;
        public float startEnergy;
        public float autoRefillEnergyRate;
        public int secondaryWeaponSelected;

        [System.Serializable]
        public struct SecondayFire
        {
            public string name;
            public Sprite icon;
            public AudioClip sfx;
            public bool sfxLoop;

            public float energyCost;
            public float fireRate;

            [Tooltip("If you want to trigger an special animation when the this seconday fire is triggered, copy the animator state name here")]
            public string animationName;

            public enum ShotModel
            {
                fireASingleShot, //like bullet and missiles
                autofire,
                keepOnTheSame, //like laser beam and force field
                fireTheSameOnceWhenButtonIsPressed
            }
            public ShotModel shotModel;

            public PlayerEmitter emitter;
            public GameObject bullet;
            [HideInInspector] public int shotId;
            public int prewarmQuantity;

            public ReusableWeapon reusableWeapon;
        }
        public Animator secondaryFireAnimator;
        public SecondayFire[] secondaryWeapons;
        //public SecondaryFireWeapon[] secondaryWeapons;

        public Color insufficientEnergyToShotColor;
        Color EnergyBarColor;
        Image EnergyBarColorTarget;
        float nextSecondaryFire = 0;
        AudioSource myAudioSource;


    // Use this for initialization
    void Start () {

            myAudioSource = GetComponent<AudioSource>();

            StartGUI();

            for (int i = 0; i < secondaryWeapons.Length; i++)
                {
                if (secondaryWeapons[i].reusableWeapon != null && secondaryWeapons[i].reusableWeapon.fireRate > 0)
                    secondaryWeapons[i].fireRate = secondaryWeapons[i].reusableWeapon.fireRate;
                }

            ShumpSceneManager.sceneManager.secondatyWeaponIcon.sprite = secondaryWeapons[secondaryWeaponSelected].icon;

            PrewarmGarbage();
        }

    void PrewarmGarbage()
        {
            //main fire
            shotId = shot.GetComponent<Bullet>().myId;
            BulletGarbage.THIS.Prewarm(shot, shotId, prewarmQuantity);

            //charge fire

                miniId = miniShot.GetComponent<Bullet>().myId;
                BulletGarbage.THIS.Prewarm(miniShot, miniId, chargin_shot[0].prewarmQuantity);

                for (int i = 0; i < chargin_shot.Length; i++)
                {
                    chargin_shot[i].bulletId = chargin_shot[i].bullet.GetComponent<Bullet>().myId;
                    BulletGarbage.THIS.Prewarm(chargin_shot[i].bullet, chargin_shot[i].bulletId, chargin_shot[i].prewarmQuantity);
                }
            

            //seconday weapons
            for (int i = 0; i < secondaryWeapons.Length; i++)
            {
                if (secondaryWeapons[i].bullet == null)
                    continue;

                secondaryWeapons[i].shotId = secondaryWeapons[i].bullet.GetComponent<Bullet>().myId;
                BulletGarbage.THIS.Prewarm(secondaryWeapons[i].bullet, secondaryWeapons[i].shotId, secondaryWeapons[i].prewarmQuantity);
            }
        }



        public void StartGUI()
        {
            //setup charge shot
            if (fireButtonDown == FireButtonDown.chargeShot)
            {
                chargin_shot[0].chargeBar.transform.parent.gameObject.SetActive(true);
                ResetChargeBar();
            }
            else
                chargin_shot[0].chargeBar.transform.parent.gameObject.SetActive(false);

            //setup secondary shot
            energyBar.minValue = 0;
            energyBar.maxValue = maxEnergyChargersForSecondayWeapon;
            energyBar.value = startEnergy;
            EnergyBarColorTarget = energyBar.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            EnergyBarColor = EnergyBarColorTarget.color;
        }

        // Update is called once per frame
        void Update () {

            MainFire();

            SecondaryFire();
        }

    float pulseTime = 0;
    Color PulseColor(Color startColor, Color endColor)
        {
            Color returnThis = Color.black;
            bool pulseUp = true;
                
                if (pulseUp)
                returnThis = Color.Lerp(startColor, endColor, pulseTime);


                pulseTime += Time.deltaTime* 2.5f;
                if (pulseTime > 1)
                {
                    pulseUp = !pulseUp;
                    pulseTime = 0;
                }

            return returnThis;
        }

        bool pulseOnce = false;
        IEnumerator PulseColorOnce(Color startColor, Color endColor, Image target)
        {
            if (pulseOnce)
                yield break;
            else
                pulseOnce = true;

            while (pulseTime < 1)
            {
                
                target.color = PulseColor(EnergyBarColor, insufficientEnergyToShotColor);
                if (pulseTime > 0.9f)
                    {
                    target.color = EnergyBarColor;
                    pulseOnce = false;
                    break;
                    }

                yield return new WaitForEndOfFrame();

            }
        }


        #region Main Fire
        public void UpgradePrimaryFire()
        {
            if (currentMainEmitterLevel < mainEmitters.Length-1)
                currentMainEmitterLevel++;
        }

        void MainFire()
        {
            if (fireButtonDown == FireButtonDown.fireForever)
            {
                if (!pressSecondaryFire && PlayerController.inputOn)
                    ShotWithMainFire();
            }
            else
            {
                if (pressFire)
                {
                    if (fireButtonDown == FireButtonDown.autofire)
                        ShotWithMainFire();
                    else if (fireButtonDown == FireButtonDown.chargeShot)
                        FillChargeBar();
                }
                else
                {
                    if (fireButtonDown == FireButtonDown.chargeShot)
                        ReleaseChargeBar();
                }
            }
        }

        public void PressFire(bool buttondown)
        {
            //print(buttondown);
            if (fireButtonDown == FireButtonDown.fireForever)
                return;
            else
                pressFire = buttondown;

        }

        int shotCount = 0;
        void ShotWithMainFire()
            {
            if (Time.time > nextFire)
                {
                nextFire = Time.time + fireRate;

                PlaySfx(mainFireSfx);
                /*
                foreach (Transform spawnPoint in mainEmitters[currentMainEmitterLevel].spawnPoints)
                {
                    Vector3 fireDispersion = new Vector3(Random.Range(-mainFireDispersion, mainFireDispersion), 0, 0);
                    Quaternion fireAngle = Quaternion.Euler(0, Random.Range(-mainFireDispersionAngle, mainFireDispersionAngle), 0);


                    BulletGarbage.THIS.GetBullet(shotId).InstantiateMe(spawnPoint.position + fireDispersion,
                                                                     spawnPoint.rotation * fireAngle);
                }*/

                
                for (int i = 0; i < mainEmitters[currentMainEmitterLevel].spawnPoints.Length; i++)
                    {

                    Vector3 fireDispersion = new Vector3(Random.Range(-mainFireDispersion, mainFireDispersion), 0, 0);
                    Quaternion fireAngle = Quaternion.Euler(0, Random.Range(-mainFireDispersionAngle, mainFireDispersionAngle), 0);

                    
                    //Bullet currentBullet = GarbageManager.THIS.GetGameObject(shot).GetComponent<Bullet>();
                    /*currentBullet.InstantiateMe(mainEmitters[currentMainEmitterLevel].spawnPoints[i].position + fireDispersion,
                                                                    mainEmitters[currentMainEmitterLevel].spawnPoints[i].rotation * fireAngle);*/
                    BulletGarbage.THIS.GetBullet(shotId).InstantiateMe(mainEmitters[currentMainEmitterLevel].spawnPoints[i].position + fireDispersion,
                                                                    mainEmitters[currentMainEmitterLevel].spawnPoints[i].rotation * fireAngle);
                    }
            }
            }



        #region Charge bar
        public void ResetChargeBar()
        {
            currentChargeLevel = 0;
            chargingShot = false;

            for (int i = 0; i < chargin_shot.Length; i++)
            {
                chargin_shot[i].chargeBar.value = 0;
                chargin_shot[i].chargeBar.minValue = 0;
                chargin_shot[i].chargeBar.maxValue = maxChargeBarValue;

                if (i > 0)
                    chargin_shot[i].chargeBar.gameObject.SetActive(false);

                if (i == chargin_shot.Length - 1)
                    {
                    if (fullBarSprite == null)
                    {
                        fullBarSprite = chargin_shot[i].chargeBar.transform.GetChild(1).GetChild(0).GetComponent<Image>();
                        fullBarSpriteColor = fullBarSprite.color;
                    }
                    else
                        fullBarSprite.color = fullBarSpriteColor;
                    }
            }
        }

        
        void FillChargeBar()
        {
            chargingShot = true;

            if (currentChargeLevel == chargin_shot.Length -1 && chargin_shot[currentChargeLevel].chargeBar.value == maxChargeBarValue)
            {
                //pulse
                fullBarSprite.color = PulseColor(fullBarSpriteColor, fullBarSpritePulseColor);
                currentChargeLevel = chargin_shot.Length;
            }

            if (currentChargeLevel == chargin_shot.Length)
                return;

            if (chargin_shot[currentChargeLevel].chargeBar.value < maxChargeBarValue)
                chargin_shot[currentChargeLevel].chargeBar.value += charging_unit;
            else
                {
                currentChargeLevel++;
                chargin_shot[currentChargeLevel].chargeBar.gameObject.SetActive(true);
                }
        }

        void ReleaseChargeBar()
        {
            if (!chargingShot)
                return;


                chargingShot = false;

                if (currentChargeLevel == 0)
                    {
                    if (miniShot && chargin_shot[currentChargeLevel].chargeBar.value > 50)
                        {
                    /*
                        Instantiate(miniShot,
                                    mainEmitters[0].spawnPoints[0].position,
                                    mainEmitters[0].spawnPoints[0].rotation);
                                    */
                    BulletGarbage.THIS.GetBullet(miniId).InstantiateMe(mainEmitters[0].spawnPoints[0].position, mainEmitters[0].spawnPoints[0].rotation);
                }
                    else
                        ShotWithMainFire();
                    }
                else
                    {
                /*
                    Instantiate(chargin_shot[currentChargeLevel-1].bullet, 
                                mainEmitters[0].spawnPoints[0].position ,
                                mainEmitters[0].spawnPoints[0].rotation);
                                */
                    BulletGarbage.THIS.GetBullet(chargin_shot[currentChargeLevel - 1].bulletId).InstantiateMe(mainEmitters[0].spawnPoints[0].position, mainEmitters[0].spawnPoints[0].rotation);
                    }

                ResetChargeBar();
                
        }


        #endregion
        #endregion

        #region SecondaryFire
        public void ChangeSecondaryWeapon(int new_weapon)
        {
            if (new_weapon != secondaryWeaponSelected)
                {
                if (secondaryWeapons[secondaryWeaponSelected].shotModel == SecondayFire.ShotModel.keepOnTheSame)
                    secondaryWeapons[secondaryWeaponSelected].reusableWeapon.readyTofire = false;

                nextSecondaryFire = Time.time + 0.25f;
                secondaryWeaponSelected = new_weapon;

                ShumpSceneManager.sceneManager.secondatyWeaponIcon.sprite = secondaryWeapons[secondaryWeaponSelected].icon;
                }
        }

        bool secondaryFireShotOnce = false;
        public void PressSecondaryFire(bool buttondown)
        {
           if (secondaryWeapons[secondaryWeaponSelected].shotModel == SecondayFire.ShotModel.fireASingleShot || secondaryWeapons[secondaryWeaponSelected].shotModel == SecondayFire.ShotModel.fireTheSameOnceWhenButtonIsPressed)
            {
                if (buttondown)
                {
                    if (!secondaryFireShotOnce)
                        pressSecondaryFire = true;
                    else
                        pressSecondaryFire = false;
                }
                else
                {
                    pressSecondaryFire = false;
                    secondaryFireShotOnce = false;
                }
            }
           else
                pressSecondaryFire = buttondown;

        }

        void SecondaryFire()
        {
   
            if (autoRefillEnergyRate > 0)
                energyBar.value += autoRefillEnergyRate * Time.deltaTime;


            if (pressSecondaryFire && Time.time > nextSecondaryFire)
            {
                
                if (secondaryWeapons[secondaryWeaponSelected].energyCost < energyBar.value) //if you have enough energy to shot)
                {
                    secondaryFireShotOnce = true;
                    nextSecondaryFire = Time.time + secondaryWeapons[secondaryWeaponSelected].fireRate;
                    energyBar.value -= secondaryWeapons[secondaryWeaponSelected].energyCost;


                    if (secondaryWeapons[secondaryWeaponSelected].shotModel == SecondayFire.ShotModel.fireASingleShot || secondaryWeapons[secondaryWeaponSelected].shotModel == SecondayFire.ShotModel.autofire)
                        {
                        if (secondaryWeapons[secondaryWeaponSelected].animationName != "")
                            secondaryFireAnimator.Play(secondaryWeapons[secondaryWeaponSelected].animationName, -1, 0f);

                        for (int i = 0; i < secondaryWeapons[secondaryWeaponSelected].emitter.spawnPoints.Length; i++)
                            {
                            BulletGarbage.THIS.GetBullet(secondaryWeapons[secondaryWeaponSelected].shotId).InstantiateMe(secondaryWeapons[secondaryWeaponSelected].emitter.spawnPoints[i].position,
                                                                                                                        secondaryWeapons[secondaryWeaponSelected].emitter.spawnPoints[i].rotation);
                            }

                        PlaySfx(secondaryWeapons[secondaryWeaponSelected].sfx, secondaryWeapons[secondaryWeaponSelected].sfxLoop);
                        }
                    else if (secondaryWeapons[secondaryWeaponSelected].shotModel == SecondayFire.ShotModel.keepOnTheSame)
                    {
                        if (secondaryWeapons[secondaryWeaponSelected].animationName != "" && !secondaryFireAnimator.GetCurrentAnimatorStateInfo(0).IsName(secondaryWeapons[secondaryWeaponSelected].animationName))
                            secondaryFireAnimator.Play(secondaryWeapons[secondaryWeaponSelected].animationName, -1, 0f);

                        secondaryWeapons[secondaryWeaponSelected].reusableWeapon.readyTofire = true;

                        PlaySfx(secondaryWeapons[secondaryWeaponSelected].sfx, secondaryWeapons[secondaryWeaponSelected].sfxLoop);
                    }
                    else if (secondaryWeapons[secondaryWeaponSelected].shotModel == SecondayFire.ShotModel.fireTheSameOnceWhenButtonIsPressed)
                    {
                        secondaryFireShotOnce = true;
                        nextSecondaryFire = Time.time + secondaryWeapons[secondaryWeaponSelected].fireRate;

                        if (secondaryWeapons[secondaryWeaponSelected].animationName != "" && !secondaryFireAnimator.GetCurrentAnimatorStateInfo(0).IsName(secondaryWeapons[secondaryWeaponSelected].animationName))
                            secondaryFireAnimator.Play(secondaryWeapons[secondaryWeaponSelected].animationName, -1, 0f);

                        secondaryWeapons[secondaryWeaponSelected].reusableWeapon.readyTofire = true;

                        PlaySfx(secondaryWeapons[secondaryWeaponSelected].sfx, secondaryWeapons[secondaryWeaponSelected].sfxLoop);
                    }

                    

                }
                else //no enough energy to shot
                    {
                    StopSfx();

                    StartCoroutine(PulseColorOnce(EnergyBarColor, insufficientEnergyToShotColor, EnergyBarColorTarget));

                    if (secondaryWeapons[secondaryWeaponSelected].shotModel == SecondayFire.ShotModel.keepOnTheSame)
                    {
                        secondaryFireAnimator.Play("None");
                        secondaryWeapons[secondaryWeaponSelected].reusableWeapon.readyTofire = false;
                    }

                }
            }
            else
            {
                StopSfx();

                if (secondaryWeapons[secondaryWeaponSelected].shotModel == SecondayFire.ShotModel.keepOnTheSame)
                    {
                    secondaryFireAnimator.Play("None");
                    secondaryWeapons[secondaryWeaponSelected].reusableWeapon.readyTofire = false;
                    }
            }
        }
        #endregion


        void PlaySfx(AudioClip clip, bool loop = false)
            {
                if (clip == myAudioSource.clip)
                    return;

                if (!clip)
                    return;

                myAudioSource.loop = loop;
                if (loop)
                    {
                    myAudioSource.clip = clip;
                    myAudioSource.Play();
                    }
                else
                    myAudioSource.PlayOneShot(clip, ShumpSceneManager.sceneManager.audioEffectsVolume);
            }

        void StopSfx()
        {
            if (!myAudioSource.loop)
                return;

            myAudioSource.Stop();
            myAudioSource.clip = null;
            myAudioSource.loop = false;
        }
    }
}
