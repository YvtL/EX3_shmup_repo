using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace shmup
{
    public class PlayerShip : MonoBehaviour {

    public int startLives;
    int currentLives;
    public Text livesUI;

    public float invulnerabilityDuration;
    bool invulnerable;

    public Collider avatarCollider;
    public Collider boundCollider;
        Renderer avatarRenderer;
       Color avatarOriginalColor;
        Color invulnerableColor;
    public float respawnSpeed;
    public Transform respawnPoint;
        Transform myTransform;
    bool respawnAnimationOn;

    public Transform startPoint;
        //Vector3 startPosition; 
        public GameObject explosionFx;

        PlayerController playerController;
        Arsenal arsenal;

        // Use this for initialization
        void Start () {
            myTransform = transform;
           // startPosition = myTransform.position;
            playerController = GetComponent<PlayerController>();
            arsenal = GetComponent<Arsenal>();
            avatarRenderer = avatarCollider.GetComponent<Renderer>();
            avatarOriginalColor = avatarRenderer.material.color;
            invulnerableColor = new Color(avatarOriginalColor.r, avatarOriginalColor.g, avatarOriginalColor.b, avatarOriginalColor.a * 0.5f);

            RestartMe();
        }
	
	// Update is called once per frame
	void Update () {
            Respawn();

    }

    public void RestartMe()
        {
            playerController.InputOnOff(false);
            currentLives = startLives;
            arsenal.currentMainEmitterLevel = 0;
            UpdateUI();

            myTransform.position = respawnPoint.position;
            invulnerable = true;
            avatarRenderer.material.color = invulnerableColor;
            avatarCollider.enabled = false;
            boundCollider.enabled = false;
            respawnAnimationOn = true;
        }

    public void HitMe()
    {
        if (ShumpSceneManager.sceneManager.currentSceneStatus != ShumpSceneManager.SceneStatus.Playing)
            return;

        if (invulnerable)
            return;
        else
            {
            print("player lose a live");
            currentLives--;
                UpdateUI();
            invulnerable = true;
                arsenal.ResetChargeBar();
            playerController.InputOnOff(false);
            arsenal.currentMainEmitterLevel = 0;
            TimerManager.timerManager.pauseTime = true;

                if (explosionFx)
                    {
                    GameObject fxTemp =  (GameObject)Instantiate(explosionFx, myTransform.position,Quaternion.identity);
                    fxTemp.transform.SetParent(respawnPoint);
                    }

                myTransform.position = respawnPoint.position;

                if (currentLives > 0)
                {
                    avatarRenderer.material.color = invulnerableColor;
                    avatarCollider.enabled = false;
                    boundCollider.enabled = false;
                    respawnAnimationOn = true;
                }
                else
                    Invoke("GameOver", 0.5f);
            }
    }

    public void GainLives(int lives)
        {
            currentLives += lives;
            UpdateUI();
        }

    public void UpdateUI()
        {
            livesUI.text = currentLives.ToString("n0");
        }
    

    void Respawn()
        {
            if (ShumpSceneManager.sceneManager.currentSceneStatus != ShumpSceneManager.SceneStatus.Playing)
                return;

            if (!respawnAnimationOn)
                return;

            float step = respawnSpeed * Time.deltaTime;
            myTransform.position = Vector3.MoveTowards(myTransform.position, startPoint.position, step);
            TimerManager.timerManager.pauseTime = true;

            float dist = Vector3.Distance(startPoint.position, myTransform.position);
            if (dist < 0.01f)
            {
                respawnAnimationOn = false;
                boundCollider.enabled = true;
                playerController.InputOnOff(true);
                Invoke("InvulnerabilityOff",invulnerabilityDuration);
                TimerManager.timerManager.pauseTime = false;
            }
        }

    void InvulnerabilityOff()
        {
            invulnerable = false;
            avatarCollider.enabled = true;
            avatarRenderer.material.color = avatarOriginalColor;
        }

       void GameOver()
        {
            ShumpSceneManager.sceneManager.GameOver();
        }
    }
}
