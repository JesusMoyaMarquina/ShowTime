using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    public Vector3 leftPosition, rightPosition;
    private Player player;
    private Weapon weapon;
    private bool hitted;

    private void Awake()
    {
        player = transform.parent.GetComponentInParent<Player>();
        weapon = GetComponentInParent<Weapon>();
    }

    private void OnEnable()
    {
        hitted = false;

        if (player.GetDirection().x > 0)
        {
            transform.localPosition = rightPosition;
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            transform.localPosition = leftPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
        if (enemy != null && !enemy.hitted && enemy.isAlive())
        {
            if (!hitted)
            {
                hitted = true;
                player.ReduceCooldown();
            }

            enemy.Knockback(weapon.MngList[player.inputQueue.Count], player.GetComponent<SpriteRenderer>().flipX ? Vector2.right : Vector2.left);

            enemy.GetDamage(player.executedAttack.GetDamage());

            if (CombatManager.instance != null)
            {
                CombatManager.instance.ComboSystem(true);
            }

            if (!enemy.isAlive())
            {
                enemy.AddScore();
                player.Heal(enemy.totalHealth * (weapon.GetLifeSteal() / 100));
            }
        }
    }
}
