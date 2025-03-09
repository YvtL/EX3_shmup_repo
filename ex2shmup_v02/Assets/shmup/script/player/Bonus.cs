using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace shmup
{
    public class Bonus : MonoBehaviour {

    public enum TriggerRequested
        {
            GrabMe,
            ShotMe
        }
    public TriggerRequested triggerRequested = TriggerRequested.GrabMe;
    bool waitForDestroyMe;

    //these variables allow to cycle troug the bonues, instead of picking one randombly at start
    [Tooltip("If 0 will it will pick a random bonus. If > 0 it will cycle trough all the bonues from a random starting point")]
    public float changeBonusEachXSeconds = 0;
    GameObject[] bonusAvatars;

    [System.Serializable]
    public class BonusType
    {
            public string name;
            public BonusTypology typology;
            public int odds;
            public GameObject Avatar;
            public Sprite icon;
            public AudioClip pickAudioFx;

    }
    public BonusType[] bonuses;

    public enum BonusTypology
    {
        live = 0,
        energy = 1,
        upgradePrimaryFire = 2,
        //secondaryEmitter = 3,
        missile = 4,
        laserBeam = 5,
        forceField = 6,
        spear = 7
    }

    [Tooltip("How much nergy give if the bonus is 'energy'")]
    public float energy = 10;
    AudioSource myAudioSource;
        GameObject avatar;
        BoxCollider myCollider;

        int totalOdds;
        List<int> bonusDeck;
        int randomBonus;
        BonusTypology bonusTypologySelected;
        

        // Use this for initialization
        void Start () {
            myAudioSource = GetComponent<AudioSource>();
            myCollider = GetComponent<BoxCollider>();

            randomBonus = -1;

            GenerateOddsDeck();

            if (changeBonusEachXSeconds > 0)
                GenerateAllAvaibleBonues();
            else
                SelectBonus();

        //set my position at the same height of the player
        Vector3 temp = this.gameObject.transform.position;
        temp.y = GameObject.FindGameObjectWithTag("Player").transform.position.y;
        this.gameObject.transform.position = temp;
        waitForDestroyMe = false;
    }

    void GenerateAllAvaibleBonues()
        {
            bonusAvatars = new GameObject[bonuses.Length];

            for (int i = 0; i < bonuses.Length; i++)
                {
                bonusAvatars[i] = (GameObject)Instantiate(bonuses[i].Avatar, transform.position, Quaternion.identity);
                bonusAvatars[i].transform.SetParent(this.gameObject.transform);
                bonusAvatars[i].SetActive(false);
                }

            randomBonus = Random.Range(0, bonuses.Length);
            bonusTypologySelected = bonuses[randomBonus].typology;
            avatar = bonusAvatars[randomBonus];
            avatar.SetActive(true);

            Invoke("RefreshBonus", changeBonusEachXSeconds);

        }

    void RefreshBonus()
        {
            avatar.SetActive(false);
            Quaternion previousRotation = avatar.transform.rotation;

            randomBonus++;
            if (randomBonus >= bonuses.Length)
                randomBonus = 0;

            bonusTypologySelected = bonuses[randomBonus].typology;
            avatar = bonusAvatars[randomBonus];
            avatar.transform.rotation = previousRotation;
            avatar.SetActive(true);

            Invoke("RefreshBonus", changeBonusEachXSeconds);
        }

    void OnTriggerEnter(Collider otherObject)
    {

        if (triggerRequested == TriggerRequested.GrabMe && otherObject.tag == "Player")
            {
                GetBonus();
                return;
            }

        if (triggerRequested == TriggerRequested.ShotMe)
            {
                if (otherObject.tag == "Bullet" && otherObject.GetComponent<Weapon>().harm == Weapon.Harm.enemy)
                {
                    GetBonus();
                    return;
                }
            }
        }

    public void LaserHitMe()
        {
            if (triggerRequested != TriggerRequested.ShotMe)
                return;

            if (waitForDestroyMe)
                return;

            waitForDestroyMe = true;
            Invoke("GetBonus", 0.25f);//need some time or player can't see the laser hit this bonus
        }

        void GetBonus()
        {
            Debug.Log("GetBonus");
            switch (bonusTypologySelected)
            {
                case BonusTypology.live:
                    ShumpSceneManager.sceneManager.playerTransform.GetComponent<PlayerShip>().GainLives(1);
                    break;

                case BonusTypology.energy:
                    ShumpSceneManager.sceneManager.playerTransform.GetComponent<Arsenal>().energyBar.value += energy;
                    break;

                case BonusTypology.upgradePrimaryFire:
                    ShumpSceneManager.sceneManager.playerTransform.GetComponent<Arsenal>().UpgradePrimaryFire();
                    break;

                //case BonusTypology.secondaryEmitter:
                //break;

                default:
                    ShumpSceneManager.sceneManager.playerTransform.GetComponent<Arsenal>().ChangeSecondaryWeapon((int)bonusTypologySelected - 4);
                    break;
            }


            myCollider.enabled = false;
            avatar.SetActive(false);
            
            ShumpSceneManager.sceneManager.ShowFloatingText(bonuses[randomBonus].name, transform.position);

            if (myAudioSource && bonuses[randomBonus].pickAudioFx)
            {
                myAudioSource.PlayOneShot(bonuses[randomBonus].pickAudioFx, ShumpSceneManager.sceneManager.audioEffectsVolume);
                Invoke("DestroyMe", bonuses[randomBonus].pickAudioFx.length);
            }
            else
                DestroyMe();
        }

    void DestroyMe()
    {
        CancelInvoke();

        if (GetComponent<PermamentElementOnMap>())
            {
                this.gameObject.SetActive(false);
                return;
            }

           // Destroy(this.gameObject.transform.GetChild(0));
            Destroy(this.gameObject);
        }


    void GenerateOddsDeck()
        {
            bonusDeck = new List<int>();

            for (int i = 0; i < bonuses.Length; i++)
            {
                for (int odds = 0; odds < bonuses[i].odds; odds++)
                    {
                    if (bonuses[i].odds > 0)
                        bonusDeck.Add(i);
                    }

            }
        }

        public void SelectBonus()
        {
            // if (transform.childCount > 0)
            //    Destroy(transform.GetChild(0));

            randomBonus = bonusDeck[Random.Range(0, bonusDeck.Count)];
            bonusTypologySelected = bonuses[randomBonus].typology;

            avatar = (GameObject)Instantiate(bonuses[randomBonus].Avatar, transform.position, Quaternion.identity);
            avatar.transform.SetParent(this.gameObject.transform);

            this.gameObject.SetActive(true);
            myCollider.enabled = true;
            waitForDestroyMe = false;


        }
    }
}
