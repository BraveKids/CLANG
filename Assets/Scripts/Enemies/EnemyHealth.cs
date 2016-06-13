using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class EnemyHealth : NetworkBehaviour
{
    public float m_Health;
    public float m_Resistance;
    public GameObject model;
    public bool destroyOnDeath;
    //Color curColor;
    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth;
    GladiatorShooting gladiatorScript;
    void Start()
    {
        gladiatorScript = GameElements.getGladiator().GetComponent<GladiatorShooting>();
        //curColor = model.GetComponent<SkinnedMeshRenderer>().material.color;
        currentHealth = m_Health;
    }


    public void Damage(float amount)
    {

        float calculatedDamage = amount - m_Resistance * 0.3f;

        currentHealth -= calculatedDamage;
        DamageColor();
        if (currentHealth <= 0)
        {

            //gladiatorScript.RemoveTarget(gameObject);

            gladiatorScript.DestroyEnemy(gameObject);



        }
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public float getMaxHealth()
    {
        return m_Health;
    }

    private void DamageColor()
    {

        //model.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        Invoke("DamageColorBack", 0.5f);

    }

    private void DamageColorBack()
    {
        //model.GetComponent<SkinnedMeshRenderer>().material.color = curColor;
    }


    void OnChangeHealth(float currentHealth)
    {
        //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

   
}
