using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Screw : MonoBehaviour
{
    public float speed;
    protected Vector2 direction;

    public float distanceToPlayer;

    protected Rigidbody2D rb;
    protected Animator anim;

    public float minDistance;
    protected bool inMovementRange;

    protected GameObject[] players;
    protected GameObject nearPlayer;
    protected float distance;
    protected Vector3 pos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        Movement();
    }

    public virtual void Movement()
    {
        if (!GameManager.Instance.isInCombat) return;

        nearPlayer = FindNearPlayer();

        distance = Vector3.Distance(new Vector2(pos.x, pos.y), new Vector2(nearPlayer.transform.position.x,
                                                                                nearPlayer.transform.position.y + nearPlayer.GetComponent<BoxCollider2D>().size.y * nearPlayer.transform.localScale.y));
        direction = new Vector2(nearPlayer.transform.position.x - pos.x,
                                nearPlayer.transform.position.y + nearPlayer.GetComponent<BoxCollider2D>().size.y * nearPlayer.transform.localScale.y - pos.y);

        Tracking();
    }
    public void Tracking()
    {
        inMovementRange = distance > minDistance;

        Translation();
    }
    public virtual void Translation()
    {
        if (inMovementRange)
        {
            Vector2 direction = new Vector2(nearPlayer.transform.position.x - transform.position.x, 
                                            nearPlayer.transform.position.y - transform.position.y).normalized;
            var targetPos = new Vector3(nearPlayer.transform.position.x, nearPlayer.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            rb.velocity = direction * speed;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public GameObject FindNearPlayer()
    {
        GameObject auxPlayer = players[0];
        pos = transform.position;

        foreach (var player in players)
        {
            if (player == null)
                continue;

            if(auxPlayer == null)
            {
                auxPlayer = player;
            }

            if (Vector3.Distance(pos, player.transform.position) < Vector3.Distance(pos, auxPlayer.transform.position))
                auxPlayer = player;
        }

        distanceToPlayer = Vector3.Distance(pos, auxPlayer.transform.position);

        return auxPlayer;
    }

    public void ActivateHeal()
    {
        anim.SetTrigger("heal");
    }
}
