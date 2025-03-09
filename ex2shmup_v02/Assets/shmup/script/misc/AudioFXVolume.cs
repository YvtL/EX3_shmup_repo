using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shmup
{
    public class AudioFXVolume : MonoBehaviour {

    AudioSource myAudioSource;

    // Use this for initialization
    void Awake () {

        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.volume = ShumpSceneManager.sceneManager.audioEffectsVolume;
    }
	

}
}
