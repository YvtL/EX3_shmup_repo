using UnityEngine;
using System.Collections;


namespace shmup
{
    public class ReusableWeapon : Weapon{

        [HideInInspector]
        public bool emissionOn;
        [HideInInspector]
        public bool readyTofire;
        [HideInInspector]
        public float fireRate;

    }
}
