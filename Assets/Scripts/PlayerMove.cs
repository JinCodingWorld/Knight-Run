using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public VariableJoystick joy;
    //public GameManager gameManger;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    Animator anim;
    AudioSource playerAudio;
    // 창작
    public AudioClip MonsterDie;
    public AudioClip Attacked;
    public AudioClip Coin;
    public AudioClip PlayerDie;
    public AudioClip jumpSound;

    int jumpCnt = 0;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerAudio = GetComponent<AudioSource>();
    }

    //점프 버튼
    public void playerJump()
    {
        if (!anim.GetBool("isJumping") && jumpCnt < 2)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            playerAudio.clip = jumpSound;
            playerAudio.Play(); // 음악
            jumpCnt++;
        }
    }

    void Update()
    {
        Debug.Log("점프 횟수 : " + jumpCnt);

        // Jump 키보드
        // JUMP 횟수 2회로 제한하기!
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping") && jumpCnt < 2)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            Debug.Log("점프 횟수 : " + jumpCnt);
            jumpCnt++;

        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // 이동 한 번 해제해보자
        //Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //창작
        if (rigid.velocity.x < 0f)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isJumping", false);
        }

    }

    void FixedUpdate()
    {
        // 한 번 해제
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");

        //키보드로 교체
        //float h = joy.Horizontal;

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed) //Right Max Speed (속도제한)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        //Landing Platform (어렵당)
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    Debug.Log("디버깅 중");
                    anim.SetBool("isJumping", false);

                }
                // 해결됨? 왜 그럴까? : 발판들이 다 platform이 아니기 때문이다!!
                jumpCnt = 0;
                //Debug.Log("점프 횟수 초기화");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            if(rigid.velocity.y <0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                playerAudio.clip = MonsterDie;
                playerAudio.Play(); // 음악
            }
            else //Damaged
            OnDamaged(collision.transform.position);

        }
    }

    void OnAttack(Transform enemy)
    {
        //point(싱클톤)
        GameManager.instance.stagePoint += 100;

        //Reaction Force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }
    void OnDamaged(Vector2 targetPos)
    {
        playerAudio.clip = Attacked;
        playerAudio.Play(); // 음악

        GameManager.instance.HealthDown();

        gameObject.layer = 11;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            //point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            playerAudio.clip = Coin;
            playerAudio.Play(); // 음악

            if (isBronze)
                GameManager.instance.stagePoint += 50;
            else if(isSilver)
                GameManager.instance.stagePoint += 100;
            else if(isGold)
                GameManager.instance.stagePoint += 300;

            //Deactive Item
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Finish")
        {
            //Next Stage
            GameManager.instance.NextStage();
        }
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    public void OnDie()
    {
        playerAudio.clip = PlayerDie;
        playerAudio.Play(); // 음악

        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
