using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] float moveSpeed = 2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Move();
	}

    private void Move()
    {
        Rigidbody2D enemyBody = GetComponent<Rigidbody2D>();
        if(isFacingRight())
            enemyBody.velocity = new Vector2(moveSpeed, 0f);
        else
            enemyBody.velocity = new Vector2(-moveSpeed, 0f);

    }

    private bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }

    private void FlipFacingDirection()
    {
        Rigidbody2D enemybody = GetComponent<Rigidbody2D>();

        transform.localScale = new Vector2(-Mathf.Sign(enemybody.velocity.x), 1f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        FlipFacingDirection();
    }


}
