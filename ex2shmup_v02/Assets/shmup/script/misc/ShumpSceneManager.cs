using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace shmup
{
    public class ShumpSceneManager : MonoBehaviour {

        public static ShumpSceneManager sceneManager;

        public Transform scrollingTransform;
        public Transform playerTransform;
        PlayerShip playerShip;

        [Space]
        [Header("Next scene is:")]
        [Tooltip("Use the number associate with that scene in 'Build Settings'")]
        public int sceneToLoadWhenWin;
        [Tooltip("Use the number associate with that scene in 'Build Settings'")]
        public int sceneToLoadWhenLose;

        //GUI
        [Space]
        [Header("GUI")]
        public GameObject startScreen;
        public GameObject gameScreen;
        public GameObject pauseScreen;
        public GameObject winScreen;
        public GameObject gameOverScreen;
        public Transform gameSceneTransform;
        public FloatingScore floatingScoreObj;
        public Text scoreText;
        public Image secondatyWeaponIcon;
        int totalScore;
        //in order to have olny one gui score animation corutine
            IEnumerator coroutine;
            int tempScore;
            int scoreAnimations = 0;

        /*
        [Space]
        [Header("Garbages")]
        public Transform scoreGarbage;
        public Transform sparksGarbage;
        */
        //elements to reset when restart
        [Space]
        [Header("Element to reset at restart")]
        public EnemyEmitterManager enemyEmitterManager;
        public FollowWaypoints followWaypoints;
        public Transform bulletsParent;
        List<GameObject> elementsOnMapAtStart;//example: turrets and ostacles
        //[HideInInspector]
        public List<Rotation> rotationToReset;

        [HideInInspector]
        public BossTrigger stageBossTrigger;
        [HideInInspector]
        public Boss stageBoss;
        //[HideInInspector]
        //public bool sceneEnd;
        public enum SceneStatus
        {
            WaitingStart,
            Playing,
            Paused,
            sceneEnd
        }
        [HideInInspector]
        public SceneStatus currentSceneStatus;
        public SceneStatus initialSceneStatus;


        //audio
        [Space]
        [Header("Audio")]
        [Range(0,1)]
        public float audioEffectsVolume;
        public AudioClip enemyShotGenericSfx;
        public AudioClip enemyGetHitGenericSfx;

        [SerializeField] bool rememberBonusLevelInNextStage;
        [SerializeField] Arsenal arsenal;

        void Awake()
        {

            sceneManager = this;
            playerShip = playerTransform.GetComponent<PlayerShip>();
            elementsOnMapAtStart = new List<GameObject>();
        }

        // Use this for initialization
        void Start () {
            StartGUI();
        }


        void StartGUI()
        {
            totalScore = 0;
            scoreText.text = "Score: " + totalScore.ToString("n0");
            currentSceneStatus = initialSceneStatus;

            gameOverScreen.SetActive(false);
            winScreen.SetActive(false);
            pauseScreen.SetActive(false);

            TimerManager.timerManager.StartGUI();

            if (currentSceneStatus == SceneStatus.WaitingStart)
                {
                gameScreen.SetActive(false);
                startScreen.SetActive(true);
                }
            else
                StartScene(rememberBonusLevelInNextStage);
        }



        public void StartScene(bool _rememberBonusLevelInNextStage) {
            startScreen.SetActive(false);
            gameScreen.SetActive(true);

            rememberBonusLevelInNextStage = _rememberBonusLevelInNextStage;
            if (rememberBonusLevelInNextStage && PlayerPrefs.GetInt("rememberBonusInNextStage") > 0)
                {
                    arsenal.currentMainEmitterLevel = PlayerPrefs.GetInt("currentMainEmitterLevel");
                    arsenal.ChangeSecondaryWeapon(PlayerPrefs.GetInt("secondaryWeaponValue"));
                }
            


            currentSceneStatus = SceneStatus.Playing;
            TimerManager.timerManager.ResetTimer();
        }

    public void BecomeChildOfScrolling(Transform child)
            {
            child.SetParent(scrollingTransform);
            }

    public void ShowFloatingText(string thisText, Vector3 location)
        {
            //floating text animation

            GameObject instanceObj = GarbageManager.THIS.GetGameObject(floatingScoreObj.gameObject, 10);
            FloatingScore instance = instanceObj.GetComponent<FloatingScore>();

            /*
            FloatingScore instance = null;
            if (scoreGarbage.childCount == 0)
            {
                instance = Instantiate(floatingScoreObj);
            }
            else
            {
                instance = scoreGarbage.GetChild(0).GetComponent<FloatingScore>();
                instance.transform.SetParent(null);

                if (floatingScoreObj == null)
                    floatingScoreObj = instance;
            }*/

            instance.SetText(thisText);
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(location);
            instance.transform.SetParent(gameSceneTransform, false);
            instance.transform.position = screenPosition;
            instance.gameObject.SetActive(true);
            
        }

    public void GainScore(int score, Transform location)
        {
            ShowFloatingText(score.ToString("n0"), location.position);

            //total score update animation
            if (scoreAnimations > 0)
                {
                totalScore += tempScore;
                tempScore = 0;
                scoreText.text = "Score: " + totalScore.ToString("n0");
                StopCoroutine(coroutine);
                scoreAnimations--;
                }
            coroutine = ScoreAnimation(score);
            StartCoroutine(coroutine);

        }

    IEnumerator ScoreAnimation(int score)
        {
            scoreAnimations++;
            tempScore = score;

            float duration = 0.5f;
            float timer = 0;

            int targetScore = (totalScore + score);
            int startScore = totalScore;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                startScore = Mathf.CeilToInt(Mathf.Lerp(startScore, targetScore, (timer / duration)));
                scoreText.text = "Score: " + startScore.ToString("n0");
                yield return null;
            }

            totalScore = targetScore;
            scoreAnimations--;
        }

    public void Pause(bool openPause)
        {
            if (openPause)
                {
                currentSceneStatus = SceneStatus.Paused;
                Time.timeScale = 0;
                pauseScreen.SetActive(true);
                }
            else
                {
                currentSceneStatus = SceneStatus.Playing;
                pauseScreen.SetActive(false);
                Time.timeScale = 1;
                }
        }

    public void Win()
        {
            print("Win");
            playerTransform.GetComponent<PlayerController>().InputOnOff(false);
            currentSceneStatus = SceneStatus.sceneEnd;
            Time.timeScale = 0;
            winScreen.SetActive(true);

            if (rememberBonusLevelInNextStage)
            {
                PlayerPrefs.SetInt("rememberBonusInNextStage", 1);

                PlayerPrefs.SetInt("currentMainEmitterLevel", arsenal.currentMainEmitterLevel);
                PlayerPrefs.SetInt("secondaryWeaponValue", arsenal.secondaryWeaponSelected);
            }
            else
                PlayerPrefs.SetInt("rememberBonusInNextStage", 0);
        }

    public void LoadSceneWhenWin()
        {
            SceneManager.LoadScene(sceneToLoadWhenWin);
        }

    public void LoadSceneWhenLose()
        {
            SceneManager.LoadScene(sceneToLoadWhenLose);
        }

        public void GameOver()
        {
            print("GameOver");
            playerTransform.GetComponent<PlayerController>().InputOnOff(false);
            currentSceneStatus = SceneStatus.sceneEnd;
            PlayerPrefs.SetInt("rememberBonusInNextStage", 0);
            Time.timeScale = 0;
            gameOverScreen.SetActive(true);
        }

    public void Restart()
        {
            //reset
            DestroyBullets();
            DestryBonuses();
            enemyEmitterManager.ResetEmitters();
            followWaypoints.Reset();
            stageBossTrigger.triggered = false;
            stageBoss.ResetMe();

            StartGUI();

            //restart
            ReEnableGameObjectsOnMap();
            for (int i = 0; i < rotationToReset.Count; i++)
                rotationToReset[i].ResetMe();
            playerShip.RestartMe();
            enemyEmitterManager.StartMe();
            Time.timeScale = 1;
        }

    void DestryBonuses()
        {
            GameObject[] bonuses = GameObject.FindGameObjectsWithTag("Bonus");
            
            for (int i = 0; i < bonuses.Length; i++)
            {
                if (bonuses[i].GetComponent<PermamentElementOnMap>() == null)
                    Destroy(bonuses[i]);
            }
        }

    void DestroyBullets()
        {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

            for (int i = 0; i < bullets.Length; i++)
                bullets[i].GetComponent<Bullet>().DestroyMe();
        }


    public void EnableThisGameObjectAtEachRestart(GameObject thisObj)
        {
            elementsOnMapAtStart.Add(thisObj);
        }

    void ReEnableGameObjectsOnMap()
        {
            for (int i = 0; i < elementsOnMapAtStart.Count; i++)
            {
                elementsOnMapAtStart[i].SetActive(true);

                Enemy enemyScript = elementsOnMapAtStart[i].GetComponent<Enemy>();
                if (enemyScript)
                    {
                    enemyScript.RestoreMe();
                    continue;
                    }

                Bonus bonusScript = elementsOnMapAtStart[i].GetComponent<Bonus>();
                if (bonusScript)
                    {
                    bonusScript.SelectBonus();
                    continue;
                    }
                }
        }


    }
}
