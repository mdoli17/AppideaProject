using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(RegisterToBattle());
    }

    IEnumerator RegisterToBattle()
    {
        yield return new WaitForSeconds(0.1f);
        GameMode.Instance.AddCharacterToBattle(this.gameObject);
    }
}
