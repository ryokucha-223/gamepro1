using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject PlayerObject; // player�I�u�W�F�N�g���󂯎���
    public Transform Player; // �v���C���[�̍��W���Ȃǂ��󂯎���
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
