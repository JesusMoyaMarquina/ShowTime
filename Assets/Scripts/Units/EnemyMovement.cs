using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public float speed;
    public float minDistance;

    private GameObject player, secondPlayer;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private float distance;
    private Vector2 direction;
    private Vector3 playerPos, secondPlayerPos;
    private Vector3 enemyPos;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
        secondPlayer = GameObject.Find("SecondPlayer");
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        playerPos = player.transform.position;
        secondPlayerPos = secondPlayer.transform.position;
        enemyPos = transform.position;

        if (Vector3.Distance(enemyPos, playerPos) < Vector3.Distance(enemyPos, secondPlayerPos))
        {
            distance = Vector3.Distance(enemyPos, playerPos);
            direction = new Vector2(playerPos.x - enemyPos.x, playerPos.y - enemyPos.y);
            Tracking(player);
        }
        else
        {
            distance = Vector3.Distance(enemyPos, secondPlayerPos);
            direction = new Vector2(secondPlayerPos.x - enemyPos.x, secondPlayerPos.y - enemyPos.y);
            Tracking(secondPlayer);
        }
    }

    public void Tracking(GameObject trackedPlayer)
    {
        if (distance > minDistance)
        {
            var targetPos = new Vector3(trackedPlayer.transform.position.x, trackedPlayer.transform.position.y, this.transform.position.z);
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
