using UnityEngine;

public class lightning : MonoBehaviour
{
    public GameObject associatedLight; // ���Ɋ֘A�t����ꂽ���C�g

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.gameObject.CompareTag("Player"))
        {
            if (associatedLight != null)
            {
                associatedLight.SetActive(false); // �G�t�F�N�g���\��
            }
            Destroy(gameObject); // ���I�u�W�F�N�g���폜
        }
    }
}
