using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float minDistance;

    private float distance;
    private Vector3 playerPos;
    private Vector3 enemyPos;
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
        playerPos = player.transform.position;
        enemyPos = transform.position;
        distance = Vector3.Distance(enemyPos, playerPos);

        if (distance >= minDistance)
        {
            var targetPos = new Vector3(player.transform.position.x, player.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            transform.position += transform.forward * speed * Time.deltaTime;
            transform.rotation = Quaternion.identity;
        }
    }
}
