using UnityEngine;
using System.Collections;

namespace shmup
{
    public class Melee : ReusableWeapon {

        Animation myAnimation;
        float anim_duration;
        public Weapon[] blades;
        bool animantionOngoing;

        // Use this for initialization
        void Awake () {

            destroyMeAtContact = false;

            for (int i = 0; i < blades.Length; i++)
                {
                blades[i].gameObject.SetActive(false);
                blades[i].damage = damage;
                blades[i].harm = harm;
                }

            myAnimation = GetComponent<Animation>();
            anim_duration = myAnimation.clip.length;
            fireRate = anim_duration + 0.1f;

        }
	
	// Update is called once per frame
	void Update () {
            MeeleAttack();

    }

    void MeeleAttack()
        {
            if (!readyTofire || animantionOngoing)
                return;

            for (int i = 0; i < blades.Length; i++)
                blades[i].gameObject.SetActive(true);

            myAnimation.Play();

            readyTofire = false;
            animantionOngoing = true;

            Invoke("Reset", anim_duration);
        }
    

    void Reset()
    {
        animantionOngoing = false;

        for (int i = 0; i < blades.Length; i++)
            blades[i].gameObject.SetActive(false);
    }

    
    }
}
