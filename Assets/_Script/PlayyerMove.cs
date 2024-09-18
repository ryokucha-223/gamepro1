using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic; // HashSet<T> ���g�p���邽�߂ɕK�v
using UnityEngine.Video;
using Unity.VisualScripting.Antlr3.Runtime;
using DG.Tweening; // DOTween�̖��O���
using System.Threading.Tasks; // Task ���g�p���邽�߂ɕK�v



public class PlayyerMove : MonoBehaviour
{
    [SerializeField] GameObject jumpEff;
    private SpriteRenderer spriteRenderer;
    public float jumpPower = 10.0f;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float stepdis = 1.0f;
    [SerializeField] float stepmove = 2f;//�X�e�b�v����
    [SerializeField] float backForce = 2.0f;
    //�_�f�֘A
    [SerializeField] float oxyspeed = 0.000001f;//�_�f����ʁH
    [SerializeField] public static float oxyval = 1.0f;//�_�f�ʁH
    [SerializeField] float startoxy = 1.0f;//�_�f���Z�b�g�p�̓z��̂Ɠ������l��
    [SerializeField] float oxyheal = 0.5f;//�_�f�̉񕜗ʁi1.0�𒴂������͖����Ȃ���̂Ƃ���j
    [SerializeField] float oxyshot = 0.05f;//�ˌ��Ō���_�f��
    [SerializeField] float oxybeam = 0.1f;//�r�[���Ō����
    [SerializeField] float oxystep = 0.02f;//�X�e�b�v�ŏ����_�f
    [SerializeField] float damage = 0.1f, thunder_damage = 0.1f, sickle_damage = 0.1f;//�_���[�W�󂯂Č���_�f��

