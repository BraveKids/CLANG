using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class CrowdIA : NetworkBehaviour
{

    DecisionTree CrowdTree;
    public float helpFrequency = 3f;
    public float maxMedpackProbability = 0.4f;
    public float weaponProbability = 0.5f;
    public int monsterTrheshold = 6;
    public float miteProbability = 0.4f;
    public float lambdaArmor = .1f;
    public float arenaLR = 18f;
    public float arenaU = 5f;
    public float arenaD = 20f;
    private float arenaBorderL;
    private float arenaBorderR;
    private float arenaBorderU;
    private float arenaBorderD;
    private GameObject arena;

    //for debug
    public float armorProbability = 0f;
    public float strategistProbability = 0f;
    public float medpackProbability = 0f;

    public GameObject medPackPrefab;
    public GameObject gunPrefab;
    public GameObject grenadePrefab;
    public GameObject armorPrefab;
    public GameObject mitePrefab;

    public bool debugMode;

    public GameObject[] arenaElements;
    // Use this for initialization

    public int count;

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        arena = GameObject.FindGameObjectWithTag("Arena");
        arenaElements = GameObject.FindGameObjectsWithTag("ArenaElements");
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
        

        arenaBorderL = arena.transform.position.x - arenaLR;
        arenaBorderR = arena.transform.position.x + arenaLR;
        arenaBorderD = arena.transform.position.z + arenaU;
        arenaBorderU = arena.transform.position.z - arenaD;

    }

    // Update is called once per frame
    void Update()
    {
        float maxLife = GameElements.getMaxLife();
        float currentLife = GameElements.getGladiatorLife();
        strategistProbability = currentLife / maxLife;   //this goes from 0 to 1
        
        int monsterCount = GameElements.getEnemyCount();
        armorProbability = 1 - Mathf.Exp(-lambdaArmor * monsterCount);

        medpackProbability = maxMedpackProbability - (currentLife * maxMedpackProbability) / maxLife;

        count = GameElements.getEnemyCount();
    }








    /*
    *   the more the life is full the highest the probability to help the strategist
    *   Linear distribution
    */
    object StrategistDice()
    {
        float maxLife = GameElements.getMaxLife();
        float currentLife = GameElements.getGladiatorLife();
        strategistProbability = currentLife / maxLife;   //this goes from 0 to 1
        return Random.value <= strategistProbability ? "strategist" : "gladiator";
    }

    //Armor probability
    object ArmorDice()
    {
        if (GameElements.getArmorDropped() || GameElements.getGladiatorArmor() > 0)
            return false;
        int monsterCount = GameElements.getEnemyCount();
        armorProbability = 1 - Mathf.Exp(-lambdaArmor * monsterCount);
        return Random.value < armorProbability ? true : false;
    }

    //it goes to a minimum of 0% to a maximum of maxMedpackProbability
    object MedpackDice()
    {
        if (GameElements.getMedDropped())
            return false;
        float maxLife = GameElements.getMaxLife();
        float currentLife = GameElements.getGladiatorLife();
        medpackProbability = maxMedpackProbability - (currentLife * maxMedpackProbability) / maxLife;
        return Random.value < medpackProbability ? true : false;
    }

    //fixed probability
    object WeaponDice()
    {
        float halfMaxIntegrity = GameElements.getMaxIntegrity() * 0.5f;
        if (GameElements.getGladiator().GetComponent<GladiatorShooting>().grenadeTaken || GameElements.getWeaponDropped() || (GameElements.getIntegrity() > halfMaxIntegrity))
            return false;
        return Random.value < weaponProbability ? true : false;
    }

    //fixed probability
    object MiteDice()
    {
        if (GameElements.getEnemyCount() <= monsterTrheshold)
            return Random.value < miteProbability ? true : false;
        return false;
    }

    /*
    *
    *               ACTION
    *
    */

    void DropWeapon()
    {
        GameElements.setWeaponDropped(true);

        if (Random.value <= .5f)
            gameObject.GetComponent<StrategistSpawner>().Spawn(gunPrefab, itemSpawnPoint());

        else
            gameObject.GetComponent<StrategistSpawner>().Spawn(grenadePrefab, itemSpawnPoint());

        DebugLine("WEAPON");

    }

    void DropMite()
    {
        Vector3 correctSpawnPos = itemSpawnPoint();
        correctSpawnPos.y = 0.2f;
        gameObject.GetComponent<StrategistSpawner>().Spawn(mitePrefab, correctSpawnPos);


        DebugLine("MITE");

    }

    void DropMedpack()
    {
        gameObject.GetComponent<StrategistSpawner>().SetMedDropped();
        gameObject.GetComponent<StrategistSpawner>().Spawn(medPackPrefab, itemSpawnPoint());

        DebugLine("MEDPACK");

    }

    void DropArmor()
    {
        gameObject.GetComponent<StrategistSpawner>().SetArmorDropped();
        gameObject.GetComponent<StrategistSpawner>().Spawn(armorPrefab, itemSpawnPoint());

        DebugLine("ARMOR");

    }



    Vector3 itemSpawnPoint()
    {
        while (true)
        {
            bool isCorrect = true;
            Vector3 spawnPoint = RandomSpawnPoint();
            foreach (GameObject elem in arenaElements)
            {
                if (PointInOABB(spawnPoint, elem.GetComponent<BoxCollider>()))
                {
                    isCorrect = false;
                    break;
                }
            }
            if (isCorrect) return spawnPoint;
        }

    }

    Vector3 RandomSpawnPoint()
    {
        float spawnX = Random.Range(arenaBorderL, arenaBorderR);
        float spawnZ = Random.Range(arenaBorderU, arenaBorderD);
        Vector3 spawnPoint = new Vector3(spawnX, arena.transform.position.y + 1f, spawnZ);
        return spawnPoint;
    }

    bool PointInOABB(Vector3 point, BoxCollider box)
    {
        point = box.transform.InverseTransformPoint(point) - box.center;

        float halfX = (box.size.x * 0.5f);
        float halfY = (box.size.y * 0.5f);
        float halfZ = (box.size.z * 0.5f);
        if (point.x < halfX && point.x > -halfX &&
           point.y < halfY && point.y > -halfY &&
           point.z < halfZ && point.z > -halfZ)
            return true;
        else
            return false;
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

    void DebugLine(string text)
    {
        if (debugMode) Debug.Log(text);
    }

}
