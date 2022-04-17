using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectile : MonoBehaviour
{

    private playerHP playerHp;
    // Start is called before the first frame update
    void Start()
    {
        playerHp = GameObject.Find("Player").GetComponent<playerHP>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerHp.takeDamage();
        }
    }
}
