using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BOSSmove : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    int HP = 20;
    [SerializeField] int maxHP;//�ݒ�p
    [SerializeField] Slider HPbar;
    [SerializeField] Image oxycolor;
    [SerializeField] private float detectionRange = 10f; // �v���C���[�����o����͈�
    private bool hasDetectedPlayer = false; // �v���C���[���������������ǂ���
    [SerializeField] GameObject[] targetObjs; // �^�[�Q�b�g�I�u�W�F�N�g�̔z��
    [SerializeField] GameObject Player;
    [SerializeField] float moveSpeed = 2f; // �{�X�̈ړ����x
    [SerializeField] float afterAttackDelay = 1f; // �U����̌��Ԏ���
    [SerializeField] GameObject shotObj; // 3way�e�̃v���n�u
    [SerializeField] GameObject bumeobj;
    [SerializeField] Transform firePoint; // �e�𔭎˂���ʒu
    [SerializeField] float bulletSpeed = 5f; // �e�̑��x
    [SerializeField] float angleSpread = 15f; // �e�̊p�x

    [SerializeField] GameObject Exitwall;

    [Header("���o���ʒu")] [SerializeField] float ofx, ofy;
    private GameObject currentTarget;// ���݂̈ړ���
    private GameObject previousTarget;//�O��̈ړ���i�A�����ē����ꏊ�ɍs���Ȃ��悤�Ɂj
    private bool isAttacking = false;//�U�����ɓ����Ȃ��悤��
    private bool isReturning = false; // �u�[���������߂�܂ňړ����~

    [Header("�����̈ʒu")] [SerializeField] public Transform[] attackPositions; // �����̔����ʒu
    [SerializeField] public GameObject lightningobj;//�d���̔���
    [SerializeField] GameObject lightningef;
    [SerializeField] public GameObject[] redLight; // �Ԃ����C�g�̃v���n�u
    [SerializeField] float posy=-4;
    [Header("�����̑��x")] [SerializeField] public float attackSpeed = 5.0f; // �U���̑��x
    [Header("�����̊m��")] [SerializeField] private float thunderProbability = 0.5f; // �����̊m��
    [Header("���̏�����܂ł̎���")] [SerializeField] private float lightningInterval = 1f; // �����̔����Ԋu
    [Header("���̍~��܂ł̎���")] [SerializeField] private float Waitlightning = 1.0f; // �����̔����Ԋu

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
    public  bool second = false;//���`�ԃt���O
    bool Isaura = false;
    GameObject auraInstance;//�I�[���������

    private bool isInvincible = false; // ���G��Ԃ��ǂ���
    [Header("���G����")] [SerializeField] private float invincibleDuration = 1f; // ���G����

    private List<GameObject> attackObjects = new List<GameObject>(); // �U���I�u�W�F�N�g�̃��X�g

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
            // �v���C���[�Ƃ̋������v�Z
            float distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);

            // �͈͓��ɓ������瓮���o��
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

        // �v���C���[�̕����ɉ����ă{�X�̌�����ύX
        if (Player != null)
        {
            Vector3 directionToPlayer = Player.transform.position - transform.position;

            if (directionToPlayer.x > 0)
            {
                muki = true;
                transform.localScale = new Vector3(0.18f, 0.18f, 0.18f); // �v���C���[���E�ɂ���ꍇ
            }
            else
            {
                muki = false;
                transform.localScale = new Vector3(-0.18f, 0.18f, 0.18f); // �v���C���[�����ɂ���ꍇ
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
               auraInstance=Instantiate(auraPrefab, transform.position, Quaternion.identity, transform); // �{�X�̎q�Ƃ��ăI�[����ǉ�
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
    }//�ړ���ւ̈ړ�

    IEnumerator Attack()
    {
        isAttacking = true;
       // Debug.Log("Starting Attack");

        int attackType;
        bool shouldRetry;

        do
        {
            float randomValue = Random.value; // 0.0 ���� 1.0 �̃����_���Ȓl
            attackType = randomValue < thunderProbability ? 2 : Random.Range(0, 2); // thunderProbability �̊m���� attackType = 2 ��I��

            attackType = Random.Range(0, 3);
            shouldRetry = false;

            if (currentTarget.CompareTag("ThunderPos"))
            {
                if (attackType == 2)
                {
                    // currentTarget �� "ThunderPos" �Ƃ����^�O�̏ꍇ�AattackType �� 2 �Ȃ�΍U����I��
                    break;
                }
            }
            else
            {
                // currentTarget �� "ThunderPos" �łȂ��ꍇ�AattackType �� 2 �̏ꍇ�̓��g���C
                if (attackType == 2)
                {
                    shouldRetry = true;
                }
            }
        }
        while (shouldRetry);

        // attackType �Ɋ�Â��U������
        switch (attackType)
        {
            case 0:
                //Debug.Log("3Way");
                animator.SetTrigger("3WayShot"); // 3Way�V���b�g
                break;
            case 1:
              //  Debug.Log("Boomerang");
                animator.SetTrigger("BoomeShot"); // ���u�[������
                isReturning = true;
                break;
            case 2:
              //  Debug.Log("Thunder Attack Triggered");
                animator.SetTrigger("Thunder"); // ����
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
    }//���̈ړ�������߂���


    public void Fire3Way()
    {
        snd.PlayOneShot(SE_3way);
        // �{�X�̌����Ă�ق��ɏo��
        float baseAngle = muki ? 0f : 180f; // �E�����Ȃ�0�x�A�������Ȃ�180�x

        // �e�𔭎˂���p�x���v�Z
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
    }//3way�V���b�g

    void FireBulletAtAngle(float angle)
    {
        // �e�̕������v�Z
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // �e�𐶐�
        GameObject bullet = Instantiate(shotObj, firePoint.position, Quaternion.Euler(0, 0, angle));

        // �e���X�g�ɒǉ�
        attackObjects.Add(bullet);

        // �e�ɑ��x��^����
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
    }//�e�̐���

    public void FireBoomerang()
    {
        snd.PlayOneShot(SE_bume);
        float direction = muki ? 1 : -1; // �����Ɋ�Â��ĕ���������
        Vector3 position = transform.position;
        position.x += ofx * direction; // �����Ɋ�Â���x�ʒu�𒲐�
        position.y += ofy; // y�ʒu��������ɒ���
        animator.SetBool("Isreturn", true);
        // �u�[�������̔���
        GameObject boomerang = Instantiate(bumeobj, position, Quaternion.identity);

        // �u�[���������X�g�ɒǉ�
        attackObjects.Add(boomerang);

        BoSSbumelan boomerangScript = boomerang.GetComponent<BoSSbumelan>();
        if (boomerangScript != null)
        {
            // �v���C���[�����������Ĕ���
            Vector2 directionVector = (Player.transform.position - firePoint.position).normalized;
            boomerang.transform.right = directionVector;
            boomerangScript.origin = firePoint; // ���ˌ���ݒ�
            boomerangScript.boss = gameObject; // boss��ݒ�
        }
        else
        {
            Debug.LogError("BoSSbumelan script is not attached to the boomerang object.");
        }
    }//���̂��

    public void StartLightning()
    {
        // Debug.Log("Starting Lightning Attack");
        StartCoroutine(LightningAtk());
    }//�A�j���[�V��������Ăяo��

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
            // ���S�T�E���h���Đ�
            snd.PlayOneShot(SE_dead);

            // Exitwall ���폜
            Destroy(Exitwall);

            // �T�E���h���Đ����̏ꍇ�A�I������܂őҋ@

            // ���ׂĂ̍U���I�u�W�F�N�g���폜
            foreach (GameObject obj in attackObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            attackObjects.Clear(); // ���X�g���N���A

            // ���ׂĂ�redLight���폜
            foreach (GameObject light in redLight)
            {
                if (light != null)
                {
                    Destroy(light);
                }
            }
            yield return new WaitWhile(() => snd.isPlaying);
            maincamera.gameObject.SetActive(true); // ���C���J�����𖳌��ɂ���
            bosscamera.gameObject.SetActive(false); // �{�X�J������L���ɂ���
            UI.worldCamera = maincamera;
            canvas.worldCamera = maincamera;
        }
        else
        {
            yield break;
        }
        // �{�X�I�u�W�F�N�g���폜
        Destroy(gameObject);
    }

    private IEnumerator LightningAtk()
    {
        // Debug.Log("Lightning Attack Coroutine Started");
        snd.PlayOneShot(SE_thunder);

        for (int i = 0; i < attackPositions.Length; i++)//atkpos�̐�����
        {
            if (!isdead)
            {
                // ���̗�����ʒu�ɍ��킹�āAredLight���A�N�e�B�u�ɂ���
                if (redLight[i] != null)
                {
                    redLight[i].SetActive(true); // �����n�_���A�N�e�B�u�ɂ���
                }

                yield return new WaitForSeconds(1.0f);

                Vector3 adjustedPosition = attackPositions[i].position;
                adjustedPosition.y -= posy; //������
                snd.PlayOneShot(SE_lightning);
                // ���𐶐�����
                GameObject efect = Instantiate(lightningef, adjustedPosition, Quaternion.identity);
                GameObject attack = Instantiate(lightningobj, attackPositions[i].position, Quaternion.identity);

                // �����X�g�ɒǉ�
                attackObjects.Add(attack);

                // ����redLight������
                StartCoroutine(DestroyLightningAfterDelay(attack, redLight[i], lightningInterval));

                if (second && i + 1 < attackPositions.Length)// ���̈ʒu�ɂ����𐶐�
                {
                    i++; // ���̈ʒu�Ɉړ�

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

                // ���̐����Ԋu
                yield return new WaitForSeconds(Waitlightning);
            }
        }
    }

    // ����redLight����莞�Ԍ�ɏ����R���[�`��
    private IEnumerator DestroyLightningAfterDelay(GameObject lightning, GameObject lightObject, float delay)
    {
        // �w�肳�ꂽ���Ԃ����ҋ@
        yield return new WaitForSeconds(delay);

        // ����j��
        if (lightning != null)
        {
            Destroy(lightning);
        }

        // redLight���A�N�e�B�u�ɂ���
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
            Instantiate(hitefect, collision.transform.position, Quaternion.identity);//�q�b�g�G�t�F�N�g            Debug.Log("hit");
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

            // �G��Knockback�X�N���v�g���擾
            PlayyerMove playerMove = collision.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // �m�b�N�o�b�N��K�p
                playerMove.plDamage(collisionPoint);
            }
        }
    }

    public  void TakeDamage(Vector2 hitPosition)
    {
        Instantiate(hitefect, hitPosition, Quaternion.identity); //�q�b�g�G�t�F�N�g
        animator.SetTrigger("damage");
      //  Debug.Log("hit");
        snd.PlayOneShot(se_damage);
        // Debug.Log("Boss HP: " + HP);
        StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        // �{�X�̃X�v���C�g�����_���[���擾
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // ���G���Ԓ��ɓ_�ł�����
            spriteRenderer.DOFade(0, 0.1f).SetLoops(-1, LoopType.Yoyo);
        }

        yield return new WaitForSeconds(invincibleDuration);

        // ���G���Ԃ��I��������_�ł��~�߂Č��̏�Ԃɖ߂�
        if (spriteRenderer != null)
        {
            spriteRenderer.DOKill();
            spriteRenderer.DOFade(1, 0.1f);
        }

        isInvincible = false;
    }
}
