using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public float speed;
    public float minDistance;

    private GameObject player, secondPlayer;
    private float distance;
    private Vector3 playerPos, secondPlayerPos;
    private Vector3 enemyPos;
    // Start is called before the first frame update
    void Start()
    {
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
            Tracking(player);
        }
        else
        {
            distance = Vector3.Distance(enemyPos, secondPlayerPos);
            Tracking(secondPlayer);
        }
    }

    public void Tracking(GameObject trackedPlayer)
    {
        if (distance >= minDistance)
        {
            var targetPos = new Vector3(trackedPlayer.transform.position.x, trackedPlayer.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            transform.position += transform.forward * speed * Time.deltaTime;
            transform.rotation = Quaternion.identity;
        }
    }
}
