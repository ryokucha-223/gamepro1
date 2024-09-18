using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // �v���C���[��Transform
    public float smoothSpeed = 0.125f; // �J�����̃X���[�Y��
    public Vector3 offset; // �v���C���[�ƃJ�����̃I�t�Z�b�g

    private void LateUpdate()
    {
        // �v���C���[�̌��݈ʒu�ɃI�t�Z�b�g���������ʒu�����߂�
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, transform.position.y, transform.position.z);

        // ���݂̃J�����ʒu����X���[�Y�ɖڕW�ʒu�ֈړ�
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // �X���[�Y�Ɉړ������ʒu�ɃJ������ݒ�
        transform.position = smoothedPosition;
    }
}
