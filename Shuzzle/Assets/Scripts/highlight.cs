using System;
using TMPro;
using UnityEngine;

public class highlight : MonoBehaviour
{

    [Header("References")] 
    public TextMeshProUGUI interaction;
    public PickUpGun gun;
    public Grenade grenade;
    public Lootables loot;
    private Renderer rend;
    private Color originalColor;
    private bool noRend;
    
    private void Start()
    {
        grenade = GameObject.Find("Player").GetComponent<Grenade>();
        
        if (transform.CompareTag("loot_kunai"))
        {
            noRend = true;
        }
        else
        {
            noRend = false;
            rend = GetComponent<Renderer>();
            originalColor = rend.material.color;
        }
    }

    private void OnMouseOver()
    {
        if (transform.CompareTag("loot_grenade") && loot.WithinRange() &&grenade.IsKunaiEquipped())
        {
            interaction.text = "Press E to pickup Grenade";
        }
        else if (transform.CompareTag("loot_kunai") && loot.WithinRange() && grenade.IsGrenadeEquipped())
        {
            interaction.text = "Press E to pickup Kunai";
        }
        else if(transform.CompareTag("weapon") && gun.WithinRange())
        {
            interaction.text = "Press E to pickup " + transform.name;
            rend.material.color = Color.yellow;
        }
        else
        {
            interaction.text = "";
        }
    }

    private void OnMouseExit()
    {
        if (!noRend && !transform.CompareTag("loot_grenade"))
        {
            rend.material.color = originalColor;
        }
        interaction.text = "";
        // Debug.Log("Mouse just left " + transform.name);
    }
}
