using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class EnemyHealth : NetworkBehaviour
{
    public float m_Health;
    public float m_Resistance;
    public GameObject model;
    public GameObject[] wurmModel;
    public bool destroyOnDeath;
    Color curColor;
    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth;
    GladiatorShooting gladiatorScript;
    MutantScriptTest mutantScript;
    TankScript tankScript;
    void Start()
    {
        mutantScript = GetComponent<MutantScriptTest>();
        tankScript = GetComponent<TankScript>();
        gladiatorScript = GameElements.getGladiator().GetComponent<GladiatorShooting>();
        if (gameObject.tag == "WurmCore")
        {
            curColor = wurmModel[0].GetComponent<MeshRenderer>().material.color;
            
        }
        else
        {
            curColor = model.GetComponent<SkinnedMeshRenderer>().material.color;
        }
        currentHealth = m_Health;
    }


    public void Damage(float amount)
    {
        if (mutantScript != null)
        {
            mutantScript.Damage();
        }
        else if (tankScript != null)
        {
            tankScript.Damage();
        }
    
        float calculatedDamage = amount - m_Resistance * 0.3f;

        currentHealth -= calculatedDamage;
        DamageColor();
        if (currentHealth <= 0 /*&& gameObject.tag != "Tank"*/)
        {
            if (mutantScript != null)
            {
                mutantScript.Death();
                Invoke("Death", 2f);
            }else if(tankScript != null)
            {
                tankScript.Death();
                Invoke("Death", 3f);
            }
            else
            {
                Death();
            }
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
        if (gameObject.tag == "WurmCore")
        {
            foreach (GameObject model in wurmModel)
            {
                model.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            
        }
        else
        {
            model.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;

        }

       
        Invoke("DamageColorBack", 0.3f);

    }

    private void DamageColorBack()
    {
        if (gameObject.tag == "WurmCore")
        {
            foreach (GameObject model in wurmModel)
            {
                model.GetComponent<MeshRenderer>().material.color = curColor;
            }
            
        }
        else
        {
            model.GetComponent<SkinnedMeshRenderer>().material.color = curColor;
        }
    }

    
    void OnChangeHealth(float amount)
    {
        currentHealth = amount;
        //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

   

}
