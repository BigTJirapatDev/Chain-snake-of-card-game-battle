using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_system : MonoBehaviour
{
    //Enemy status value.
    public Sprite EnemySprite;
    public float Speed;
    public int EnemySword;
    public int EnemyShield;
    public int EnemyHeart;
    public int EnemyType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Enemy destroy when lose battle.
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
