using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    Left, Right
}

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;
    public Transform bottom;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public float maxRollDistance;
    public float attackRange;
    
    private Rigidbody2D body;
    private CapsuleCollider2D collider;
    private bool onGround;
    private Direction facingDirection;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        facingDirection = Direction.Right;
    }

    private void FixedUpdate() {
        var xDir = Input.GetAxis("Horizontal") * speed;
        if (xDir != 0) {
            facingDirection = xDir < 0f ? Direction.Left : Direction.Right;
        }
        var dir = new Vector2(xDir, body.velocity.y);
        body.velocity = dir;
        
        var jump = Input.GetButtonDown("Jump");
        if (jump) {
            var hit = Physics2D.Raycast(bottom.position, Vector2.down, 0.1f, groundLayer);
            if (hit) {
                body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        var roll = Input.GetButtonDown("Roll");
        if (roll) {
            var movingDirection = body.velocity.normalized;
            var rollDistance = GetRollDistance(movingDirection);
            body.MovePosition(transform.position + (Vector3) movingDirection * rollDistance);
            // transform.position += (Vector3) movingDirection * rollDistance;
        }

        var attack = Input.GetButtonDown("Fire1");
        if (attack) {
            var direction = facingDirection == Direction.Left ? Vector2.left : Vector2.right;
            var enemy = GetEnemy(direction);
            print(enemy);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Pickup")) {
            print("Item pickup");
            Destroy(other.gameObject);
        }
    }

    private float GetRollDistance(Vector2 direction) {
        var result = 
            Physics2D.BoxCast(
                transform.position, 
                collider.size - 0.1f * Vector2.one, 
                0f,
                direction,
                maxRollDistance, 
                groundLayer);
        return result.collider ? result.distance : maxRollDistance;
    }

    private GameObject GetEnemy(Vector2 direction) {
        var result = 
            Physics2D.BoxCast(
                transform.position, 
                collider.size - 0.1f * Vector2.one, 
                0f,
                direction,
                attackRange, 
                enemyLayer);
        return result.collider ? result.collider.gameObject : null;
    }
}