    [SerializeField] float attackWindow = 0.3f; // �U�����͎�t���ԁi�b�j
    [SerializeField] float attackCooldown = 0.5f;//�ߐڂ̃N�[���^�C��
    [SerializeField] float attackCooldownAfterCombo = 1.0f; // �O�i�ڂ̌�̃N�[���^�C��
    private int attackCount = 0;//�ߐڂ̉񐔁i�ő�R�j
    private bool isInAttackCooldown = false; // �N�[���^�C�������ǂ����������t���O
    private float lastAttackTime;
    private float lastAttackInputTime;
    public static bool muki = true; // true�ŉE����
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 force;
    float shotwait = 0;
    float beamwait = 0;
    [SerializeField] private float invincibleTime = 2.0f; // ���G����
    [SerializeField] private float blinkDuration = 0.2f;  // �_�ł̊Ԋu
    private bool isInvincible = false;
    [SerializeField] TextMeshProUGUI scoretext, cleartext, Gotext;
    [SerializeField] GameObject getkeyflag;
    [SerializeField] Slider oxybar;
    [SerializeField] Image oxycolor;
    [SerializeField] GameObject clearVideoObj;//�N���A���̃r�f�I�i�[������
    private MeshRenderer clearVideoRen;//�����x�ő���
    private VideoPlayer clearVideo;//�N���A���̃r�f�I
    [SerializeField] GameObject shotobj;
    [SerializeField] GameObject slobj, lasslobj;//�ߐڂ̔���
    [SerializeField] GameObject slefc;//�ߐڂ̃G�t�F�N�g
    private GameObject atkobj;//�ߐڂ̃G�t�F�N�g�������
    private GameObject atkcol;//�ߐڔ���������
    [SerializeField] GameObject Shotefect; // �e���o���Ƃ��̃G�t�F�N�g
    [SerializeField] GameObject bemefe;
    [SerializeField] GameObject beamobj;
    [SerializeField]
    AudioClip SE_jump, SE_shot, SE_damage, SE_dead, SE_getItem, SE_Clear, SE_slash, SE_open,
     SE_step, SE_goal, se_keyitem;//se
    AudioSource snd;//���o�����
    bool isdead;//���S����
    bool GetKey;
    bool Getkagi;
    bool isclear;//�N���A����
    private Vector3 targetPosition;
    private bool isStep = false;
    private bool canstep = true;
    //bool canmove;
    private bool canMoveLeft = true;//���E�̕ǂɐG��Ă鎞�̏���
    private bool canMoveRight = true;//
    private bool InsafeZone;//�Z�[�t�]�[���̒��ɂ���Ƃ�
    private bool IsAtk = false;
    public bool clstFlag;
    private HashSet<Collider2D> leftColliders = new HashSet<Collider2D>();
    private HashSet<Collider2D> rightColliders = new HashSet<Collider2D>();
    [SerializeField] GameObject arrowobj, itemarrow;
    [SerializeField] private BOSSmove boss;
    private GoalManager goalManager;//goalmanager�̃v���O����
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
        GameManager.Instance.SavePreviousScene(currentScene); // ���݂̃V�[����ۑ�
        goalManager = FindObjectOfType<GoalManager>(); // GoalManager�����Q�[���I�u�W�F�N�g������
        StartCoroutine(VideoSet());
        fademane = FindObjectOfType<Fademane>();//fademane�̎擾
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
        }//��莞�Ԍo�߂ōU�����Z�b�g
        scoretext.text = "�҂�������: " + newsystem.score;
        if (!isclear)
        {
            if (!InsafeZone)
            {
                oxyval -= oxyspeed * Time.deltaTime;
            }
        }//�_�f����
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
        }//arrow�̏���
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
        }//���S����

    }

    private async void HandleAttackAsync()
    {
        // �񓯊������ōU���������Ăяo��
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
        else // ���͂��Ȃ��Ƃ�
        {
            anim.SetBool("walk", false); 
            Vector2 velocity = rb.velocity;
        }
    }// �ړ�

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
    }//�W�����v

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
        if (!InsafeZone)//�Q�[�W����
        {
            oxyval -= oxystep;
        }
        float dir = muki ? 1 : -1;
        targetPosition = transform.position + new Vector3(stepmove * dir, 0, 0);

        // �X�e�b�v�J�n
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
            // AttackFollow �X�N���v�g��ǉ����A�v���C���[�� Transform �ƃI�t�Z�b�g��ݒ�
            AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
            followScript.playerTransform = transform;
            followScript.offset = new Vector2(1f * direction, 0.2f);
            snd.PlayOneShot(SE_shot);
        }
        return false;
    }//�ˌ��p��

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

    }//�e�̌Ăяo���Ƃ�

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
            // AttackFollow �X�N���v�g��ǉ����A�v���C���[�� Transform �ƃI�t�Z�b�g��ݒ�
            AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
            followScript.playerTransform = transform;
            followScript.offset = new Vector2(1f * direction, 0.2f);
            snd.PlayOneShot(SE_shot);
        }
        return false;
    }//�r�[���̗p��

    void plbeam()
    {

        float direction = muki ? 1 : -1;
        float rot = muki ? 0 : 180;
        Vector2 position = new Vector2(transform.position.x + 12f * direction, transform.position.y + 0.2f);
        Quaternion rotation = Quaternion.Euler(-90f, 0, 0 + rot);
        atkcol = Instantiate(beamobj, position, Quaternion.identity);
        atkobj = Instantiate(bemefe, position, rotation);
        Vector3 newScale = new Vector3(direction, 1f, 1f); // X�����ɃX�P�[����ύX
        atkobj.transform.localScale = newScale;
        // AttackFollow �X�N���v�g��ǉ����A�v���C���[�� Transform �ƃI�t�Z�b�g��ݒ�
        AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
        followScript.playerTransform = transform;
        followScript.offset = new Vector2(1f * direction, 0.2f);
        // AttackFollow 
        AttackFollow followScriptAtkCol = atkcol.AddComponent<AttackFollow>();
        followScriptAtkCol.playerTransform = transform;
        followScriptAtkCol.offset = new Vector2(14f * direction, 0.2f);

        // �p�[�e�B�N���V�X�e���̏I�����Ď�
        StartCoroutine(DestroyOnParticleEnd(atkobj, atkcol));

        beamwait = 1f;
    }
    private IEnumerator DestroyOnParticleEnd(GameObject particleObject1, GameObject particleObject2)
    {
        // ParticleSystem �R���|�[�l���g���擾
        ParticleSystem ps = particleObject1.GetComponent<ParticleSystem>();

        // ParticleSystem �����݂���ꍇ
        if (ps != null)
        {
            // �p�[�e�B�N�����I������܂őҋ@
            while (ps.isPlaying)
            {
                yield return null;
            }
        }

        // �p�[�e�B�N���V�X�e�����I��������I�u�W�F�N�g��j��
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
            // �N�[���^�C�������ǂ������`�F�b�N
            if (isInAttackCooldown)
            {
                return;
            }

            canstep = false;
            float currentTime = Time.time;

            // �N�[���^�C�����o�߂��Ă���΍U���J�E���g�����Z�b�g
            if (currentTime - lastAttackTime > attackCooldown)
            {
                attackCount = 0;
            }

            // �U���J�E���g�ƍU���E�B���h�E�̃`�F�b�N
            if (attackCount == 0 || currentTime - lastAttackInputTime < attackWindow)
            {
                attackCount++;
                lastAttackTime = currentTime;
                lastAttackInputTime = currentTime;

                // �U���A�j���[�V�����̃g���K�[
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
                        attackCount = 0; // 3��ڂ̍U����̓��Z�b�g

                        // 3���ڂ̍U����ɃN�[���^�C����ݒ�
                        isInAttackCooldown = true;
                        await Task.Delay((int)(attackCooldownAfterCombo * 1000)); // �N�[���^�C����Ƀt���O�����Z�b�g
                        isInAttackCooldown = false;
                        break;
                }
            }
            else
            {
                // �U���J�E���g��3�����ł���Βʏ�̃N�[���^�C��
                await Task.Delay((int)(attackCooldown * 1000));
            }
        }
    }

    // �ߐڂ̓��́A�A�j���[�V�����̌Ăяo��


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
        // AttackFollow �X�N���v�g��ǉ����A�v���C���[�� Transform �ƃI�t�Z�b�g��ݒ�
        AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
        followScript.playerTransform = transform;
        followScript.offset = new Vector2(1f * direction, 0.2f);
        // AttackFollow 
        AttackFollow followScriptAtkCol = atkcol.AddComponent<AttackFollow>();
        followScriptAtkCol.playerTransform = transform;
        followScriptAtkCol.offset = new Vector2(1f * direction, 0.2f);
    }//1,2�i�ڂ̔���Ăяo��

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
        // AttackFollow �X�N���v�g��ǉ����A�v���C���[�� Transform �ƃI�t�Z�b�g��ݒ�
        AttackFollow followScript = atkobj.AddComponent<AttackFollow>();
        followScript.playerTransform = transform;
        followScript.offset = new Vector2(1f * direction, 0.2f);
        // AttackFollow 
        AttackFollow followScriptAtkCol = atkcol.AddComponent<AttackFollow>();
        followScriptAtkCol.playerTransform = transform;
        followScriptAtkCol.offset = new Vector2(1f * direction, 0.2f);
        // Debug.Log("����");
    }//3�i�ڂ̔���Ăяo��

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
    }//�A�j���[�V��������m�b�N�o�b�N���Ăяo��



    IEnumerator triggerDead()
    {
        snd.PlayOneShot(SE_dead);
        yield return new WaitWhile(() => snd.isPlaying);
        if (fademane != null)
        {
            fademane.ChangeSceneWithFade(1f, 0.5f, "Gameover"); // �t�F�[�h���Ԃ͓K�X����
        }
        else
        {
            SceneManager.LoadScene("Gameover");
        }
    }//���S����

    public void plDamage(Vector2 collisionPoint)
    {
        if (!isInvincible || !isdead)
        {
            // �m�b�N�o�b�N�������v�Z�i�Փ˓_���炱�̃I�u�W�F�N�g�̒��S�ւ̃x�N�g���j
            Vector2 knockbackDirection = (rb.position - collisionPoint).normalized;

            // �m�b�N�o�b�N�̗͂�K�p
            rb.AddForce(knockbackDirection * backForce, ForceMode2D.Impulse);

        }
    }//�m�b�N�o�b�N����

    public void StopKnockback()
    {
        rb.velocity = Vector2.zero; // �m�b�N�o�b�N���ɑ��x���c���Ă����烊�Z�b�g
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

    private void OnTriggerEnter2D(Collider2D collision)//�A�C�e���Ƃ��ɐG�ꂽ���̏���
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
            StartCoroutine(ClearVideoPlay());//�f���Ăяo��
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
        if (collision.gameObject.tag == "Bubble" && !isdead)//�_�f��
        {
            if (oxyval + oxyheal >= 1.0)//�ő�l�𒴂��Ȃ����߂̏���
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
        if (collision.gameObject.tag == "Scoreup" && !isdead)//�R�C��
        {
            // �󕨂ɐG�ꂽ���̃X�R�A�A�b�v����
            newsystem.score += 100;
            newsystem.coincount++;
            Destroy(collision.gameObject);
            snd.PlayOneShot(SE_getItem);
        }
        if (collision.gameObject.tag == "diya" && !isdead)//�_�C��
        {
            newsystem.score += 1000;
            newsystem.diyacount++;
            Destroy(collision.gameObject);
            snd.PlayOneShot(SE_getItem);
        }
        if (collision.gameObject.tag == "key" && !isdead)//�L�[�A�C�e��
        {
            // �󕨂ɐG�ꂽ���̃X�R�A�A�b�v����
            GetKey = true;
            Gotext.text = "�S�[���֌������I";
            newsystem.score += 2000;
            Destroy(collision.gameObject);
            snd.PlayOneShot(se_keyitem);
        }
        if (collision.gameObject.tag == "kagi" && !isdead)//�h�A�J���錮
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
            float dam = boss?.second == true ? 1.5f : 1.0f; // �{�X�̗͔̑����Ń_���[�W����
            oxyval -= sickle_damage * dam;
            snd.PlayOneShot(SE_damage);
            StartCoroutine(StartInvincibility());
        }
        if (collision.gameObject.tag == "Thunder" && !isInvincible && !isdead)
        {
            float dam = boss?.second == true ? 1.5f : 1.0f; // �{�X�̗͔̑����Ń_���[�W����
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
        goalManager.HandleGoal();//goalmanager�����s
    }

    private IEnumerator StartInvincibility()
    {
        if (!isdead)
        {
            isInvincible = true;
            anim.SetTrigger("damage");

            // DOTween�œ_�ŃA�j���[�V�������쐬
            float halfBlinkDuration = blinkDuration / 2f;
            Sequence blinkSequence = DOTween.Sequence();

            // �_�ł̃A�j���[�V������ݒ�
            blinkSequence.Append(spriteRenderer.DOFade(0f, halfBlinkDuration).SetLoops(-1, LoopType.Yoyo));
            blinkSequence.Play();

            // ���G���Ԃ�ҋ@
            yield return new WaitForSeconds(invincibleTime);

            // DOTween�̃A�j���[�V�������~
            blinkSequence.Kill();

            // ���̕s�����x�ɖ߂�
            spriteRenderer.DOFade(1f, 0f);

            isInvincible = false;
        }
    }//���G����

    private IEnumerator ResetAttackCountAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        attackCount = 0;
    }//�U�����Z�b�g

    IEnumerator EndStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isStep = false;
        rb.velocity = Vector2.zero; // �X�e�b�v�I����ɑ��x�����Z�b�g
    }//�X�e�b�v�̃��Z�b�g

    IEnumerator ClearVideoPlay()
    {
        clearVideo.Play();
        //yield return new WaitForSeconds(0.1f);
        clearVideoRen.enabled = true;
        yield return new WaitForSeconds(1f);
        snd.PlayOneShot(SE_Clear);
        yield return null;
    }//�N���A���o
    IEnumerator VideoSet()
    {
        clearVideo.Play();
        yield return new WaitForSeconds(0.6f);
        clearVideo.Pause();
        yield return null;
    }
}

