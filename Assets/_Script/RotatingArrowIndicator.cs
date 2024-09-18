using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    [SerializeField] public Transform player;           // プレイヤーのTransform
    [SerializeField] public Transform goal;             // ゴールのTransform
    [SerializeField] public float radius = 2.0f;        // プレイヤーからの距離
    [SerializeField] public float rotationSpeed = 50.0f; // 矢印の速度
    [SerializeField] public float angleThreshold = 1.0f; // 矢印の向きをゴールの方向に一致させるための角度の許容範囲
    [SerializeField] public float rotationAroundPlayerSpeed = 30.0f; // プレイヤーの周囲を回転する速度

    private void Start()
    {
        Vector3 directionToGoal = (goal.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, directionToGoal);
        transform.rotation = targetRotation;

    }

    private void Update()
    {
        
        // ゴールの方向を計算
        Vector3 directionToGoal = (goal.position - transform.position).normalized;

        // 矢印の現在の方向
        Vector3 arrowDirection = transform.up; // 矢印が上向きとして

        // ゴール方向との角度を計算
        float angleToGoal = Vector3.Angle(arrowDirection, directionToGoal);

        // ゴールの方向に向けて回転
        if (angleToGoal > angleThreshold)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, directionToGoal);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 矢印の向きに応じて位置を調整
        AdjustArrowPosition();
    }

    private void AdjustArrowPosition()
    {
        // 矢印が向いている方向を取得
        Vector3 forwardDirection = transform.up;

        // プレイヤーからの位置
        Vector3 playerPosition = player.position;

        // 矢印の向きに基づいて位置を調整
        if (Vector3.Dot(forwardDirection, Vector3.up) > 1f) // 上
        {
            transform.position = playerPosition + Vector3.up * radius;
        }
        else if (Vector3.Dot(forwardDirection, Vector3.right) > 1f) // 右
        {
            transform.position = playerPosition + Vector3.right * radius;
        }
        else if (Vector3.Dot(forwardDirection, Vector3.down) > 1f) // 下
        {
            transform.position = playerPosition + Vector3.down * radius;
        }
        else if (Vector3.Dot(forwardDirection, Vector3.left) > 1f) // 左
        {
            transform.position = playerPosition + Vector3.left * radius;
        }
        else
        {
            // 斜め方向の処理
            Vector3 adjustedPosition = playerPosition + forwardDirection * radius;
            transform.position = adjustedPosition;
        }
    }
}
