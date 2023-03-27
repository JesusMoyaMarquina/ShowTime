using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        if (player != null)
        {
            Vector3 position = transform.position;
            position.x = player.transform.position.x;
            position.y = player.transform.position.y;
            transform.position = position;
        }
    }
}
