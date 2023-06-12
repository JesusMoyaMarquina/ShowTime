using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceCinematic : MonoBehaviour
{
    // -12.6 1.9
    [SerializeField] private Transform goal, midPoint;
    [SerializeField] private float speed;
    [SerializeField] private AudioClip stepFX;
    private AudioSource audioSource;
    private Animator anim;
    private bool isOnGoal;
    private bool startMoving, pause;

    private bool onMidPoint;

    public bool IsOnGoal()
    {
        return isOnGoal;
    }

    public void StartMoving()
    {
        startMoving = true;
    }

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        else if (state == GameState.Pause) 
        {
            pause = true;
        } else
        {
            pause = false;
        }
    }

    void Update()
    {
        CameraFollowUp();
        if (startMoving && !pause)
        {
            if(midPoint == null)
            {
                onMidPoint = true;
            }
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

                if (transform.position != goal.position)
                {
                    anim.SetFloat("speed", 1f);
                }
                else
                {
                    isOnGoal = true;
                    anim.SetFloat("speed", 0);

                }
            }
        }
    }

    private void CameraFollowUp() => Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

    public void PlayStepFX()
    {
        audioSource.PlayOneShot(stepFX);
    }


}
