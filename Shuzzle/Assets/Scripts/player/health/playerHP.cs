using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerHP : MonoBehaviour
{
    [Header("Health")] 
    public float health;
    public float damagedTimer;

    [Header("References")] 
    public playerCamera cam;
    public Image healthbar;
    public Button restartBTN;
    public TextMeshProUGUI deathMessage;
    public TextMeshProUGUI interaction;
    private RectTransform hpBar;
    private List<float> healthBarToRestore = new List<float>();
    
    // Start is called before the first frame update
    void Start()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        hpBar = healthbar.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void takeDamage()
    {
        health -= 20;
        if (health == 0)
        {
            healthbar.GetComponent<RectTransform>().gameObject.SetActive(false);
            deathMessage.gameObject.SetActive(true);
            restartBTN.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else
        {
            healthbar.GetComponent<RectTransform>().localScale =
                new Vector3(hpBar.localScale.x - 0.20f, hpBar.localScale.y, hpBar.localScale.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Debug.Log("Player has been hit: " + health);
            takeDamage();
            Destroy(collision.gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "health_pickup")
        {
            healHP(other.gameObject.GetComponent<health_pickup>().health_per_pickup, other.gameObject);
            Debug.Log("Player has picked up " + other.gameObject.name);
        }
    }

    private void healHP(float hpToRestore, GameObject other)
    {
        float timer = 5f;
        if (health < 100)
        {
            health += hpToRestore;
            healthbar.GetComponent<RectTransform>().localScale =
                new Vector3(hpBar.localScale.x + 0.20f, hpBar.localScale.y, hpBar.localScale.z);
            Destroy(other);
            while (timer > 0f)
            {
                interaction.text = "You picked up " + other.name + " Your health is now " + health;
                timer -= Time.deltaTime;
            }
        }

        if (health >= 100)
        {
            health = 100; 
            Debug.Log("Health is full already. Didn't pick up " + other.name);
            while (timer > 0f)
            {
                interaction.text = "Your health is full";
                timer -= Time.deltaTime;
            }
        }
    }
}
