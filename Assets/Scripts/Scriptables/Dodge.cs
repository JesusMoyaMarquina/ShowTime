using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodge : MonoBehaviour
{
    public float JumpForce;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) DoDodge();
    }

    private void DoDodge()
    {
        Animator.SetBool("Dodge", true);
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }
}
