using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    //public float width = 30;

    private void Update()
    {
        if(transform.position.x <= -25)
        {
            Reposition();
        }
    }

    void Reposition()
    {
        Vector2 offset = new Vector2(25, 0);
        transform.position = (Vector2) transform.position + offset;
    }
}
