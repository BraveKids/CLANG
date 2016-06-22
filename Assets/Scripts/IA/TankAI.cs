using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class TankAI : NetworkBehaviour
{
    public GameObject attackTrigger;
    GladiatorShooting gladShot;
    public float damageTaken;
    Animator anim;
    NetworkAnimator netAnim;
    public float distance;
    FSM tankFSM;
    AILerp agent;
    //variabili di debug
    public bool gladiatorInRange;
    public bool shooting;
    public bool die;
    public float damageTime;
    float damageTimer = 0f;
    //tempo tra l'inizio di un attacco e l'inizio di un altro
    public float attackTime = 3f;
    public bool isDamaged;
    public float normalSpeed;
    public float shieldSpeed;
    public bool debugMode;
    float timer;
    GameObject target;
    EnemyHealth health;
    public float afterShield;
    float shieldTimer = 0f;


    // Use this for initialization
    void Start()
    {
        netAnim = GetComponent<NetworkAnimator>();
        anim = GetComponent<Animator>();
        target = GameElements.getGladiator();
        gladShot = target.GetComponent<GladiatorShooting>();
        health = GetComponent<EnemyHealth>();
        agent = GetComponent<AILerp>();
        FSMState chasing = new FSMState();
        FSMState shieldChasing = new FSMState();
        FSMState attacking = new FSMState();
        FSMState dying = new FSMState();
        FSMState damaged = new FSMState();

        //chasing
        chasing.AddStayAction(Chasing);
        chasing.AddEnterAction(ResetTimer);
        chasing.AddTransition(new FSMTransition(GladiatorShootingMe, shieldChasing));
        chasing.AddTransition(new FSMTransition(GladiatorInRange, attacking));
        chasing.AddTransition(new FSMTransition(GoigToDie, dying));
        chasing.AddTransition(new FSMTransition(IsDamaged, damaged));


        //shieldChasing
        shieldChasing.AddStayAction(ShieldChasing);
        shieldChasing.AddTransition(new FSMTransition(GladiatorInRange, attacking));
        shieldChasing.AddTransition(new FSMTransition(GladiatorStopShootingMe, chasing));
        shieldChasing.AddTransition(new FSMTransition(GoigToDie, dying));

        //attacking
        attacking.AddStayAction(Attacking);
        attacking.AddTransition(new FSMTransition(GladiatorOutOfRange, chasing));
        attacking.AddTransition(new FSMTransition(GoigToDie, dying));
        attacking.AddTransition(new FSMTransition(IsDamaged, damaged));


        //damged
        damaged.AddEnterAction(GetDamage);
        damaged.AddTransition(new FSMTransition(PerkUp, chasing));

        //dying
        dying.AddEnterAction(Dying);

        tankFSM = new FSM(chasing);

    }

    // Update is called once per frame
    void Update()
    {

        tankFSM.Update();
    }

    bool GladiatorInRange()
    {
        distance = Vector3.Distance(transform.position, target.transform.position);
        if(distance<= 2.5f)
        {
            return true;
        }
        //TOTO controlla se il gladiatore è nel raggio d'azione del tank
        return false;
    }

    bool GladiatorOutOfRange()
    {
        return !GladiatorInRange();
    }

    bool GladiatorShootingMe()
    {
        if(gladShot.specialAttack )
        {
            shieldTimer = 0f;
            return true;
        }
        //TODO controlla se il gladiatore sta sparando. Come? Il gladiatore sta sparando e il tank è nel cono visivo
        return false;
    }

    bool GladiatorStopShootingMe()
    {
        shieldTimer += Time.deltaTime;
        if(shieldTimer >= afterShield)
        {
            return true;
        }
        if (gladShot.specialAttack)
            shieldTimer = 0f;
        return false;
        
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
        health.m_Resistance = 9f;
        Chase(normalSpeed, false);
    }

    void ShieldChasing()
    {
        isDamaged = false;
        health.m_Resistance = 11f;
        DebugLine("Shield chasing");
        Chase(shieldSpeed, true);
    }

    void Chase(float speed, bool charge)
    {
        
        anim.SetBool("Run", charge);
        anim.SetBool("Attack", false);
        agent.canMove = true;
        agent.speed = speed;

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

    void Dying()
    {
        agent.speed = 0f;
        agent.canMove = false;
        //TODO animazione dying e destroy del gameobject. Ricordati di mettere nell'OnDestroy l'eventuale rimozione dalla lista del player (se presente)
        DebugLine("Dead");
        anim.SetTrigger("Death");
        netAnim.SetTrigger("Death");
        Invoke("Destroy", 2f);
    }

    void OnDestroy()
    {
        target.GetComponent<GladiatorShooting>().RemoveTarget(gameObject.transform);
    }

    void Destroy()
    {
        target.GetComponent<GladiatorShooting>().DestroyEnemy(gameObject);
    }

    void Attacking()
    {
        anim.SetBool("Attack", true);
        agent.speed = 0f;
        agent.canMove = false;
        if (timer == 0)
        {

            Invoke("Attack", 0.75f);
            DebugLine("Attack " + Random.value);
        }

        timer += Time.deltaTime;
        if (timer >= attackTime)
        {
            ResetTimer();
        }
    }

    void Attack()
    {
        attackTrigger.GetComponent<BoxCollider>().enabled = true;
        Invoke("AttackDown", 0.2f);
    }

    void AttackDown()
    {
        attackTrigger.GetComponent<BoxCollider>().enabled = false;
    }

    void ResetTimer()
    {
        agent.target = target.transform;
        timer = 0;
    }

    void GetDamage()
    {

        //anim.SetTrigger("Damage");
        anim.SetBool("Attack", false);
        anim.SetBool("Run", false);
        netAnim.SetTrigger("Damage");
        isDamaged = false;
        DebugLine("damage");
    }

    bool IsDamaged()
    {
        return isDamaged;
    }

    void DebugLine(string text)
    {
        if (debugMode) Debug.Log(text);
    }
}