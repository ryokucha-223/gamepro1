using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyZone : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keytxt;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            keytxt.text = "�����K�v�I";
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            keytxt.text = " ";
        }
    }
}
