using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float distance;

    private float playerX, playerY;
    private float enemyX, enemyY;
    // Start is called before the first frame update
    void Start()
    {
        //Movement();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        playerX = player.transform.position.x;
        playerY = player.transform.position.y;

        if (Mathf.Abs(playerX - this.transform.position.x) > distance)
        {
            enemyX = playerX - speed;
        }

    }
}
