using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public TextMeshProUGUI damageText;
    
    private Rigidbody2D body;
    private CapsuleCollider2D collider;
    private bool onGround;
    private Direction facingDirection;
    private PlayerStatus status;
    private Vector2 rollingTarget;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int damageCount;

    public bool TakeDamage() {
        if (status == PlayerStatus.Rolling) return false;
        
        animator.SetTrigger("TakeDamage");
        ++damageCount;
        damageText.text = $"Damage: {damageCount}";
        return true;
    }

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        facingDirection = Direction.Right;
        status = PlayerStatus.Idle;
        damageCount = 0;
        damageText.text = "Damage: 0";
    }

    private void FixedUpdate() {
        if (status == PlayerStatus.Rolling) {
            var currentPosition = (Vector2) transform.position;
            var direction = (rollingTarget - currentPosition).normalized;
            var target = currentPosition + direction * (rollingSpeed * Time.fixedDeltaTime);
            if (direction != (rollingTarget - target).normalized || currentPosition == target) {
                body.MovePosition(rollingTarget);
                status = PlayerStatus.Idle;
            } else {
                body.MovePosition(target);
            }
            return;
        }
        
        var xDir = Input.GetAxis("Horizontal") * speed;
        animator.SetBool("Running", !Mathf.Approximately(xDir, 0f));
        if (xDir != 0f) {
            facingDirection = xDir < 0f ? Direction.Left : Direction.Right;
        }

        spriteRenderer.flipX = facingDirection == Direction.Left;
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
            var movingDirection = body.velocity.normalized;
            if (movingDirection != Vector2.zero) {
                status = PlayerStatus.Rolling;
                var rollDistance = GetRollDistance(movingDirection);
                rollingTarget = transform.position + (Vector3) movingDirection * rollDistance;
                animator.SetTrigger("Dodge");
            }
        }

        var attack = Input.GetButtonDown("Fire1");
        if (attack) {
            animator.SetTrigger("Attack");
            var direction = facingDirection == Direction.Left ? Vector2.left : Vector2.right;
            var enemy = GetEnemy(direction);
            print(enemy);
            if(enemy) enemy.GetComponent<Enemy>().TakeDamage();
        }

        // var testDamage = Input.GetKeyDown(KeyCode.F);
        // if (testDamage) {
        //     animator.SetTrigger("TakeDamage");
        // }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Pickup")) {
            print("Item pickup");
            Destroy(other.gameObject);
        } 
        // else if (other.CompareTag("Portal")) {
        //     var portal = other.GetComponent<Portal>();
        //     SceneManager.LoadScene(portal.sceneName);
        // }
    }

    private float GetRollDistance(Vector2 direction) {
        var result = 
            Physics2D.BoxCast(
                (Vector2) transform.position + 0.52f * Vector2.right, 
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
