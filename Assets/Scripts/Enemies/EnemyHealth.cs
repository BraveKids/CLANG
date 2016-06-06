using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class EnemyHealth : NetworkBehaviour {
    public float m_Health;
    public float m_Resistance;

    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth;

    void Start()
    {
        currentHealth = m_Health;
    }
    

    public void Damage(float amount)
    {

        float calculatedDamage = amount - m_Resistance * 0.3f;
        
        currentHealth -= calculatedDamage;
        if (currentHealth <= 0)
        {
            GameElements.getGladiator().GetComponent<GladiatorShooting>().CmdDestroyEnemy(gameObject);

        }
    }


    void OnChangeHealth(float currentHealth)
    {
        //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

}
