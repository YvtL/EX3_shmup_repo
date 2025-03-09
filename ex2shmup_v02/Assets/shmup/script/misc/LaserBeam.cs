using UnityEngine;
using System.Collections;

namespace shmup
{
    public class LaserBeam : ReusableWeapon
    {

        [Space]
        public float laserLength;
        //raycast
        public Transform[] raycastStartPoints;
        LineRenderer lineRenderer;
        Ray[] rays;
        RaycastHit[] hits;

        public Transform sparksPosition;

        public enum EmissionBehavior
        {
            alwaysOn,
            intermittent,
            atRequest
        }
        public EmissionBehavior emissionBehavior;

        [Space]
        [Header("Intermittent")]
        public float pauseDuration;
        float pauseCount;
        public float warningFxDuration;
        float warningFxCount;
        public GameObject warningFx;
        bool warningFxOn;
        public float emissionDuration;
        float emissionCount;


        //damage speed
        float frequence = 0.1f;
        float lastHit;


        void Awake()
        {
            rays = new Ray[raycastStartPoints.Length];
            hits = new RaycastHit[raycastStartPoints.Length];

            lineRenderer = this.GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
        }

        void OnDisable()
        {
            VisualEffectON(false);
        }

        // Use this for initialization
        void Start () {

            if (emissionBehavior == EmissionBehavior.alwaysOn)
                readyTofire = true;
            else
                {
                readyTofire = false;
                pauseCount = Time.timeSinceLevelLoad + pauseDuration;
                }

        }
	
	    // Update is called once per frame
	    void Update () {

            if (ShumpSceneManager.sceneManager.currentSceneStatus != ShumpSceneManager.SceneStatus.Playing)
                return;

            for (int i = 0; i < rays.Length; i++)
                {
                //rays[i] = new Ray(raycastStartPoints[i].position, raycastStartPoints[i].forward);
                rays[i].origin = raycastStartPoints[i].position;
                rays[i].direction = raycastStartPoints[i].forward;
                //Debug.DrawRay(raycastStartPoints[i].position, raycastStartPoints[i].forward * laserLength, Color.blue);
            }

            lineRenderer.SetPosition(0, raycastStartPoints[0].position);

            if (emissionBehavior == EmissionBehavior.intermittent)
            {
                if (readyTofire)
                {
                    if (Time.timeSinceLevelLoad > emissionCount)
                    {
                        pauseCount = Time.timeSinceLevelLoad + pauseDuration;
                        readyTofire = false;
                    }
                }
                else
                {
                    if (warningFxOn)
                        {
                        if (Time.timeSinceLevelLoad > warningFxCount)
                            {
                            warningFxOn = false;
                            warningFx.SetActive(warningFxOn);

                            emissionCount = Time.timeSinceLevelLoad + emissionDuration;
                            readyTofire = true;
                            }
                        }
                    else
                        {
                        if (Time.timeSinceLevelLoad > pauseCount)
                            {
                            warningFxCount = Time.timeSinceLevelLoad + warningFxDuration;
                            warningFxOn = true;
                            warningFx.SetActive(warningFxOn);
                            }
                        }
                }
            }

            LaserShot();

        }

        void VisualEffectON(bool showNow)
        {
            sparksPosition.gameObject.SetActive(showNow);
            lineRenderer.enabled = showNow;
        }

        void LaserShot()
        {
            VisualEffectON(readyTofire);

            if (!readyTofire)
                return;

            
            float hitDistance = laserLength;
            Vector3 hitPoint = Vector3.zero;

            //exclude the bounds collider from the raycast
            int layerMask = 1 << 10; //collide only with layer 10 (ScreenBounds)
            layerMask = ~layerMask; //collide with all layer except 10

            //find laser lenght
            for (int i = 0; i < rays.Length; i++)
            {
                if (Physics.Raycast(rays[i].origin, rays[i].direction, out hits[i], laserLength, layerMask))
                {

                    if ((hits[i].transform.tag == "Bullet"))//ignore these collisions
                        continue;

                    if (hits[i].transform.tag == "Bonus")
                    {
                        Bonus bonusScript = hits[i].transform.GetComponent<Bonus>();
                        if (bonusScript && bonusScript.triggerRequested == Bonus.TriggerRequested.ShotMe)
                            hits[i].transform.GetComponent<Bonus>().LaserHitMe();
                        else
                            continue;
                    }

                    if (hits[i].distance < hitDistance)
                        hitDistance = hits[i].distance;

                }
            }

            

            //player deal damage
            for (int i = 0; i < rays.Length; i++)
            {
                //find laser lenght
                if (Physics.Raycast(rays[i].origin, rays[i].direction, out hits[i], hitDistance + 0.01f, layerMask))
                {


                    if ((hits[i].transform.tag == "Bullet") || (hits[i].transform.tag == "Bonus"))//ignore these collisions
                        continue;
                    /*
                    if ((harm == Harm.player) && (hits[i].transform.tag == "Player")) //if enemy laser hit the player
                        {
                       hits[i].collider.gameObject.GetComponent<PlayerShip>().HitMe();
                        }*/

                    //print("hits[i].transform.tag " + hits[i].transform.tag + " ... "+hits[i].transform.gameObject.name);

                    //enemy deal damage
                    if (harm == Harm.player && (hits[i].transform.tag == "PlayerShip"))
                    {
                        hits[i].transform.GetComponent<PlayerShip>().HitMe();
                    }
                    else if ((harm == Harm.enemy) && (hits[i].transform.tag == "Enemy")) //player laser hit enemy
                    {
                        if (Time.timeSinceLevelLoad > lastHit)
                        {

                            IdentificateEnemy(hits[i].collider.gameObject);

                            lastHit = frequence + Time.timeSinceLevelLoad;
                        }
                    }
                }

                Debug.DrawRay(raycastStartPoints[i].position, raycastStartPoints[i].forward * hitDistance, Color.yellow);
            }

            hitPoint = rays[0].origin + rays[0].direction * hitDistance;

            sparksPosition.position = hitPoint;
            //lineRenderer.SetVertexCount(2);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(1, hitPoint);

        }
    }
}