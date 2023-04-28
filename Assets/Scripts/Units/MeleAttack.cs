using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class MeleAttack : MonoBehaviour
{
    public float damage;

    private GameObject player;
    private Rigidbody2D rb;
    private Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(GameObject nearPlayer, char look)
    {
        player = nearPlayer;
        direction = (player.transform.position - transform.position).normalized;
        Rotation(look);
    }

    public void Rotation(char look)
    {
        float angle = 0f;
        switch (look)
        {
            case 'L':
                angle = -90f;
                break;
            case 'R':
                angle = 90f;
                break;
            case 'U':
                angle = 0f;
                break;
            case 'D':
                angle = 180f;
                break;
        }

        var targetPos = new Vector3(direction.x, direction.y, this.transform.position.z);
        transform.LookAt(targetPos);
        Debug.Log(angle);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void DestroyObject()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().GetDamage(damage);
            DestroyObject();
        }
    }
}
