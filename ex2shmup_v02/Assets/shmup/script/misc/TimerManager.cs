using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace shmup
{
    public partial class TimerManager : MonoBehaviour{

        public bool useTimer;
        public float timer;
        public enum TimeRule
        {
            LoseALive,
            LoseTheStage
        }
        public TimeRule whenTimeUp;

        public static TimerManager timerManager;
        public PlayerShip playerShip;
        public Text timerText;

        [HideInInspector]
        public float startTime;
        [HideInInspector]
        public bool pauseTime;
        float pauseDuration;

        float timeLeft;
        float timeBonus;

        void Awake()
        {
            timerManager = this;
        }

        public void StartGUI()
        {
            timerText.gameObject.SetActive(useTimer);
            timeLeft = startTime;
        }

        void Update()
        {
            Timer();
        }

        void UpdateTimerGUI(float currentTime)
        {
            TimeSpan t = TimeSpan.FromSeconds(timeLeft);
            //seconds
            string ss;
            if (t.Seconds < 10)
                ss = "0" + t.Seconds.ToString("n0");
            else
                ss = t.Seconds.ToString("n0");
            //minutes
            string mm;
            if (t.Minutes < 10)
                mm = "0" + t.Minutes.ToString("n0");
            else
                mm = t.Minutes.ToString("n0");

            if (currentTime < 60)
                {
                timerText.text = ss;
                }
            else
                timerText.text = mm + ":" +ss;
        }

        void Timer()
        {
            if (pauseTime)
            {
                pauseDuration += Time.deltaTime;
                return;
            }

            if (useTimer && ShumpSceneManager.sceneManager.currentSceneStatus == ShumpSceneManager.SceneStatus.Playing)
                {
                timeLeft = (timer + startTime + timeBonus + pauseDuration) - Time.timeSinceLevelLoad;

                UpdateTimerGUI(timeLeft);

                if (timeLeft <= 0)
                    {
                    timeLeft = 0;
                    if (whenTimeUp == TimeRule.LoseALive)
                        {
                        playerShip.HitMe();
                        ResetTimer();
                        }
                    else if (whenTimeUp == TimeRule.LoseTheStage)
                        ShumpSceneManager.sceneManager.GameOver();
                    }
                else if (timeLeft > timer)
                    timeLeft = timer;
                }
        }


        public void GainTime(float addTime, Vector3 position)
        {
            timeBonus += addTime;
            ShumpSceneManager.sceneManager.ShowFloatingText("+ " + addTime.ToString("n0") + " seconds", position + new Vector3(0, -1, 0));
        }

        public void HideTimer()
        {
            pauseTime = true;
            timerText.gameObject.SetActive(false);
        }

        public void ResetTimer()
        {
            startTime = Time.timeSinceLevelLoad;
            pauseDuration = 0;
            timeBonus = 0;
            timeLeft = timer;
            pauseTime = false;
            UpdateTimerGUI(timeLeft);
        }

    }
}
