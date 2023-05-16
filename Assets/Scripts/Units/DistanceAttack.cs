using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DistanceAttack : MonoBehaviour
{
    public float launchSpeed;
    public float distance;
    public float damage;
    
    private GameObject player;
    private Rigidbody2D rb;
    private Vector3 enemyPosition;
    private Vector2 direction;
    private bool startAttack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!startAttack) return;

        Translation();

        if (Vector3.Distance(transform.position, enemyPosition) >= distance) DestroyObject();
    }

    public void Launch(GameObject nearPlayer)
    {
        player = nearPlayer;
        enemyPosition = transform.position;
        direction = (player.transform.position - transform.position).normalized;
        startAttack = true;
    }

    public void Translation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        float x = transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad);
        var targetPos = new Vector3(x, y, this.transform.position.z);
        transform.LookAt(targetPos);
        rb.velocity = direction * launchSpeed;
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
            collision.GetComponent<Player>().GetDamage(damage, direction);
            DestroyObject();
        }
    }
}
