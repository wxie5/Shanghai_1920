using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;
    public Transform bottom;
    public LayerMask groundLayer;
    
    private Rigidbody2D body;
    private bool onGround;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        // rigidbody.MovePosition(new Vector2(6.9f, 1.2f));
    }

    private void FixedUpdate() {
        var xDir = Input.GetAxis("Horizontal") * speed;
        var dir = new Vector2(xDir, body.velocity.y);
        body.velocity = dir;
        
        var jump = Input.GetButtonDown("Jump");
        if (jump) {
            var hit = Physics2D.Raycast(bottom.position, Vector2.down, 0.1f, groundLayer);
            if (hit) {
                body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Pickup")) {
            print("Item pickup");
            Destroy(other.gameObject);
        }
    }
}
