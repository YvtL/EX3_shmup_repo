using UnityEngine;
using System.Collections;

namespace shmup
{
    public class Weapon : MonoBehaviour {

        public enum Harm
        {
            enemy,
            player
        }
        public Harm harm;
        public float damage;
        [HideInInspector]
        public bool destroyMeAtContact;

        public void IdentificateEnemy(GameObject thisEnemy)
        {
            Enemy enemy = thisEnemy.gameObject.GetComponentInParent<Enemy>();
            if (enemy)
            {
                if (enemy.enemyType == Enemy.EnemyType.ship)
                    enemy.TakeDamage(damage);
                else if (enemy.enemyType == Enemy.EnemyType.mine)
                {
                    Mine mine = thisEnemy.gameObject.GetComponentInParent<Mine>();
                    mine.TakeDamage(damage);
                }
                else if (enemy.enemyType == Enemy.EnemyType.bossElement)
                {
                    BossElement bossElement = thisEnemy.gameObject.GetComponent<BossElement>();
                    if(bossElement)
                        bossElement.TakeDamage(damage);
                }
            }
        }

        public virtual void OnTriggerEnter(Collider coll)
        {
            if (harm == Harm.enemy)
            {
                IdentificateEnemy(coll.gameObject);
            }
            else if (harm == Harm.player)
            {
                //ForceField forcefield = coll.gameObject.GetComponentInParent<ForceField>();
                if (coll.tag == "ForceField")
                    {
                    DestroyMe();
                    return;
                    }

                PlayerShip player = coll.gameObject.GetComponentInParent<PlayerShip>();
                if (player)
                {
                    player.HitMe();
                }
            }
            //don't destroy if hit another bullet
            Bullet bullet = coll.gameObject.GetComponentInParent<Bullet>();
            if (bullet == null)
                DestroyMe();
        }

        public virtual void DestroyMe()
        {
            /*
            if (destroyMeAtContact)
                Destroy(this.gameObject);*/
        }
    }
}
