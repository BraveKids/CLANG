using UnityEngine;
using System.Collections;

public class CrowdIA : MonoBehaviour {

    DecisionTree CrowdTree;
    public float helpFrequency = 3f;
    public float armorProbability = 0.3f;
    public float maxMedpackProbability = 0.5f;
    public float maxArmorProbability = 0.4f;
    public float weaponProbability = 0.4f;
    public int monsterTrheshold;
    // Use this for initialization
    void Start() {
        DTDecision kingMaker = new DTDecision(StrategistDice);
        DTDecision miteNode = new DTDecision(MiteDice);
        DTDecision weaponNode = new DTDecision(WeaponDice);
        DTDecision armorNode = new DTDecision(ArmorDice);
        DTDecision medpackNode = new DTDecision(MedpackDice);

        DTAction miteAction = new DTAction(DropMite);
        DTAction medpackAction = new DTAction(DropMedpack);
        DTAction armorAction = new DTAction(DropArmor);
        DTAction weaponAction = new DTAction(DropWeapon);

        kingMaker.AddNode("strategist", miteNode);
        miteNode.AddNode(true, miteAction);
        kingMaker.AddNode("gladiator", weaponNode);
        weaponNode.AddNode(true, weaponAction);
        weaponNode.AddNode(false, armorNode);
        armorNode.AddNode(true, armorAction);
        armorNode.AddNode(false, medpackNode);
        medpackNode.AddNode(true, medpackAction);


        CrowdTree = new DecisionTree(kingMaker);
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
    object StrategistDice() {
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

    //it goes to a minimum of 0% to a maximum of maxMedpackProbability
    object MedpackDice() {
        if (GameElements.getMedDropped())
            return false;
        float maxLife = GameElements.getMaxLife();
        float currentLife = GameElements.getGladiatorLife();
        float currentProbability = 1 - ((currentLife * maxMedpackProbability) / maxLife);
        return Random.value > currentProbability ? true : false;
    }

    //fixed probability
    object WeaponDice() {
        float halfMaxIntegrity = GameElements.getMaxIntegrity() / 2;
        if ((GameElements.getIntegrity() >= halfMaxIntegrity) || GameElements.getWeaponDropped())
            return false;
        return Random.value < weaponProbability ? true : false;
    }
    //not a real dice but...
    object MiteDice() {
        if (GameElements.getEnemyCount() >= monsterTrheshold)
            return true;
        return false;
    }

    /*
    *
    *               ACTION
    */

    void DropWeapon() {
        Debug.Log("WEAPON");
    }

    void DropMite() {
        Debug.Log("MITE");
    }

    void DropMedpack() {
        Debug.Log("MEDPACK");
    }

    void DropArmor() {
        Debug.Log("ARMOR");
    }

    IEnumerator Patrol() {
        while (true) {
            CrowdTree.Walk();
            yield return new WaitForSeconds(helpFrequency);
        }
    }

}
