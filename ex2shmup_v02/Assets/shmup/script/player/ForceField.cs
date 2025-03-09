using UnityEngine;
using System.Collections;

namespace shmup
{
    public class ForceField : ReusableWeapon
    {

        Collider myCollider;
        Renderer myRenderer;

        void Awake()
        {
            destroyMeAtContact = false;
            myCollider = GetComponent<Collider>();
            myRenderer = GetComponent<Renderer>();
        }


        // Update is called once per frame
        void Update () {

            myCollider.enabled = readyTofire;
            myRenderer.enabled = readyTofire;



    }


}
}
