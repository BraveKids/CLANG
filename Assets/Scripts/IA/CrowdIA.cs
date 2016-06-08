using UnityEngine;
using System.Collections;

public class CrowdIA : MonoBehaviour {

    public float kingMaker;             //higher means more probability to help the gladiator
    DecisionTree CrowdTree;
    public float helpFrequency = 3f;
    public float armorProbability = 0.3f;
    public float maxMedpackProbability = 0.5f;
    public float weaponProbability = 0.4f;
    // Use this for initialization
    void Start() {
        DTDecision d1 = new DTDecision(randomNumber);


        DTAction a1 = new DTAction(trueOption);
        DTAction a2 = new DTAction(falseOption);
        d1.AddNode(true, a1);
        d1.AddNode(false, a2);


        CrowdTree = new DecisionTree(d1);
        StartCoroutine(Patrol());

    }

    // Update is called once per frame
    void Update() {

    }

    object randomNumber() {
        return Random.value > 0.5 ? true : false;
    }

    void trueOption() {
        Debug.Log("Opzione true");
    }

    void falseOption() {
        Debug.Log("Opzione false");
    }

    /*
    *   the more the life is full the highest the probability to help the strategist
    *   Linear distribution
    */
    object strategistDice() {
        float maxLife = GameElements.getMaxLife();
        float currentLife = GameElements.getGladiatorLife();
        float normalizedLife = currentLife / maxLife;   //this goes from 0 to 1
        return Random.value < normalizedLife ? "strategist" : "gladiator";
    }

    //fixed probability
    object ArmorDice() {
        if (GameElements.getArmorDropped() || GameElements.getGladiatorArmor() > 0)
            return false;
        return Random.value < armorProbability ? true : false;
    }

    object LifeDice() {
        if (GameElements.getMedDropped())
            return false;
        float maxLife = GameElements.getMaxLife();
        float currentLife = GameElements.getGladiatorLife();
        float currentProbability = 1 - ((currentLife * maxMedpackProbability) / maxLife);
        return Random.value > currentProbability ? true : false;
    }

    object weaponDice() {
        float halfMaxIntegrity = GameElements.getMaxIntegrity() / 2;
        if ((GameElements.getIntegrity() >= halfMaxIntegrity) || GameElements.getWeaponDropped())
            return false;

        return Random.value < weaponProbability ? true : false;


    }

    IEnumerator Patrol() {
        while (true) {
            CrowdTree.Walk();
            yield return new WaitForSeconds(helpFrequency);
        }
    }


}
