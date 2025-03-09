using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace shmup
{
    public class EnemyAvatar : MonoBehaviour {

        [HideInInspector]
        public Enemy enemy;
        [SerializeField] bool isGroundUnit = false;
        [SerializeField] UnityEvent OnScreenExit = null;

        void Start()
        {
            enemy = GetComponentInParent<Enemy>();
        }


        public void TakeDamage(float damage)
        {
            if (enemy.enemyType == Enemy.EnemyType.ship)
                enemy.TakeDamage(damage);
            else if (enemy.enemyType == Enemy.EnemyType.mine)
            {
                Mine mine = GetComponentInParent<Mine>();
                mine.TakeDamage(damage);
            }

        }

        void OnTriggerEnter(Collider otherObject)
        {
            if (!isGroundUnit && otherObject.tag == "Player")
            {
                ShumpSceneManager.sceneManager.playerTransform.GetComponent<PlayerShip>().HitMe();
            }
        }


        void OnBecameInvisible()
        {
            //Debug.Log("OnScreenExit");
            OnScreenExit.Invoke();
        }
    }
}
