using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollitions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Player Ignore Enemy Collisions
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Mele"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Distance"));

        //Enemy Ignore Other Enemy Collisions
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Mele"), LayerMask.NameToLayer("Distance"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Distance"), LayerMask.NameToLayer("Mele"));
    }
}
