using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace shmup
{
    public class StartMenu : MonoBehaviour {

        public Dropdown fireMode;
        public Dropdown secondaryWeapon;
        public Toggle timerToggle;
        public Arsenal arsenal;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartButton(bool rememberBonusLevelInNextStage)
        {
           

            arsenal.fireButtonDown = (Arsenal.FireButtonDown)fireMode.value;
            arsenal.ChangeSecondaryWeapon(secondaryWeapon.value);
            arsenal.StartGUI();
            TimerManager.timerManager.useTimer = timerToggle.isOn;
            TimerManager.timerManager.StartGUI();
            ShumpSceneManager.sceneManager.StartScene(rememberBonusLevelInNextStage);
        }


        
    }


}
