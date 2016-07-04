using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MutantAI : MonoBehaviour
{

    public GameObject attackTrigger;
    FSM mutantFSM;
    GameObject target;
    EnemyHealth health;
    AILerp agent;
    public bool debugMode;
    public bool isDamaged;
    public float damageTime;
    public float attackTime;
    public bool inRange;
    public bool die;
    float damageTimer;
    float attackTimer;
    Animator anim;
    public float distance;
    NetworkAnimator netAnim;
    float timer;

    // Use this for initialization
    void Start()
    {

        target = GameElements.getGladiator();
        health = GetComponent<EnemyHealth>();
        agent = GetComponent<AILerp>();
        anim = GetComponent<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
        agent.target = target.transform;

        FSMState chasing = new FSMState();
        FSMState attacking = new FSMState();
        FSMState damaged = new FSMState();
        FSMState dying = new FSMState();

        chasing.AddStayAction(Chasing);
        chasing.AddEnterAction(ResetTimer);
        chasing.AddTransition(new FSMTransition(GladiatorInRange, attacking));
        chasing.AddTransition(new FSMTransition(IsDamaged, damaged));

        attacking.AddStayAction(Attacking);
        attacking.AddTransition(new FSMTransition(GladiatorOutOfRange, chasing));
        attacking.AddTransition(new FSMTransition(IsDamaged, damaged));

        damaged.AddEnterAction(GetDamage);
        damaged.AddTransition(new FSMTransition(PerkUp, chasing));
        damaged.AddTransition(new FSMTransition(GoigToDie, dying));

        dying.AddEnterAction(Die);

        mutantFSM = new FSM(chasing);





    }

    // Update is called once per frame
    void Update()
    {
        mutantFSM.Update();
    }





    bool GladiatorInRange()
    {
        distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= 2f)
        {
            return true;
        }
        return false;
    }

    bool GladiatorOutOfRange()
    {
        return !GladiatorInRange();
    }

    bool IsDamaged()
    {
        return isDamaged;
    }

    bool GoigToDie()
    {
        if (health.getCurrentHealth() <= 0)
            return true;
        return false;


    }

    void Chasing()
    {
        DebugLine("Chasing");
        anim.SetBool("Attack", false);
        agent.speed = 5f;
        agent.canMove = true;

    }

    void Attacking()
    {
        transform.LookAt(target.transform);
        anim.SetBool("Attack", true);
        agent.speed = 0f;
        agent.canMove = false;
        if (timer == 0)
        {

            Invoke("AttackUp", 0.7f);
            DebugLine("Attack " + Random.value);
        }

        timer += Time.deltaTime;
        if (timer >= attackTime)
        {
            ResetTimer();
        }
    }


    void AttackUp()
    {
        attackTrigger.GetComponent<BoxCollider>().enabled = true;
        Invoke("AttackDown", 0.2f);
    }

    void AttackDown()
    {
        attackTrigger.GetComponent<BoxCollider>().enabled = false;
    }

    void GetDamage()
    {

        attackTrigger.GetComponent<BoxCollider>().enabled = false;
        agent.speed = 0f;
        agent.canMove = false;
        anim.SetBool("Attack", false);
        //anim.SetTrigger("Damage");
        netAnim.SetTrigger("Damage");
        isDamaged = false;
        DebugLine("damage");
    }

    void DebugLine(string text)
    {
        if (debugMode) Debug.Log(text);
    }

    bool PerkUp()
    {
        damageTimer += Time.deltaTime;
        if (damageTimer >= damageTime)
        {
            damageTimer = 0;

            return true;
        }
        return false;
    }

    void Die()
    {
        agent.speed = 0f;
        agent.canMove = false;
        //TODO animazione dying e destroy del gameobject. Ricordati di mettere nell'OnDestroy l'eventuale rimozione dalla lista del player (se presente)
        anim.SetTrigger("Death");
        netAnim.SetTrigger("Death");
        Invoke("Destroy", 2f);
        DebugLine("Dead");

    }

    void ResetTimer()
    {
        //agent.target = target.transform;
        timer = 0;
    }

    void OnDestroy()
    {
        target.GetComponent<GladiatorShooting>().RemoveTarget(gameObject.transform);
    }

    void Destroy()
    {
        target.GetComponent<GladiatorShooting>().DestroyEnemy(gameObject);
    }



}
