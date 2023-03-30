using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetDirection(string direction)
    {
        if(direction == "up")
        {
            anim.SetBool("up", true);
        }
        else if (direction == "left")
        {
            anim.SetBool("left", true);
        }
        else if (direction == "right")
        {
            anim.SetBool("right", true);
        }
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy") 
        {
            float damage = GetComponentInParent<Player>().damage;
            collision.GetComponent<EnemyMovement>().GetDamage(damage);
        }
    }
}
