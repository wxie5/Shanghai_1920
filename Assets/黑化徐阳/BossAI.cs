using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public float movespeed = 3f;
    public float attRange = 0.1f;
    public GameObject Player;

    public float health = 100;
    public float dmgTaken = 5f;

    public float maxjumpCD = 5f;
    private float jumpCD,attCD,rayCD;

    public TextMeshProUGUI hpText;

    bool enabled=false;
    [SerializeField] private LayerMask bossLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    private int facing,yDir;
    private Animator anim;
    private bool isAttacking=false,moveEnable=true;
    private float trackCD = 0.1f;
    private float distanceToPlayer;

    private bool walk,jump,stage2;
    private int stage = 1;

    //死亡
    private bool death;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        facing = 1;
        bossLayerMask = ~bossLayerMask;
        yDir = 1;

        hpText.text = $"HP: {health}";
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            spriteFacingConrtoller();

            trackPlayer();
            if (!isAttacking)
                AI();
            boundaryCheck();
            animatorControl();

            if (health <= 40) {
                stage2 = true;
                // jumpCD = maxjumpCD;
                stage = 2;
            }
                
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
    void attack()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, new Vector2(boxCollider2d.bounds.size.x / 2, boxCollider2d.bounds.size.y / 2), 0f, Vector2.right * facing, attRange / 2, bossLayerMask);
        if(raycastHit2d.collider != null)
        {
            if (raycastHit2d.collider.gameObject==Player &&rayCD==0)
            {
                //这里调用玩家受伤
                raycastHit2d.collider.GetComponent<PlayerController>().TakeDamage();
                rayCD = 0.2f;
            }
        }
    }
    public void damaged()
    {
        health -= dmgTaken;
        hpText.text = $"HP: {health}";
    }
    void animatorControl()
    {
        anim.SetBool("jump", jump);
        anim.SetBool("walk", walk);
        anim.SetBool("stage2", stage2);
    }
    void spriteFacingConrtoller()
    {
        if (facing == 1)
            rigidbody2d.GetComponent<SpriteRenderer>().flipX = false;
        else if (facing == -1)
            rigidbody2d.GetComponent<SpriteRenderer>().flipX = true;
    }
    public void enable()
    {
        enabled = true;
    }
    void CDcontrol()
    {
        if (trackCD > 0)
            trackCD -= Time.deltaTime;
        if (trackCD < 0)
            trackCD = 0;
        if (jumpCD > 0)
            jumpCD -= Time.deltaTime;
        if (jumpCD < 0)
            jumpCD = 0;
        if (attCD > 0)
            attCD -= Time.deltaTime;
        if (attCD < 0)
            attCD = 0;
        if (rayCD > 0)
            rayCD -= Time.deltaTime;
        if (rayCD < 0)
            rayCD = 0;

    }
    void AI()
    {
        if(stage==1)
        {
            if (!isAttacking &&!detectPlayer() && attCD == 0)
            {
                walk = true;
                if (notOnLeftEdge() && facing == -1 || (notOnRightEdge() && facing == 1))
                    transform.Translate(Vector2.right * facing * movespeed * Time.deltaTime, Space.Self);
                if ((notOnLowerEdge() && yDir == -1) || (notOnUpperEdge() && yDir == 1))
                    transform.Translate(new Vector2(0, yDir) * movespeed * Time.deltaTime, Space.Self);
            }
            else if (detectPlayer() && attCD == 0)
            {

                isAttacking = true;
                walk = false;
                anim.SetTrigger("att");
                attCD = 2f;


            }
            else
                walk = true;
        }
        else
        {
            walk = false;
            if (jumpCD==0)
            {
                jump=true;
                if (notOnLeftEdge() && facing == -1 || (notOnRightEdge() && facing == 1))
                    transform.Translate(Vector2.right * facing * movespeed*5f * Time.deltaTime, Space.Self);
                if ((notOnLowerEdge() && yDir == -1) || (notOnUpperEdge() && yDir == 1))
                    transform.Translate(new Vector2(0, yDir) * movespeed*5f * Time.deltaTime, Space.Self);
            }
            if(jump &&detectPlayer())
            {
                jump = false;
                jumpCD = maxjumpCD;
                anim.SetTrigger("att_2");
            }
        }
    }
    void trackPlayer()
    {
        if(!isAttacking &&trackCD==0)
        {
            if (Player.transform.position.x <transform.position.x)
                facing = -1;
            else
                facing = 1;
            if (Player.transform.position.y < transform.position.y- (boxCollider2d.bounds.size.y / 2))
                yDir = -1;
            else if (Player.transform.position.y > transform.position.y+(boxCollider2d.bounds.size.y / 2))
                yDir = 1;
            else
                yDir = 0;
            trackCD = 0.3f;
        }
    }
    bool detectPlayer()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, new Vector2(boxCollider2d.bounds.size.x/2, boxCollider2d.bounds.size.y / 2), 0f, Vector2.right*facing, attRange/2, bossLayerMask);
        if (raycastHit2d.collider == null)
            return false;
        else
            return raycastHit2d.collider.gameObject == Player;
    }
    void boundaryCheck()
    {
        if (!notOnLeftEdge() && facing == -1)
            moveEnable = false;
        else if (!notOnRightEdge() && facing == 1)
            moveEnable = false;
        else if (!notOnLowerEdge() && yDir == -1)
            moveEnable = false;
        else if (!notOnUpperEdge() && yDir == 1)
            moveEnable = false;
        else
            moveEnable = true;
    }
    private bool notOnLeftEdge()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.left, 0.1f, groundLayerMask);
        return raycastHit2d.collider == null;
    }
    private bool notOnRightEdge()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.right, 0.1f, groundLayerMask);
        return raycastHit2d.collider == null;
    }
    private bool notOnLowerEdge()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 0.1f, groundLayerMask);
        return raycastHit2d.collider == null;
    }
    private bool notOnUpperEdge()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.up, 0.1f, groundLayerMask);
        return raycastHit2d.collider == null;
    }
    void enteridle()
    {
        isAttacking = false;

    }
}
