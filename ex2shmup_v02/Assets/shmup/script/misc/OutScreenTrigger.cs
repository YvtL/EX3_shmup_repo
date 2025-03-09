using UnityEngine;
using System.Collections;

namespace shmup
{
    public class OutScreenTrigger : MonoBehaviour {

        public Bullet bulletScript;


    void OnBecameInvisible()
        {
            if (bulletScript)
                bulletScript.ExitFromScreen();
        }
    }
}
