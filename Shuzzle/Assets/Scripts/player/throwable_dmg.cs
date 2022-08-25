using UnityEngine;

public class throwable_dmg : MonoBehaviour
{
    [Header("Knife Stats")]
    public bool isKunai;
    public int kunaiDamage;

    [Header("Grenade Stats")]
    public bool isGrenade;
    public int grenadeDamage;
    public float blastRadius;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("enemy"))
        {
            Debug.Log("not an enemy ");
            return;
        }
        
        if (isKunai)
        {
            collision.transform.GetComponent<enemyAI>().TakeDamage(kunaiDamage);
            Destroy(gameObject);
            Debug.Log(collision.transform.GetComponent<enemyAI>().health);
        }else if (isGrenade)
        {
            collision.transform.GetComponent<enemyAI>().TakeDamage(grenadeDamage);
            Destroy(gameObject);
            Debug.Log(collision.transform.GetComponent<enemyAI>().health);
        }
    }
}
