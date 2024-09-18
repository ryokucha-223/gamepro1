using UnityEngine;

public class AttackFollow : MonoBehaviour
{
    public Transform playerTransform; // �v���C���[��Transform��ݒ�
    public Vector2 offset; // �v���C���[�Ƃ̑��ΓI�ȃI�t�Z�b�g�ʒu
    public bool shouldFollow = true; // �Ǐ]���邩�ǂ����𐧌䂷��t���O

    void Update()
    {
        // shouldFollow �� true �̏ꍇ�̂݁A�v���C���[�̈ʒu�Ɋ�Â��čU������I�u�W�F�N�g�̈ʒu���X�V
        if (playerTransform != null)
        {
            transform.position = new Vector2(playerTransform.position.x + offset.x, playerTransform.position.y + offset.y);
        }
    }
}
