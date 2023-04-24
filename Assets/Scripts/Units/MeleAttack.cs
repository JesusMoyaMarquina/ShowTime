using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleAttack : MonoBehaviour
{
    public GameObject player;
    private Vector3 playerPosition;
    private float launchSpeed;
    private float startTime;
    private float time;

    void Start()
    {
        launchSpeed = 10f;
        startTime = Time.time;
        time = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch(GameObject nearPlayer)
    {
        player = nearPlayer;
        playerPosition = player.transform.position;
        //while (Time.time > startTime + time)
        //{
        //    Vector3 direction = (playerPosition - transform.position).normalized;

        //    Vector3 newPosition = transform.position + launchSpeed * Time.deltaTime * direction;

        //    transform.position = newPosition;
        //}
    }
}
