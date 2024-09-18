using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // プレイヤーのTransform
    public float smoothSpeed = 0.125f; // カメラのスムーズさ
    public Vector3 offset; // プレイヤーとカメラのオフセット

    private void LateUpdate()
    {
        // プレイヤーの現在位置にオフセットを加えた位置を求める
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, transform.position.y, transform.position.z);

        // 現在のカメラ位置からスムーズに目標位置へ移動
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // スムーズに移動した位置にカメラを設定
        transform.position = smoothedPosition;
    }
}
