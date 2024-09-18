using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic; // HashSet<T> を使用するために必要
using UnityEngine.Video;
using Unity.VisualScripting.Antlr3.Runtime;
using DG.Tweening; // DOTweenの名前空間
using System.Threading.Tasks; // Task を使用するために必要



public class PlayyerMove : MonoBehaviour
{
    [SerializeField] GameObject jumpEff;
    private SpriteRenderer spriteRenderer;
    public float jumpPower = 10.0f;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float stepdis = 1.0f;
    [SerializeField] float stepmove = 2f;//ステップ距離
    [SerializeField] float backForce = 2.0f;
    //酸素関連
    [SerializeField] float oxyspeed = 0.000001f;//酸素消費量？
    [SerializeField] public static float oxyval = 1.0f;//酸素量？
    [SerializeField] float startoxy = 1.0f;//酸素リセット用の奴上のと同じ数値に
    [SerializeField] float oxyheal = 0.5f;//酸素の回復量（1.0を超えた分は無くなるものとする）
    [SerializeField] float oxyshot = 0.05f;//射撃で減る酸素量
    [SerializeField] float oxybeam = 0.1f;//ビームで減る量
    [SerializeField] float oxystep = 0.02f;//ステップで消費する酸素
    [SerializeField] float damage = 0.1f, thunder_damage = 0.1f, sickle_damage = 0.1f;//ダメージ受けて減る酸素量

