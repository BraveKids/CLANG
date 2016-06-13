using UnityEngine;
using System.Collections;

public class CrowdIA : MonoBehaviour
{

    DecisionTree CrowdTree;
    public float helpFrequency = 3f;
    public float armorProbability = 0.3f;
    public float maxMedpackProbability = 0.5f;
    public float maxArmorProbability = 0.4f;
    public float weaponProbability = 0.4f;
    public int monsterTrheshold;
    public float arenaLR = 18f;
    public float arenaU = 5f;
    public float arenaD = 20f;
    private float arenaBorderL;
    private float arenaBorderR;
    private float arenaBorderU;
    private float arenaBorderD;
    private GameObject arena;

    public GameObject medPackPrefab;
    public GameObject weaponPrefab;
    public GameObject armorPrefab;
    // Use this for initialization
    void Start()
    {
        arena = GameElements.getArena();
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
        //DebugLine();

        arenaBorderL = arena.transform.position.x - arenaLR;
        arenaBorderR = arena.transform.position.x + arenaLR;
        arenaBorderD = arena.transform.position.z + arenaU;
        arenaBorderU = arena.transform.position.z - arenaD;

    }

    // Update is called once per frame
    void Update()
    {

        //DebugLine();
    }



   




    /*
    *   the more the life is full the highest the probability to help the strategist
    *   Linear distribution
    */
    object StrategistDice()
    {
        float maxLife = GameElements.getMaxLife();
        float currentLife = GameElements.getGladiatorLife();
        float normalizedLife = currentLife / maxLife;   //this goes from 0 to 1
        return Random.value < normalizedLife ? "strategist" : "gladiator";
    }

    //fixed probability
    object ArmorDice()
    {
        if (GameElements.getArmorDropped() || GameElements.getGladiatorArmor() > 0)
            return false;
        return Random.value < armorProbability ? true : false;
    }

    //it goes to a minimum of 0% to a maximum of maxMedpackProbability
    object MedpackDice()
    {
        if (GameElements.getMedDropped())
            return false;
        float maxLife = GameElements.getMaxLife();
        float currentLife = GameElements.getGladiatorLife();
        float currentProbability = 1 - ((currentLife * maxMedpackProbability) / maxLife);
        return Random.value > currentProbability ? true : false;
    }

    //fixed probability
    object WeaponDice()
    {
        float halfMaxIntegrity = GameElements.getMaxIntegrity() * 0.5f;
        if ((GameElements.getIntegrity() > halfMaxIntegrity) || GameElements.getWeaponDropped())
            return false;
        return Random.value < weaponProbability ? true : false;
    }

    //not a real dice but...
    object MiteDice()
    {
        if (GameElements.getEnemyCount() <= monsterTrheshold)
            return true;
        return false;
    }

    /*
    *
    *               ACTION
    *
    */

    void DropWeapon()
    {
        gameObject.GetComponent<StrategistSpawner>().Spawn(weaponPrefab, itemSpawnPoint());
        GameElements.setWeaponDropped(true);
        //Debug.Log("WEAPON");
    }

    void DropMite()
    {

        //Debug.Log("MITE");

    }

    void DropMedpack()
    {
        gameObject.GetComponent<StrategistSpawner>().Spawn(medPackPrefab, itemSpawnPoint());
        gameObject.GetComponent<StrategistSpawner>().SetMedDropped();
        //Debug.Log("MEDPACK");
    }

    void DropArmor()
    {
        gameObject.GetComponent<StrategistSpawner>().Spawn(armorPrefab, itemSpawnPoint());
        gameObject.GetComponent<StrategistSpawner>().SetArmorDropped();
        //Debug.Log("ARMOR");
    }



    Vector3 itemSpawnPoint()
    {
        float spawnX = Random.Range(arenaBorderL, arenaBorderR);
        float spawnZ = Random.Range(arenaBorderU, arenaBorderD);
        return new Vector3(spawnX, arena.transform.position.y + 1f, spawnZ);
    }

    void DebugLine()
    {
        arenaBorderL = arena.transform.position.x - arenaLR;
        arenaBorderR = arena.transform.position.x + arenaLR;
        arenaBorderD = arena.transform.position.z + arenaU;
        arenaBorderU = arena.transform.position.z - arenaD;
        float arenaY = arena.transform.position.y;
        float arenaZ = arena.transform.position.z;
        float arenaX = arena.transform.position.x;

        Debug.DrawLine(new Vector3(arenaBorderL, arenaY, arenaZ - 30f), new Vector3(arenaBorderL, arenaY, arenaZ + 30f), Color.magenta, 2f, false);
        Debug.DrawLine(new Vector3(arenaBorderR, arenaY, arenaZ - 30f), new Vector3(arenaBorderR, arenaY, arenaZ + 30f), Color.magenta, 2f, false);
        Debug.DrawLine(new Vector3(arenaX - 30f, arenaY, arenaBorderU), new Vector3(arenaX + 30f, arenaY, arenaBorderU), Color.magenta, 2f, false);
        Debug.DrawLine(new Vector3(arenaX - 30f, arenaY, arenaBorderD), new Vector3(arenaX + 30f, arenaY, arenaBorderD), Color.magenta, 2f, false);
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            CrowdTree.Walk();
            yield return new WaitForSeconds(helpFrequency);
        }
    }

}
