using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class ArrowScript : MonoBehaviour
{
    private const float ARROW_MAX_HEIGHT = 10f;

    public readonly float gravity = -30f;
    
    [Range(0, 30f)]
    public float SingleArrowSpeed = 15f;
    
    private bool bIsUlti;

    private GameObject blastVFX;

    private BoxCollider2D coll;
    
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        coll.isTrigger = true;
        coll.size = new Vector2(0.2f, 0.2f);
        GetComponent<Rigidbody2D>().isKinematic = true;
        if(!bIsUlti)
            Destroy(this.gameObject, 3f);
        
    }

    // In case a single arrow is blocked by a different enemy
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if(!bIsUlti)
            {
                OnImpact();
            }
        }
    }
    public void ShootSingle(GameObject targetObject, GameObject vfx)
    {
        bIsUlti = false;
        blastVFX = vfx;
        Vector3 target = targetObject.transform.position + new Vector3(0, targetObject.GetComponent<BoxCollider2D>().bounds.size.y / 2);
        StartCoroutine(SmoothShot(target));
    }
    public void Ultimate(Vector3 TargetPosition, GameObject vfx)
    {
        blastVFX = vfx;
        bIsUlti = true;
        StartCoroutine(SimulateParabola(TargetPosition));
    }


    IEnumerator SmoothShot(Vector3 finalPos)
    {
        Vector3 startingPos = transform.position;

        float timeToFly = Mathf.Abs(finalPos.x - startingPos.x) / SingleArrowSpeed;
        float elapsedTime = 0;

        while (elapsedTime < timeToFly)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / timeToFly));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

       
        OnImpact(); // Destination reached 
    }

    IEnumerator SimulateParabola(Vector3 target)
    {
        float deltaY = target.y - transform.position.y;
        float deltaX = (target.x - transform.position.x) + Random.Range(-0.5f,0.5f); // Adding Random for some variation

        float maxHeight = deltaY + ARROW_MAX_HEIGHT + Random.Range(0f,5f); // Adding Random for some variation

        float time = Mathf.Sqrt(-2 * maxHeight / gravity) + Mathf.Sqrt(2 * (deltaY - maxHeight) / gravity);

        float Vy = Mathf.Sqrt(-2 * gravity * maxHeight);
        float Vx = deltaX / time;
        
        Vector3 velocity = new Vector3(Vx, Vy);
        float TimeElapsed = 0;

        // Reference to a Arrow Particle (Needed for rotation)
        GameObject childParticle = transform.GetChild(0).gameObject;

        while (TimeElapsed < time)
        {
            transform.Translate(velocity * Time.deltaTime); // Apply Movement
            velocity.y += gravity * Time.deltaTime; // Apply gravity

            // Rotate the arrowVFX
            float angle = Vector2.Angle(Vector2.right, velocity.normalized);
            Vector3 cross = Vector3.Cross(Vector2.right, velocity.normalized);
            if (cross.z < 0) angle = -angle;
            Vector3 eulers = childParticle.transform.rotation.eulerAngles;
            childParticle.transform.rotation = Quaternion.Euler(new Vector3(-angle, eulers.y, eulers.z));

            TimeElapsed += Time.deltaTime;

            yield return null;
        }

        // Destination reached 
        Instantiate(blastVFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }





    private void OnImpact()
    {
        Transform[] children = GetComponentsInChildren<Transform>();

        for (int i = 1; i < children.Length; i++)
        {
            children[i].parent = null;
            Destroy(children[i].gameObject, 0.25f);
        }

        Instantiate(blastVFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}