using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    [SerializeField] public Transform player;           // �v���C���[��Transform
    [SerializeField] public Transform goal;             // �S�[����Transform
    [SerializeField] public float radius = 2.0f;        // �v���C���[����̋���
    [SerializeField] public float rotationSpeed = 50.0f; // ���̑��x
    [SerializeField] public float angleThreshold = 1.0f; // ���̌������S�[���̕����Ɉ�v�����邽�߂̊p�x�̋��e�͈�
    [SerializeField] public float rotationAroundPlayerSpeed = 30.0f; // �v���C���[�̎��͂���]���鑬�x

    private void Start()
    {
        Vector3 directionToGoal = (goal.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, directionToGoal);
        transform.rotation = targetRotation;

    }

    private void Update()
    {
        
        // �S�[���̕������v�Z
        Vector3 directionToGoal = (goal.position - transform.position).normalized;

        // ���̌��݂̕���
        Vector3 arrowDirection = transform.up; // ��󂪏�����Ƃ���

        // �S�[�������Ƃ̊p�x���v�Z
        float angleToGoal = Vector3.Angle(arrowDirection, directionToGoal);

        // �S�[���̕����Ɍ����ĉ�]
        if (angleToGoal > angleThreshold)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, directionToGoal);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // ���̌����ɉ����Ĉʒu�𒲐�
        AdjustArrowPosition();
    }

    private void AdjustArrowPosition()
    {
        // ��󂪌����Ă���������擾
        Vector3 forwardDirection = transform.up;

        // �v���C���[����̈ʒu
        Vector3 playerPosition = player.position;

        // ���̌����Ɋ�Â��Ĉʒu�𒲐�
        if (Vector3.Dot(forwardDirection, Vector3.up) > 1f) // ��
        {
            transform.position = playerPosition + Vector3.up * radius;
        }
        else if (Vector3.Dot(forwardDirection, Vector3.right) > 1f) // �E
        {
            transform.position = playerPosition + Vector3.right * radius;
        }
        else if (Vector3.Dot(forwardDirection, Vector3.down) > 1f) // ��
        {
            transform.position = playerPosition + Vector3.down * radius;
        }
        else if (Vector3.Dot(forwardDirection, Vector3.left) > 1f) // ��
        {
            transform.position = playerPosition + Vector3.left * radius;
        }
        else
        {
            // �΂ߕ����̏���
            Vector3 adjustedPosition = playerPosition + forwardDirection * radius;
            transform.position = adjustedPosition;
        }
    }
}
