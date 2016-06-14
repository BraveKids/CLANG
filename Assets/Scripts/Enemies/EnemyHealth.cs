using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class EnemyHealth : NetworkBehaviour
{
    GameObject GarbageCollector;
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
        GarbageCollector = GameObject.FindGameObjectWithTag("Garbage");
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

            Destroy(gameObject);
            //gameObject.SetActive(false);
            //gameObject.transform.parent = GarbageCollector.transform;
           



        }
    }

    void OnDestroy()
    {
        gladiatorScript.RemoveTarget(gameObject.transform);
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


    void OnChangeHealth(float amount)
    {
        currentHealth = amount;
        //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    void Death()
    {
        gladiatorScript.DestroyEnemy(gameObject);
    }
   
}
