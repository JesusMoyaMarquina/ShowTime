using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public float speed;
    public float minDistance;

    private GameObject[] players;
    private GameObject nearPlayer;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private float distance;
    private Vector2 direction;
    private Vector3 enemyPos;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        nearPlayer = FindNearPlayer();

        distance = Vector3.Distance(enemyPos, nearPlayer.transform.position);
        direction = new Vector2(nearPlayer.transform.position.x - enemyPos.x, nearPlayer.transform.position.y - enemyPos.y);
        Tracking();
    }

    public GameObject FindNearPlayer()
    {
        GameObject auxPlayer = players[0];
        enemyPos = transform.position;

        foreach (var player in players)
        {
            if (Vector3.Distance(enemyPos, player.transform.position) < Vector3.Distance(enemyPos, auxPlayer.transform.position))
                auxPlayer = player;
        }
        return auxPlayer;
    }

    public void Tracking()
    {
        if (distance > minDistance)
        {
            var targetPos = new Vector3(nearPlayer.transform.position.x, nearPlayer.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            transform.position += speed * Time.deltaTime * transform.forward;
            transform.rotation = Quaternion.identity;
            SetAnimation(true);
        }
        else
        {
            SetAnimation(false);
        }
    }

    public void SetAnimation(bool isMoving)
    {
        bool isDown = false;
        bool isUp = false;
        bool isSide = false;

        anim.SetBool("isMoving", isMoving);
        if (!isMoving) return;

        if (direction.y >= 0)
        {
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                isSide = true;
            }
            else
            {
                isUp = true;
            }
        }
        else
        {
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                isSide = true;
            }
            else
            {
                isDown = true;
            }
        }

        spriteRenderer.flipX = direction.x > 0;

        anim.SetBool("isUp", isUp);
        anim.SetBool("isSide", isSide);
        anim.SetBool("isDown", isDown);

    }
}
