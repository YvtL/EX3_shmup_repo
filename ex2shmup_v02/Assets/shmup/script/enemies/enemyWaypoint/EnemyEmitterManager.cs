using UnityEngine;
using System.Collections;

namespace shmup
{
    public class EnemyEmitterManager : MonoBehaviour {

    [System.Serializable]
    public class EnemyEmission
    {
	    public EnemyEmitter emitter;
            public EnemyEmitterBezier emitterBezier;
        public float delay;

    }

    public EnemyEmission[] emitters;
    int count;
    float nextEmission;
    bool endReached;

    // Use this for initialization
    void Start () {
        StartMe();

    }

    public void StartMe()
    {
        count = 0;
        nextEmission = Time.timeSinceLevelLoad + emitters[count].delay;
    }

    // Update is called once per frame
    void Update () {

            if (endReached)
                return;

            if (ShumpSceneManager.sceneManager.currentSceneStatus != ShumpSceneManager.SceneStatus.Playing)
                return;

        if (count >= emitters.Length)
            return;

        if (Time.timeSinceLevelLoad > nextEmission)
        {
                if (emitters[count].emitter)
                    emitters[count].emitter.ActivateMe();
                else if (emitters[count].emitterBezier)
                        emitters[count].emitterBezier.ActivateMe();

                count++;
                if (emitters.Length > count)
                    nextEmission = Time.timeSinceLevelLoad + emitters[count].delay;
                else
                    endReached = true;
            
        }

    }

    public void ResetEmitters()
        {
            endReached = false;
            for (int i = 0; i < emitters.Length; i++)
            {
                if (emitters[i].emitter)
                    emitters[i].emitter.ResetMe();
                else if (emitters[i].emitterBezier)
                    emitters[i].emitterBezier.ResetMe();
            }
        }
    }
}