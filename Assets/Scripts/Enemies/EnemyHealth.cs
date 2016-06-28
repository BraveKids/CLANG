using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
public class EnemyHealth : NetworkBehaviour
{
    public Image healthBar;
    public float m_Health;
    public float m_Resistance;
    public GameObject model;
    public GameObject[] wurmModel;
    public bool destroyOnDeath;
    Color curColor;
    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth;
    GladiatorShooting gladiatorScript;
    MutantAI mutantScript;
    TankAI tankScript;
    void Start()
    {
        mutantScript = GetComponent<MutantAI>();
        tankScript = GetComponent<TankAI>();
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
            mutantScript.isDamaged = true;
        }
        else if (tankScript != null)
        {
            tankScript.isDamaged = true;
        }

        float calculatedDamage = amount - m_Resistance * 0.3f;

        currentHealth -= calculatedDamage;
        DamageColor();

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
        SetHealthUI();
    }


    private void SetHealthUI()
    {

        healthBar.fillAmount = mapValueTo01(currentHealth, 0f, m_Health);
    }

    public static float mapValueTo01(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }



}
