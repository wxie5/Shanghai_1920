using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    public float movespeed = 3f;
    public float attRange = 0.1f;
    public GameObject Player;

    public float health = 100;
    public float dmgTaken = 5f;

    public float maxdashCD = 5f;
    private float dashCD, rayCD;

    public TextMeshProUGUI hpText;

    bool enabled = false;
    [SerializeField] private LayerMask bossLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    private int facing, yDir;
    private Animator anim;
    private bool isAttacking,startDash;
    private float trackCD = 0.1f;
    private bool death;
    private float x, y,moveEnable;
    void Start()
    {
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        facing = 1;
        bossLayerMask = ~bossLayerMask;
        yDir = 1;
        
        hpText.text = $"HP: {health}";
        print($"start");
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            print("update");
            spriteFacingConrtoller();

            trackPlayer();
            if (!isAttacking)
                AI();
            boundaryCheck();
            if (health <= 0)
            {
                death = true;
                enabled = false;
                //这里进行死亡后的流程
                BattleLevelController.instance.OnWin();
                Destroy(gameObject);
            }
            CDcontrol();
        }
    }
    void LateUpdate()
    {
        dashing();
    }
    void spriteFacingConrtoller()
    {
        if (facing == 1)
            rigidbody2d.GetComponent<SpriteRenderer>().flipX = false;
        else if (facing == -1)
            rigidbody2d.GetComponent<SpriteRenderer>().flipX = true;
    }

    public void damaged()
    {
        health -= dmgTaken;
        hpText.text = $"HP: {health}";
    }
    void attack()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.right * facing, attRange / 2, bossLayerMask);
        if (raycastHit2d.collider != null)
        {
            if (raycastHit2d.collider.gameObject == Player && rayCD == 0)
            {
                //这里调用玩家受伤
                raycastHit2d.collider.GetComponent<PlayerController>().TakeDamage();
                rayCD = 0.5f;
            }
        }
    }
    void AI()
    {

        if(dashCD==0)
        {
            anim.SetTrigger("dash");
            isAttacking = true;
            dashCD = maxdashCD;
        }
        
    }
    void dashing()
    {
        if (startDash)
        {
            transform.Translate(new Vector2(x, y) * movespeed * Time.deltaTime*moveEnable, Space.Self);
            attack();
        }
    }
    void dash()
    {
        startDash = true;
    }
    void exist()
    {
        startDash = false;
    }
    bool detectPlayer()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, new Vector2(boxCollider2d.bounds.size.x / 2, boxCollider2d.bounds.size.y / 2), 0f, Vector2.right * facing, attRange / 2, bossLayerMask);
        if (raycastHit2d.collider == null)
            return false;
        else
            return raycastHit2d.collider.gameObject == Player;
    }
    public void enable()
    {
        enabled = true;
        print($"enable");
    }
    void CDcontrol()
    {
        if (trackCD > 0)
            trackCD -= Time.deltaTime;
        if (trackCD < 0)
            trackCD = 0;
        if (dashCD > 0)
            dashCD -= Time.deltaTime;
        if (dashCD < 0)
            dashCD = 0;
        if (rayCD > 0)
            rayCD -= Time.deltaTime;
        if (rayCD < 0)
            rayCD = 0;

    }
    void trackPlayer()
    {
        if(!startDash)
        {
            if (Player.transform.position.y > boxCollider2d.bounds.center.y - boxCollider2d.bounds.size.y / 2 && Player.transform.position.y < boxCollider2d.bounds.center.y+ boxCollider2d.bounds.size.y / 2)
                y = 0;
            else 
                y = Player.transform.position.y - transform.position.y;

            x = Player.transform.position.x - transform.position.x;
        }
        if (!isAttacking && trackCD == 0)
        {
            if (Player.transform.position.x < transform.position.x)
                facing = -1;
            else
                facing = 1;
            if (Player.transform.position.y < transform.position.y - boxCollider2d.bounds.size.y/2)
                yDir = -1;
            else if (Player.transform.position.y > transform.position.y + boxCollider2d.bounds.size.y/2)
                yDir = 1;
            else
                yDir = 0;
            trackCD = 0.3f;
        }
    }
    void boundaryCheck()
    {
        if (!notOnLeftEdge() && facing == -1)
            moveEnable = 0;
        else if (!notOnRightEdge() && facing == 1)
            moveEnable = 0;
        else if (!notOnLowerEdge() && yDir == -1)
            moveEnable = 0;
        else if (!notOnUpperEdge() && yDir == 1)
            moveEnable = 0;
        else
            moveEnable = 1;
    }
    private bool notOnLeftEdge()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(new Vector2(boxCollider2d.bounds.center.x-0.4f, boxCollider2d.bounds.center.y), boxCollider2d.bounds.size/2, 0f, Vector2.left, 0.3f, groundLayerMask);
        return raycastHit2d.collider == null;
    }
    private bool notOnRightEdge()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(new Vector2(boxCollider2d.bounds.center.x + 0.4f, boxCollider2d.bounds.center.y), boxCollider2d.bounds.size/2, 0f, Vector2.right, 0.3f, groundLayerMask);
        return raycastHit2d.collider == null;
    }
    private bool notOnLowerEdge()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 0.3f, groundLayerMask);
        return raycastHit2d.collider == null;
    }
    private bool notOnUpperEdge()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.up, 0.3f, groundLayerMask);
        return raycastHit2d.collider == null;
    }
    void enteridle()
    {
        isAttacking = false;

    }
}
