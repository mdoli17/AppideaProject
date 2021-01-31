using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public LayerMask playerMask;
    public LayerMask enemyMask;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit2D = CameraToScreen(playerMask);
            if (hit2D.collider)
            {
                if(PlayerController.playerState == PlayerState.Idle)
                {
                    PlayerController.playerState = PlayerState.Aiming;
                    // Can add animations for aiming
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(PlayerController.playerState == PlayerState.Aiming)
            {
                RaycastHit2D hit2D = CameraToScreen(enemyMask);
                if (hit2D.collider)
                {

                    GameObject targetObject = hit2D.collider.gameObject;

                    PlayerController.FireSingleArrow(targetObject);

                    PlayerController.playerState = PlayerState.Firing;
                }
                else
                {
                    PlayerController.playerState = PlayerState.Idle;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {   
            if(PlayerController.playerState == PlayerState.Idle)
            {
                PlayerController.playerState = PlayerState.Firing;
                PlayerController.UseUltimate();
            }
        }
    }

    private RaycastHit2D CameraToScreen(LayerMask mask)
    {
        Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 MousePosition = new Vector2(pz.x, pz.y);

        return Physics2D.Raycast(MousePosition, Vector2.zero, Mathf.Infinity, mask);
    }
}
