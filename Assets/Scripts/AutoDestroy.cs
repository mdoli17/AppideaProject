using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private ParticleSystem ps;
    [SerializeField]
    bool isChild;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(ps)
            if (!ps.IsAlive())
                Destroy(isChild ? transform.parent.gameObject : this.gameObject);
    }
}
