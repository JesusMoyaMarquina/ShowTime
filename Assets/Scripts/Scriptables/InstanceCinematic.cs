using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceCinematic : MonoBehaviour
{
    // -12.6 1.9
    [SerializeField] private Transform goal, midPoint;
    [SerializeField] private float speed;
    private Animator anim;
    private bool isOnGoal;

    private bool onMidPoint;

    public bool IsOnGoal()
    {
        return isOnGoal;
    }

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
        anim = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChange;
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        if(state == GameState.Combat)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CameraFollowUp();

        if (!onMidPoint)
        {
            transform.position = Vector2.MoveTowards(transform.position, midPoint.position, speed);

            if (transform.position == midPoint.position)
            {
                onMidPoint = true;
            }

            anim.SetFloat("speed", 1f);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, goal.position, speed);

            if(transform.position != goal.position) 
            {
                anim.SetFloat("speed", 1f);
            } else
            {
                isOnGoal = true;
                anim.SetFloat("speed", 0);

            }
        }
    }

    private void CameraFollowUp() => Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);


}
