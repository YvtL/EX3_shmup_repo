using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shmup
{
    public class BossTrigger : MonoBehaviour {

    public Boss target;

    [HideInInspector]
    public bool triggered;

    // Use this for initialization
        void Start () {
            ShumpSceneManager.sceneManager.stageBossTrigger = this;

    }

    public virtual void OnTriggerEnter(Collider coll)
    {
            if (triggered)
                return;

        if (coll.tag == "Player")
            {
                TimerManager.timerManager.HideTimer();
                target.ActivateMe();
                triggered = true;
            }
        }
    }
}
