using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    public Vector3 leftPosition, rightPosition;
    private Player player;

    private void Awake()
    {
        player = transform.parent.GetComponentInParent<Player>();
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
        if (collision.CompareTag("Enemy") && !collision.GetComponent<EnemyMovement>().hitted)
        {
            collision.GetComponent<EnemyMovement>().GetDamage(player.executedAttack.GetDamage());
            CombatManager.instance.ComboSistem(true);
        }
    }
}
