using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    Left, Right
}

public enum PlayerStatus {
    Idle, Moving, Rolling
}

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;
    public Transform bottom;
    public LayerMask blockedLayer;
    public LayerMask enemyLayer;
    public float maxRollDistance;
    public float attackRange;
    public float rollingSpeed;
    
    private Rigidbody2D body;
    private CapsuleCollider2D collider;
    private bool onGround;
    private Direction facingDirection;
    private PlayerStatus status;
    private Vector2 rollingTarget;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        facingDirection = Direction.Right;
        status = PlayerStatus.Idle;
    }

    private void FixedUpdate() {
        if (status == PlayerStatus.Rolling) {
            var currentPosition = (Vector2) transform.position;
            var direction = (rollingTarget - currentPosition).normalized;
            var target = currentPosition + direction * (rollingSpeed * Time.fixedDeltaTime);
            if (direction != (rollingTarget - target).normalized) {
                body.MovePosition(rollingTarget);
                status = PlayerStatus.Idle;
            } else {
                body.MovePosition(target);
            }
            return;
        }
        
        var xDir = Input.GetAxis("Horizontal") * speed;
        if (xDir != 0) {
            facingDirection = xDir < 0f ? Direction.Left : Direction.Right;
        }
        var dir = new Vector2(xDir, body.velocity.y);
        body.velocity = dir;
        
        var jump = Input.GetButtonDown("Jump");
        if (jump) {
            var hit = Physics2D.Raycast(bottom.position, Vector2.down, 0.1f, blockedLayer);
            if (hit) {
                body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        var roll = Input.GetButtonDown("Roll");
        if (roll && status != PlayerStatus.Rolling) {
            status = PlayerStatus.Rolling;
            var movingDirection = body.velocity.normalized;
            var rollDistance = GetRollDistance(movingDirection);
            rollingTarget = transform.position + (Vector3) movingDirection * rollDistance;
            // body.MovePosition(transform.position + (Vector3) movingDirection * rollDistance);
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
                collider.size - 0.2f * Vector2.one, 
                0f,
                direction,
                maxRollDistance, 
                blockedLayer);
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
