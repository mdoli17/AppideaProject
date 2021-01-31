using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
    
{   
    [SerializeField]
    SpriteRenderer background;

    void Update()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = background.bounds.size.x / background.bounds.size.y;

        Camera.main.orthographicSize = (screenRatio >= targetRatio) ? background.size.y / 2 : background.bounds.size.y / 2 * targetRatio / screenRatio;
    }
}
