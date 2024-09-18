using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObject : MonoBehaviour
{
    [SerializeField] GameObject obj;


    void Start()
    {
        for (int y = 0; y < 15; y++)
        {
            for (int x = 0; x < 15; x++)
            {
                Instantiate(obj, new Vector3(-6f + 0.4f * x , -1.8f + 0.4f * y, 0), Quaternion.identity);
            }
        }
    }

    void Update()
    {
        
    }
}
