using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shmup
{
    public class PermamentElementOnMap : MonoBehaviour {

	// Use this for initialization
	void Start () {
            ShumpSceneManager.sceneManager.EnableThisGameObjectAtEachRestart(this.gameObject);
    }

    }
}
