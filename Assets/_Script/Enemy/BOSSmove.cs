using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BOSSmove : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    int HP = 20;
    [SerializeField] int maxHP;//設定用
    [SerializeField] Slider HPbar;
    [SerializeField] Image oxycolor;
    [SerializeField] private float detectionRange = 10f; // プレイヤーを検出する範囲
    private bool hasDetectedPlayer = false; // プレイヤーが来たかしたかどうか
    [SerializeField] GameObject[] targetObjs; // ターゲットオブジェクトの配列
    [SerializeField] GameObject Player;
    [SerializeField] float moveSpeed = 2f; // ボスの移動速度
    [SerializeField] float afterAttackDelay = 1f; // 攻撃後の隙間時間
    [SerializeField] GameObject shotObj; // 3way弾のプレハブ
    [SerializeField] GameObject bumeobj;
    [SerializeField] Transform firePoint; // 弾を発射する位置
    [SerializeField] float bulletSpeed = 5f; // 弾の速度
    [SerializeField] float angleSpread = 15f; // 弾の角度

    [SerializeField] GameObject Exitwall;

    [Header("鎌出す位置")] [SerializeField] float ofx, ofy;
    private GameObject currentTarget;// 現在の移動先
    private GameObject previousTarget;//前回の移動先（連続して同じ場所に行かないように）
    private bool isAttacking = false;//攻撃中に動かないように
    private bool isReturning = false; // ブーメランが戻るまで移動を停止

    [Header("落雷の位置")] [SerializeField] public Transform[] attackPositions; // 落雷の発生位置
    [SerializeField] public GameObject lightningobj;//電撃の判定
    [SerializeField] GameObject lightningef;
    [SerializeField] public GameObject[] redLight; // 赤いライトのプレハブ
    [SerializeField] float posy=-4;
    [Header("落雷の速度")] [SerializeField] public float attackSpeed = 5.0f; // 攻撃の速度
    [Header("落雷の確率")] [SerializeField] private float thunderProbability = 0.5f; // 落雷の確率
    [Header("雷の消えるまでの時間")] [SerializeField] private float lightningInterval = 1f; // 落雷の発生間隔
    [Header("雷の降るまでの時間")] [SerializeField] private float Waitlightning = 1.0f; // 落雷の発生間隔

    [SerializeField] GameObject auraPrefab;

    [SerializeField] AudioClip SE_awake, SE_3way, SE_bume, SE_thunder, SE_hit, SE_dead,se_damage,SE_lightning;
    [SerializeField] GameObject hitefect, slefect;
    [SerializeField] Camera maincamera, bosscamera;
    [SerializeField] Canvas UI, canvas;
    private AudioSource snd;
    private Animator animator;
    bool muki;
    bool isawake = false;
    bool isdead = false;
    public  bool second = false;//第二形態フラグ
    bool Isaura = false;
    GameObject auraInstance;//オーラ入れるやつ

    private bool isInvincible = false; // 無敵状態かどうか
    [Header("無敵時間")] [SerializeField] private float invincibleDuration = 1f; // 無敵時間

    private List<GameObject> attackObjects = new List<GameObject>(); // 攻撃オブジェクトのリスト

    void Start()
    {
        HP = maxHP;
        HPbar.value = 1;
        HPbar.gameObject.SetActive(false);
        isawake = false;
        isdead = false;
        Isaura = false;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        snd = gameObject.AddComponent<AudioSource>();
        if (targetObjs.Length > 0)
        {
            MoveToNextTarget();
        }
    }

    void Update()
    {
        if (HP <= 0)
        {
            StartCoroutine(triggerDead());
            isdead = true;
            if (auraInstance != null&&isdead)
            {
                Destroy(auraInstance);
                auraInstance = null;
            }
            return;
        }
        if (Player != null)
        {
            // プレイヤーとの距離を計算
            float distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);

            // 範囲内に入ったら動き出す
            if (distanceToPlayer <= detectionRange)
            {
                if (!isawake)
                {
                    isawake = true;
                    HPbar.gameObject.SetActive(isawake);
                    snd.PlayOneShot(SE_awake);
                }
                hasDetectedPlayer = true;
            }

            if (hasDetectedPlayer && !isAttacking && currentTarget != null && !isReturning)
            {
                MoveTowardsTarget();
            }
        }

        // プレイヤーの方向に応じてボスの向きを変更
        if (Player != null)
        {
            Vector3 directionToPlayer = Player.transform.position - transform.position;

            if (directionToPlayer.x > 0)
            {
                muki = true;
                transform.localScale = new Vector3(0.18f, 0.18f, 0.18f); // プレイヤーが右にいる場合
            }
            else
            {
                muki = false;
                transform.localScale = new Vector3(-0.18f, 0.18f, 0.18f); // プレイヤーが左にいる場合
            }
        }
        if (HP <= maxHP / 2 && !second)
        {
            second = true;
        }
        if (auraPrefab != null)
        {
            if(second && !Isaura&&!isdead)
            {
               auraInstance=Instantiate(auraPrefab, transform.position, Quaternion.identity, transform); // ボスの子としてオーラを追加
                Isaura = true;
            }
        }
    }


    void MoveTowardsTarget()
    {
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1f)
        {
            StartCoroutine(Attack());
        }
    }//移動先への移動

    IEnumerator Attack()
    {
        isAttacking = true;
       // Debug.Log("Starting Attack");

        int attackType;
        bool shouldRetry;

        do
        {
            float randomValue = Random.value; // 0.0 から 1.0 のランダムな値
            attackType = randomValue < thunderProbability ? 2 : Random.Range(0, 2); // thunderProbability の確率で attackType = 2 を選択

            attackType = Random.Range(0, 3);
            shouldRetry = false;

            if (currentTarget.CompareTag("ThunderPos"))
            {
                if (attackType == 2)
                {
                    // currentTarget が "ThunderPos" というタグの場合、attackType が 2 ならば攻撃を選択
                    break;
                }
            }
            else
            {
                // currentTarget が "ThunderPos" でない場合、attackType が 2 の場合はリトライ
                if (attackType == 2)
                {
                    shouldRetry = true;
                }
            }
        }
        while (shouldRetry);

        // attackType に基づく攻撃処理
        switch (attackType)
        {
            case 0:
                //Debug.Log("3Way");
                animator.SetTrigger("3WayShot"); // 3Wayショット
                break;
            case 1:
              //  Debug.Log("Boomerang");
                animator.SetTrigger("BoomeShot"); // 鎌ブーメラン
                isReturning = true;
                break;
            case 2:
              //  Debug.Log("Thunder Attack Triggered");
                animator.SetTrigger("Thunder"); // 落雷
                break;
        }
        yield return new WaitForSeconds(afterAttackDelay);

        isAttacking = false;
        previousTarget = currentTarget;
        MoveToNextTarget();
    }

    void MoveToNextTarget()
    {
        if (targetObjs.Length > 1)
        {
            GameObject nextTarget;
            do
            {
                int randomIndex = Random.Range(0, targetObjs.Length);
                nextTarget = targetObjs[randomIndex];
            } while (nextTarget == previousTarget);

            currentTarget = nextTarget;
            //Debug.Log("Moving to next target: " + currentTarget.name);
        }
        else if (targetObjs.Length == 1)
        {
            currentTarget = targetObjs[0];
            // Debug.Log("Moving to the only target: " + currentTarget.name);
        }
    }//次の移動先を決めるやつ


    public void Fire3Way()
    {
        snd.PlayOneShot(SE_3way);
        // ボスの向いてるほうに出る
        float baseAngle = muki ? 0f : 180f; // 右向きなら0度、左向きなら180度

        // 弾を発射する角度を計算
        float angle1 = baseAngle;
        float angle2 = baseAngle + angleSpread;
        float angle3 = baseAngle - angleSpread;
        float angle4 = baseAngle + angleSpread * 2;
        float angle5 = baseAngle - angleSpread * 2;
        FireBulletAtAngle(angle1);
        FireBulletAtAngle(angle2);
        FireBulletAtAngle(angle3);
        if (second)
        {
            FireBulletAtAngle(angle4);
            FireBulletAtAngle(angle5);
        }
    }//3wayショット

    void FireBulletAtAngle(float angle)
    {
        // 弾の方向を計算
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // 弾を生成
        GameObject bullet = Instantiate(shotObj, firePoint.position, Quaternion.Euler(0, 0, angle));

        // 弾リストに追加
        attackObjects.Add(bullet);

        // 弾に速度を与える
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
    }//弾の生成

    public void FireBoomerang()
    {
        snd.PlayOneShot(SE_bume);
        float direction = muki ? 1 : -1; // 向きに基づいて方向を決定
        Vector3 position = transform.position;
        position.x += ofx * direction; // 向きに基づいてx位置を調整
        position.y += ofy; // y位置を少し上に調整
        animator.SetBool("Isreturn", true);
        // ブーメランの発射
        GameObject boomerang = Instantiate(bumeobj, position, Quaternion.identity);

        // ブーメランリストに追加
        attackObjects.Add(boomerang);

        BoSSbumelan boomerangScript = boomerang.GetComponent<BoSSbumelan>();
        if (boomerangScript != null)
        {
            // プレイヤー方向を向けて発射
            Vector2 directionVector = (Player.transform.position - firePoint.position).normalized;
            boomerang.transform.right = directionVector;
            boomerangScript.origin = firePoint; // 発射元を設定
            boomerangScript.boss = gameObject; // bossを設定
        }
        else
        {
            Debug.LogError("BoSSbumelan script is not attached to the boomerang object.");
        }
    }//鎌のやつ

    public void StartLightning()
    {
        // Debug.Log("Starting Lightning Attack");
        StartCoroutine(LightningAtk());
    }//アニメーションから呼び出す

    public void counter()
    {
        HP -= 2;
        HPbar.value = (float)HP / (float)maxHP;
    }

    public void HideRedLights()
    {
        foreach (GameObject light in redLight)
        {
            if (light != null)
            {
                light.SetActive(false);
            }
        }
    }

    private IEnumerator triggerDead()
    {
        if (!isdead)
        {
            animator.SetBool("dead", true);
            // 死亡サウンドを再生
            snd.PlayOneShot(SE_dead);

            // Exitwall を削除
            Destroy(Exitwall);

            // サウンドが再生中の場合、終了するまで待機

            // すべての攻撃オブジェクトを削除
            foreach (GameObject obj in attackObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            attackObjects.Clear(); // リストをクリア

            // すべてのredLightを削除
            foreach (GameObject light in redLight)
            {
                if (light != null)
                {
                    Destroy(light);
                }
            }
            yield return new WaitWhile(() => snd.isPlaying);
            maincamera.gameObject.SetActive(true); // メインカメラを無効にする
            bosscamera.gameObject.SetActive(false); // ボスカメラを有効にする
            UI.worldCamera = maincamera;
            canvas.worldCamera = maincamera;
        }
        else
        {
            yield break;
        }
        // ボスオブジェクトを削除
        Destroy(gameObject);
    }

    private IEnumerator LightningAtk()
    {
        // Debug.Log("Lightning Attack Coroutine Started");
        snd.PlayOneShot(SE_thunder);

        for (int i = 0; i < attackPositions.Length; i++)//atkposの数だけ
        {
            if (!isdead)
            {
                // 雷の落ちる位置に合わせて、redLightをアクティブにする
                if (redLight[i] != null)
                {
                    redLight[i].SetActive(true); // 落下地点をアクティブにする
                }

                yield return new WaitForSeconds(1.0f);

                Vector3 adjustedPosition = attackPositions[i].position;
                adjustedPosition.y -= posy; //下げる
                snd.PlayOneShot(SE_lightning);
                // 雷を生成する
                GameObject efect = Instantiate(lightningef, adjustedPosition, Quaternion.identity);
                GameObject attack = Instantiate(lightningobj, attackPositions[i].position, Quaternion.identity);

                // 雷リストに追加
                attackObjects.Add(attack);

                // 雷とredLightを消す
                StartCoroutine(DestroyLightningAfterDelay(attack, redLight[i], lightningInterval));

                if (second && i + 1 < attackPositions.Length)// 次の位置にも雷を生成
                {
                    i++; // 次の位置に移動

                    if (!isdead && redLight[i] != null)
                    {
                        redLight[i].SetActive(true);
                    }

                    yield return new WaitForSeconds(1.0f);

                    adjustedPosition = attackPositions[i].position;
                    adjustedPosition.y -= posy;
                    snd.PlayOneShot(SE_lightning);
                    efect = Instantiate(lightningef, adjustedPosition, Quaternion.identity);
                    attack = Instantiate(lightningobj, attackPositions[i].position, Quaternion.identity);

                    attackObjects.Add(attack);

                    StartCoroutine(DestroyLightningAfterDelay(attack, redLight[i], lightningInterval));
                }

                // 雷の生成間隔
                yield return new WaitForSeconds(Waitlightning);
            }
        }
    }

    // 雷とredLightを一定時間後に消すコルーチン
    private IEnumerator DestroyLightningAfterDelay(GameObject lightning, GameObject lightObject, float delay)
    {
        // 指定された時間だけ待機
        yield return new WaitForSeconds(delay);

        // 雷を破壊
        if (lightning != null)
        {
            Destroy(lightning);
        }

        // redLightを非アクティブにする
        if (lightObject != null)
        {
            lightObject.SetActive(false);
        }
    }

    public void BoomerangReturned()
    {
        isReturning = false;
        //   Debug.Log("Boomerang Returned");
    }

    public void TakeDamageFromBoomerang()
    {
        if (isInvincible || HP <= 0) return;

        HP--;
        HPbar.value = (float)HP / (float)maxHP;
        snd.PlayOneShot(se_damage);
        Instantiate(hitefect, transform.position, Quaternion.identity);

        TakeDamage(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "shot" && isawake && !isInvincible && !isdead)
        {
            TakeDamage(collision.transform.position);
            HP-=2;
            HPbar.value = (float)HP / (float)maxHP;
            //Debug.Log("Boss HP: " + HP);
        }
        if (collision.gameObject.tag == "beam" && isawake && !isInvincible && !isdead)
        {
           // TakeDamage(collision.transform.position);
            HP -= 5;
            HPbar.value = (float)HP / (float)maxHP;
            //Debug.Log("Boss HP: " + HP);
        }
        if (collision.gameObject.tag == "lassl" && isawake && !isInvincible && !isdead)
        {
            TakeDamage(collision.transform.position);
            HP--;
            HPbar.value = (float)HP / (float)maxHP;
            //Debug.Log("Boss HP: " + HP);
        }
        if (collision.gameObject.tag == "slash" && isawake && !isInvincible && !isdead)
        {
            Instantiate(hitefect, collision.transform.position, Quaternion.identity);//ヒットエフェクト            Debug.Log("hit");
            snd.PlayOneShot(SE_hit);
            HP--;
            HPbar.value = (float)HP / (float)maxHP;
            //Debug.Log("Boss HP: " + HP);
        }
        if (collision.gameObject.tag == "sickle")
        {
            BoomerangReturned();
            animator.SetBool("Isreturn", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector2 collisionPoint = collision.contacts[0].point;

            // 敵のKnockbackスクリプトを取得
            PlayyerMove playerMove = collision.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // ノックバックを適用
                playerMove.plDamage(collisionPoint);
            }
        }
    }

    public  void TakeDamage(Vector2 hitPosition)
    {
        Instantiate(hitefect, hitPosition, Quaternion.identity); //ヒットエフェクト
        animator.SetTrigger("damage");
      //  Debug.Log("hit");
        snd.PlayOneShot(se_damage);
        // Debug.Log("Boss HP: " + HP);
        StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        // ボスのスプライトレンダラーを取得
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 無敵時間中に点滅させる
            spriteRenderer.DOFade(0, 0.1f).SetLoops(-1, LoopType.Yoyo);
        }

        yield return new WaitForSeconds(invincibleDuration);

        // 無敵時間が終了したら点滅を止めて元の状態に戻す
        if (spriteRenderer != null)
        {
            spriteRenderer.DOKill();
            spriteRenderer.DOFade(1, 0.1f);
        }

        isInvincible = false;
    }
}