    [SerializeField] float attackWindow = 0.3f; // 攻撃入力受付時間（秒）
    [SerializeField] float attackCooldown = 0.5f;//近接のクールタイム
    [SerializeField] float attackCooldownAfterCombo = 1.0f; // 三段目の後のクールタイム
    private int attackCount = 0;//近接の回数（最大３）
    private bool isInAttackCooldown = false; // クールタイム中かどうかを示すフラグ
    private float lastAttackTime;
    private float lastAttackInputTime;
    public static bool muki = true; // trueで右向き
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 force;
    float shotwait = 0;
    float beamwait = 0;
    [SerializeField] private float invincibleTime = 2.0f; // 無敵時間
    [SerializeField] private float blinkDuration = 0.2f;  // 点滅の間隔
    private bool isInvincible = false;
    [SerializeField] TextMeshProUGUI scoretext, cleartext, Gotext;
    [SerializeField] GameObject getkeyflag;
    [SerializeField] Slider oxybar;
    [SerializeField] Image oxycolor;
    [SerializeField] GameObject clearVideoObj;//クリア時のビデオ格納するやつ
    private MeshRenderer clearVideoRen;//透明度で操作
    private VideoPlayer clearVideo;//クリア時のビデオ
    [SerializeField] GameObject shotobj;
    [SerializeField] GameObject slobj, lasslobj;//近接の判定
    [SerializeField] GameObject slefc;//近接のエフェクト
    private GameObject atkobj;//近接のエフェクト入れるやつ
    private GameObject atkcol;//近接判定入れるやつ
    [SerializeField] GameObject Shotefect; // 弾を出すときのエフェクト
    [SerializeField] GameObject bemefe;
    [SerializeField] GameObject beamobj;
    [SerializeField]
    AudioClip SE_jump, SE_shot, SE_damage, SE_dead, SE_getItem, SE_Clear, SE_slash, SE_open,
     SE_step, SE_goal, se_keyitem;//se
    AudioSource snd;//音出すやつ
    bool isdead;//死亡判定
    bool GetKey;
    bool Getkagi;
    bool isclear;//クリア判定
    private Vector3 targetPosition;
    private bool isStep = false;
    private bool canstep = true;
    //bool canmove;
    private bool canMoveLeft = true;//左右の壁に触れてる時の処理
    private bool canMoveRight = true;//
    private bool InsafeZone;//セーフゾーンの中にいるとき
    private bool IsAtk = false;
    public bool clstFlag;
    private HashSet<Collider2D> leftColliders = new HashSet<Collider2D>();
    private HashSet<Collider2D> rightColliders = new HashSet<Collider2D>();
    [SerializeField] GameObject arrowobj, itemarrow;
    [SerializeField] private BOSSmove boss;
    private GoalManager goalManager;//goalmanagerのプログラム
    private Fademane fademane;
    [SerializeField] GameObject BOSS;
    bool InBossZone;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        snd = gameObject.AddComponent<AudioSource>();
        oxyval = startoxy;
        if (itemarrow != null)
        {
            itemarrow.gameObject.SetActive(true);
        }
        isInvincible = false;
        force = new Vector2(0.0f, jumpPower);
        rb = GetComponent<Rigidbody2D>();
        isdead = false;
        GetKey = false;
        Getkagi = false;
        isclear = false;
        isStep = false;
        muki = true;
        IsAtk = false;
        InsafeZone = false;
        getkeyflag.gameObject.SetActive(false);
        clearVideo = clearVideoObj.GetComponent<VideoPlayer>();
        clearVideoRen = clearVideoObj.GetComponent<MeshRenderer>();
        clearVideoRen.enabled = false;
        clstFlag = GameManager.Instance.clst;
        string currentScene = SceneManager.GetActiveScene().name;
        GameManager.Instance.SavePreviousScene(currentScene); // 現在のシーンを保存
        goalManager = FindObjectOfType<GoalManager>(); // GoalManagerを持つゲームオブジェクトを検索
        StartCoroutine(VideoSet());
        fademane = FindObjectOfType<Fademane>();//fademaneの取得
        if (fademane == null)
        {
            Debug.LogError("Fademane instance not found in the scene.");
        }
    }

    void Update()
    {
        Move();
        Step();
        Down();
        Jump();
        if (Shot()) { return; }
        if (beam()) { return; }
        HandleAttackAsync();
        if (Time.time > lastAttackTime + attackCooldown)
        {
            attackCount = 0;
        }//一定時間経過で攻撃リセット
        scoretext.text = "稼いだお金: " + newsystem.score;
        if (!isclear)
        {
            if (!InsafeZone)
            {
                oxyval -= oxyspeed * Time.deltaTime;
            }
        }//酸素消費
        oxybar.value = oxyval;
        if (oxybar.value > 0.25f) { oxycolor.color = new Color32(255, 209, 59, 255); } else { oxycolor.color = new Color32(255, 59, 61, 255); }
        getkeyflag.gameObject.SetActive(Getkagi);
        if (GetKey)
        {
            arrowobj.SetActive(true);
            if (isclear)
            {
                arrowobj.SetActive(false);
            }
            if (itemarrow != null)
            {
                itemarrow.gameObject.SetActive(false);
            }
        }//arrowの処理
        if (oxyval <= 0.0f)
        {
            if (isdead == false)
            {
                anim.SetTrigger("dead");
                StartCoroutine(triggerDead());
                isdead = true;
                if (BOSS != null)
                {
                    boss.HideRedLights();
                }
            }
        }//死亡判定

    }

    private async void HandleAttackAsync()
    {
        // 非同期処理で攻撃処理を呼び出す
        await Attack();
    }

    void Move()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 && !isdead && !isclear && !isStep)
        {
            float x = Input.GetAxisRaw("Horizontal");
            if (x >= 0 && canMoveRight)
            {
                muki = true;
                transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                anim.SetBool("walk", true);
            }
            else if (x < 0 && canMoveLeft)
            {
                muki = false;
                transform.localScale = new Vector3(-0.15f, 0.15f, 0.15f);
                anim.SetBool("walk", true);
            }
            else if (!canMoveLeft || !canMoveRight)
            {
                x = 0;
                anim.SetBool("walk", false);
                //   Debug.Log("kabe");
            }
            Vector2 velocity = rb.velocity;
            velocity.x = x * moveSpeed;
            rb.velocity = velocity;
        }
        else // 入力がないとき
        {
            anim.SetBool("walk", false); 
            Vector2 velocity = rb.velocity;
        }
    }// 移動

    void Down()
    {
        if (Input.GetAxisRaw("Vertical") != 0 && !isdead && !isclear && !isStep)
        {
            float y = Input.GetAxisRaw("Vertical");
            if (Input.GetKeyDown(KeyCode.S))
            {
                y = 0;
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * y);
            }
            if (y < -0.3)
            {
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * y);
            }
        }
    }

    void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Z)) && !isdead && !isclear && !isStep)
        {
            anim.SetTrigger("jump");
            Instantiate(jumpEff, transform.position, Quaternion.identity);
            rb.AddForce(force, ForceMode2D.Impulse);
            snd.PlayOneShot(SE_jump);
        }
    }//ジャンプ

    void Step()
    {
        if ((Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.S)) && !isdead && !isclear && !isStep && canstep)
        {
            isStep = true;
            canstep = false;
            float y = Input.GetAxisRaw("Vertical");
            y = 0;
            anim.SetTrigger("step");

        }
    }

    void Instep()
    {
        snd.PlayOneShot(SE_step);
        if (!InsafeZone)//ゲージ消費
        {
            oxyval -= oxystep;
        }
        float dir = muki ? 1 : -1;
        targetPosition = transform.position + new Vector3(stepmove * dir, 0, 0);

        // ステップ開始
        isStep = true;
        canstep = false;
        rb.AddForce(new Vector2(stepdis * dir, 0), ForceMode2D.Impulse);
    }

    public void StepEnd()
    {
        rb.velocity = Vector2.zero;
        isStep = false;
        canstep = true;
    }

    bool Shot()
    {
        if (shotwait > 0)
        {
            shotwait -= Time.deltaTime;
            return true;
        }
        if ((Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.X)) && !isdead && !isclear && !IsAtk)
        {
            IsAtk = true;
            anim.SetTrigger("shot");
            if (!InsafeZone)
            {
                oxyval -= oxyshot;
            }
            float direction = muki ? 1 : -1;
            Vector3 position = transform.position;
            position.x += 1f * direction;
            position.y += 0.2f;
            atkobj= Instantiate(Shotefect, position, Quaternion.identity);
            // AttackFollow スクリプトを追加し、プレイヤーの Transform とオフセットを設定
            AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
            followScript.playerTransform = transform;
            followScript.offset = new Vector2(1f * direction, 0.2f);
            snd.PlayOneShot(SE_shot);
        }
        return false;
    }//射撃用意

    void plshot()
    {
        float direction = muki ? 1 : -1;
        Vector3 position = transform.position;
        position.x += 1f * direction;
        position.y += 0.2f;
        GameObject a = Instantiate(shotobj, position, Quaternion.identity);
        Shot shotScript = a.GetComponent<Shot>();
        if (shotScript != null)
        {
            shotScript.muki = muki;
        }
        Rigidbody2D b = a.GetComponent<Rigidbody2D>();
        b.AddForce(new Vector3(1000 * direction, 0, 0));
        shotwait = 0.5f;

    }//弾の呼び出しとか

    bool beam()
    {
        if (beamwait > 0)
        {
            beamwait -= Time.deltaTime;
            return true;
        }
        if ((Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.V)) && !isdead && !isclear && !IsAtk&&!isStep)
        {

            Debug.Log("beam");
            IsAtk = true;
            isStep = true;
            anim.SetTrigger("beam");
            if (!InsafeZone)
            {
                oxyval -= oxybeam;
            }
            float direction = muki ? 1 : -1;
            Vector3 position = transform.position;
            position.x += 1f * direction;
            position.y += 0.2f;
           atkobj= Instantiate (Shotefect, position, Quaternion.identity);
            // AttackFollow スクリプトを追加し、プレイヤーの Transform とオフセットを設定
            AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
            followScript.playerTransform = transform;
            followScript.offset = new Vector2(1f * direction, 0.2f);
            snd.PlayOneShot(SE_shot);
        }
        return false;
    }//ビームの用意

    void plbeam()
    {

        float direction = muki ? 1 : -1;
        float rot = muki ? 0 : 180;
        Vector2 position = new Vector2(transform.position.x + 12f * direction, transform.position.y + 0.2f);
        Quaternion rotation = Quaternion.Euler(-90f, 0, 0 + rot);
        atkcol = Instantiate(beamobj, position, Quaternion.identity);
        atkobj = Instantiate(bemefe, position, rotation);
        Vector3 newScale = new Vector3(direction, 1f, 1f); // X方向にスケールを変更
        atkobj.transform.localScale = newScale;
        // AttackFollow スクリプトを追加し、プレイヤーの Transform とオフセットを設定
        AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
        followScript.playerTransform = transform;
        followScript.offset = new Vector2(1f * direction, 0.2f);
        // AttackFollow 
        AttackFollow followScriptAtkCol = atkcol.AddComponent<AttackFollow>();
        followScriptAtkCol.playerTransform = transform;
        followScriptAtkCol.offset = new Vector2(14f * direction, 0.2f);

        // パーティクルシステムの終了を監視
        StartCoroutine(DestroyOnParticleEnd(atkobj, atkcol));

        beamwait = 1f;
    }
    private IEnumerator DestroyOnParticleEnd(GameObject particleObject1, GameObject particleObject2)
    {
        // ParticleSystem コンポーネントを取得
        ParticleSystem ps = particleObject1.GetComponent<ParticleSystem>();

        // ParticleSystem が存在する場合
        if (ps != null)
        {
            // パーティクルが終了するまで待機
            while (ps.isPlaying)
            {
                yield return null;
            }
        }

        // パーティクルシステムが終了したらオブジェクトを破壊
        Destroy(particleObject1);
        Destroy(particleObject2);
        IsAtk = false;
        isStep = false;

    }
    void shotend()
    {
        IsAtk = false;
        canstep = true;
    }

    private async Task Attack()
    {
        if ((Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.C)) && !isdead && !isclear && !isStep)
        {
            // クールタイム中かどうかをチェック
            if (isInAttackCooldown)
            {
                return;
            }

            canstep = false;
            float currentTime = Time.time;

            // クールタイムが経過していれば攻撃カウントをリセット
            if (currentTime - lastAttackTime > attackCooldown)
            {
                attackCount = 0;
            }

            // 攻撃カウントと攻撃ウィンドウのチェック
            if (attackCount == 0 || currentTime - lastAttackInputTime < attackWindow)
            {
                attackCount++;
                lastAttackTime = currentTime;
                lastAttackInputTime = currentTime;

                // 攻撃アニメーションのトリガー
                switch (attackCount)
                {
                    case 1:
                        anim.SetTrigger("Attack1");
                        canstep = false;
                        break;
                    case 2:
                        anim.SetTrigger("Attack2");
                        canstep = false;
                        break;
                    case 3:
                        anim.SetTrigger("Attack3");
                        canstep = false;
                        attackCount = 0; // 3回目の攻撃後はリセット

                        // 3発目の攻撃後にクールタイムを設定
                        isInAttackCooldown = true;
                        await Task.Delay((int)(attackCooldownAfterCombo * 1000)); // クールタイム後にフラグをリセット
                        isInAttackCooldown = false;
                        break;
                }
            }
            else
            {
                // 攻撃カウントが3未満であれば通常のクールタイム
                await Task.Delay((int)(attackCooldown * 1000));
            }
        }
    }

    // 近接の入力、アニメーションの呼び出し


    void Slash()
    {
        IsAtk = true;
        canstep = false;
        snd.PlayOneShot(SE_slash);
        float direction = muki ? 1 : -1;
        float rot = muki ? 0 : 180;
        Vector2 position = new Vector2(transform.position.x + 1f * direction, transform.position.y + 0.2f);
        Quaternion rotation = Quaternion.Euler(-238.51f, 0, 0 + rot);
        atkcol = Instantiate(slobj, position, Quaternion.identity);
        atkobj = Instantiate(slefc, position, rotation);
        // AttackFollow スクリプトを追加し、プレイヤーの Transform とオフセットを設定
        AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
        followScript.playerTransform = transform;
        followScript.offset = new Vector2(1f * direction, 0.2f);
        // AttackFollow 
        AttackFollow followScriptAtkCol = atkcol.AddComponent<AttackFollow>();
        followScriptAtkCol.playerTransform = transform;
        followScriptAtkCol.offset = new Vector2(1f * direction, 0.2f);
    }//1,2段目の判定呼び出し

    void LasSlash()
    {
        IsAtk = true;
        canstep = false;
        snd.PlayOneShot(SE_slash);
        float direction = muki ? 1 : -1;
        float rot = muki ? 0 : 180;
        Vector2 position = new Vector2(transform.position.x + 1f * direction, transform.position.y + 0.2f);
        Quaternion rotation = Quaternion.Euler(-90f, 0, 0 + rot);
        atkcol = Instantiate(lasslobj, position, Quaternion.identity);
        atkobj = Instantiate(slefc, position, rotation);
        // AttackFollow スクリプトを追加し、プレイヤーの Transform とオフセットを設定
        AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
        followScript.playerTransform = transform;
        followScript.offset = new Vector2(1f * direction, 0.2f);
        // AttackFollow 
        AttackFollow followScriptAtkCol = atkcol.AddComponent<AttackFollow>();
        followScriptAtkCol.playerTransform = transform;
        followScriptAtkCol.offset = new Vector2(1f * direction, 0.2f);
        // Debug.Log("くも");
    }//3段目の判定呼び出し

    void Destroyobj()
    {
        IsAtk = false;
        if (atkobj != null)
        {
            Destroy(atkobj);
            Destroy(atkcol);
        }
        canstep = true;
    }

    public void TriggerKnockback()
    {
        plDamage(transform.position);
    }//アニメーションからノックバックを呼び出す



    IEnumerator triggerDead()
    {
        snd.PlayOneShot(SE_dead);
        yield return new WaitWhile(() => snd.isPlaying);
        if (fademane != null)
        {
            fademane.ChangeSceneWithFade(1f, 0.5f, "Gameover"); // フェード時間は適宜調整
        }
        else
        {
            SceneManager.LoadScene("Gameover");
        }
    }//死亡処理

    public void plDamage(Vector2 collisionPoint)
    {
        if (!isInvincible || !isdead)
        {
            // ノックバック方向を計算（衝突点からこのオブジェクトの中心へのベクトル）
            Vector2 knockbackDirection = (rb.position - collisionPoint).normalized;

            // ノックバックの力を適用
            rb.AddForce(knockbackDirection * backForce, ForceMode2D.Impulse);

        }
    }//ノックバック処理

    public void StopKnockback()
    {
        rb.velocity = Vector2.zero; // ノックバック中に速度が残っていたらリセット
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && !isInvincible && !isdead)
        {
            oxyval -= damage;
            snd.PlayOneShot(SE_damage);
            StartCoroutine(StartInvincibility());
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "doar" && !isdead && Getkagi)
        {
            Getkagi = false;
            snd.PlayOneShot(SE_open);
            Destroy(col.gameObject);
        }

        if (col.collider.CompareTag("Ground"))
        {


            foreach (ContactPoint2D contact in col.contacts)
            {
                float angle = Vector2.Angle(contact.normal, Vector2.up);

                if (angle >= 45f)
                {

                    if (contact.normal.x > 0)
                    {
                        leftColliders.Add(col.collider);
                        canMoveLeft = false;
                    }
                    else if (contact.normal.x < 0)
                    {
                        rightColliders.Add(col.collider);
                        canMoveRight = false;
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            if (leftColliders.Contains(collision.collider))
            {
                leftColliders.Remove(collision.collider);
                if (leftColliders.Count == 0)
                {
                    canMoveLeft = true;
                }
            }

            if (rightColliders.Contains(collision.collider))
            {
                rightColliders.Remove(collision.collider);
                if (rightColliders.Count == 0)
                {
                    canMoveRight = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)//アイテムとかに触れた時の処理
    {
        if (collision.gameObject.tag == "Goal" && !isdead && GetKey)
        {
            if (!isclear)
            {
                snd.PlayOneShot(SE_goal);
                isclear = true;
            }
            moveSpeed = 0;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            Gotext.text = " ";
            cleartext.text = "CLEAR!!";
            StartCoroutine(ClearVideoPlay());//映像呼び出し
            Invoke("Goal", 3.0f);
        }

        if (collision.gameObject.tag == "safe")
        {
            InsafeZone = true;
        }
        if (collision.gameObject.tag == "BossZone")
        {
            InsafeZone = clstFlag;
            Debug.Log("in");
        }
        if (collision.gameObject.tag == "Bubble" && !isdead)//酸素回復
        {
            if (oxyval + oxyheal >= 1.0)//最大値を超えないための処理
            {
                oxyval = 1.0f;
            }
            else
            {
                oxyval += 0.5f;
            }
            Destroy(collision.gameObject);
            snd.PlayOneShot(SE_getItem);
        }
        if (collision.gameObject.tag == "Scoreup" && !isdead)//コイン
        {
            // 宝物に触れた時のスコアアップ処理
            newsystem.score += 100;
            newsystem.coincount++;
            Destroy(collision.gameObject);
            snd.PlayOneShot(SE_getItem);
        }
        if (collision.gameObject.tag == "diya" && !isdead)//ダイヤ
        {
            newsystem.score += 1000;
            newsystem.diyacount++;
            Destroy(collision.gameObject);
            snd.PlayOneShot(SE_getItem);
        }
        if (collision.gameObject.tag == "key" && !isdead)//キーアイテム
        {
            // 宝物に触れた時のスコアアップ処理
            GetKey = true;
            Gotext.text = "ゴールへ向かえ！";
            newsystem.score += 2000;
            Destroy(collision.gameObject);
            snd.PlayOneShot(se_keyitem);
        }
        if (collision.gameObject.tag == "kagi" && !isdead)//ドア開ける鍵
        {
            //newsystem.score += 1000;
            Getkagi = true;
            Destroy(collision.gameObject);
            snd.PlayOneShot(SE_getItem);
        }
        if (collision.gameObject.tag == "EnemyShot" && !isInvincible && !isdead)
        {
            oxyval -= damage;
            snd.PlayOneShot(SE_damage);
            StartCoroutine(StartInvincibility());
        }
        if (collision.gameObject.tag == "sickle" && !isInvincible && !isdead)
        {
            float dam = boss?.second == true ? 1.5f : 1.0f; // ボスの体力半分でダメージ増加
            oxyval -= sickle_damage * dam;
            snd.PlayOneShot(SE_damage);
            StartCoroutine(StartInvincibility());
        }
        if (collision.gameObject.tag == "Thunder" && !isInvincible && !isdead)
        {
            float dam = boss?.second == true ? 1.5f : 1.0f; // ボスの体力半分でダメージ増加
            oxyval -= thunder_damage * dam;
            snd.PlayOneShot(SE_damage);
            StartCoroutine(StartInvincibility());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "safe")
        {
            InsafeZone = false;
        }
        if (other.gameObject.tag == "BossZone")
        {
            InsafeZone = false;
        }
    }

    void Goal()
    {
        goalManager.HandleGoal();//goalmanagerを実行
    }

    private IEnumerator StartInvincibility()
    {
        if (!isdead)
        {
            isInvincible = true;
            anim.SetTrigger("damage");

            // DOTweenで点滅アニメーションを作成
            float halfBlinkDuration = blinkDuration / 2f;
            Sequence blinkSequence = DOTween.Sequence();

            // 点滅のアニメーションを設定
            blinkSequence.Append(spriteRenderer.DOFade(0f, halfBlinkDuration).SetLoops(-1, LoopType.Yoyo));
            blinkSequence.Play();

            // 無敵時間を待機
            yield return new WaitForSeconds(invincibleTime);

            // DOTweenのアニメーションを停止
            blinkSequence.Kill();

            // 元の不透明度に戻す
            spriteRenderer.DOFade(1f, 0f);

            isInvincible = false;
        }
    }//無敵時間

    private IEnumerator ResetAttackCountAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        attackCount = 0;
    }//攻撃リセット

    IEnumerator EndStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isStep = false;
        rb.velocity = Vector2.zero; // ステップ終了後に速度をリセット
    }//ステップのリセット

    IEnumerator ClearVideoPlay()
    {
        clearVideo.Play();
        //yield return new WaitForSeconds(0.1f);
        clearVideoRen.enabled = true;
        yield return new WaitForSeconds(1f);
        snd.PlayOneShot(SE_Clear);
        yield return null;
    }//クリア演出
    IEnumerator VideoSet()
    {
        clearVideo.Play();
        yield return new WaitForSeconds(0.6f);
        clearVideo.Pause();
        yield return null;
    }
}

