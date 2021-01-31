using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    private static GameMode _instance;
    public static GameMode Instance
    {
        get
        {
            return _instance;
        }
    }

    private List<GameObject> CharactersInBattle;
    
    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
        CharactersInBattle = new List<GameObject>();
    }

    public void AddCharacterToBattle(GameObject obj)
    {
        CharactersInBattle.Add(obj);
    }

    public void RemoveCharacterFromBattle(GameObject obj)
    {
        CharactersInBattle.Remove(obj);
    }

    public  GameObject[] GetAllCharactersInBattle()
    {
        return CharactersInBattle.ToArray();
    }

    public GameObject[] GetEnemiesInBattle()
    {
        List<GameObject> enemies = new List<GameObject>();
        foreach(var enemy in CharactersInBattle)
        {
            if(enemy.layer == LayerMask.NameToLayer("Enemy"))
            {
                enemies.Add(enemy);
            }
        }
        return enemies.ToArray();
    }

}
