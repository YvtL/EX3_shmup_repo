using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shmup
{
    public class Spark : MonoBehaviour {

    Transform myTransform;
    ParticleSystem myParticleSystem;

    void Awake()
    {
        myTransform = transform;
        myParticleSystem = GetComponent<ParticleSystem>();
            SparkOn();

    }

    void SparkOn()
        {
            myTransform.SetParent(null);
            this.gameObject.SetActive(true);
            myParticleSystem.Play();
            Invoke("PutMeInGarbage", myParticleSystem.main.duration);
        }

        // Use this for initialization
        void Start () {
		
	}
	
    void PutMeInGarbage()
    {
            this.gameObject.SetActive(false);
            //myTransform.SetParent(ShumpSceneManager.sceneManager.sparksGarbage);
    }

    public void RecycleMe(Vector3 newPosition)
    {
        myTransform.position = newPosition;
        SparkOn();
    }
}
}
