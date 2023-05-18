using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UIElements;

public class BossAttack : BossEM
{
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        knockbacked = false;
        alive = true;
        attackCooldown = 3f;
        currentHealth = totalHealth;

        isDash = false;
        isDashing = false;
        isDashInCooldown = false;
        bossHealthBar = GameObject.Find("BossProgressBarHealth");
        bossHealthBar.GetComponent<EntityProgressBar>().maximum = totalHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().previousCurrent = currentHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();
    }
}
