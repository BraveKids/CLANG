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
    Color curColor;
    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth;
    GladiatorShooting gladiatorScript;
    MutantScriptTest mutantScript;
    void Start()
    {
        mutantScript = GetComponent<MutantScriptTest>();
       
        gladiatorScript = GameElements.getGladiator().GetComponent<GladiatorShooting>();
        if (model.GetComponent<SkinnedMeshRenderer>() != null)
        {
            curColor = model.GetComponent<SkinnedMeshRenderer>().material.color;
        }
        else
        {
            curColor = model.GetComponent<MeshRenderer>().material.color;
        }
        currentHealth = m_Health;
    }


    public void Damage(float amount)
    {
        if (mutantScript != null)
        {
            mutantScript.Damage();
        }
        float calculatedDamage = amount - m_Resistance * 0.3f;

        currentHealth -= calculatedDamage;
        DamageColor();
        if (currentHealth <= 0)
        {
            if (mutantScript != null)
            {
                mutantScript.Death();
                Invoke("Death", 1.8f);
            }
            else
            {
                Death();
            }

            
            //gameObject.SetActive(false);
            //gameObject.transform.parent = GarbageCollector.transform;
           



        }
    }

    void Death()
    {
        gladiatorScript.DestroyEnemy(gameObject);
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
        if (model.GetComponent<SkinnedMeshRenderer>() != null)
        {
           model.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        }
        else
        {
            model.GetComponent<MeshRenderer>().material.color= Color.red;
        }

       
        Invoke("DamageColorBack", 0.5f);

    }

    private void DamageColorBack()
    {
        if (model.GetComponent<SkinnedMeshRenderer>() != null)
        {
            model.GetComponent<SkinnedMeshRenderer>().material.color = curColor;
        }
        else
        {
            model.GetComponent<MeshRenderer>().material.color = curColor;
        }
    }


    void OnChangeHealth(float amount)
    {
        currentHealth = amount;
        //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

   

}
