using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    public Vector3 leftPosition, rightPosition;
    private Player player;
    private Weapon weapon;

    private void Awake()
    {
        player = transform.parent.GetComponentInParent<Player>();
        weapon = GetComponentInParent<Weapon>();
    }

    private void OnEnable()
    {
        if (player.GetDirection().x > 0)
        {
            transform.localPosition = rightPosition;
        }
        else
        {
            transform.localPosition = leftPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
        if (enemy != null && !enemy.hitted && enemy.isAlive())
        {
            enemy.Knockback(weapon.MngList[player.inputQueue.Count], player.GetComponent<SpriteRenderer>().flipX ? Vector2.right : Vector2.left);

            enemy.GetDamage(player.executedAttack.GetDamage());

            CombatManager.instance.ComboSystem(true);

            if (!enemy.isAlive())
            {
                enemy.AddScore();
                player.Heal(enemy.totalHealth * (weapon.GetLifeSteal() / 100));
            }
        }
    }
}
