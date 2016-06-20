using UnityEngine;
using System.Collections;

public class TankAI : MonoBehaviour {
    AILerp agent;
    FSM tankFSM;
    public float distance;

    //variabili di debug
    public bool gladiatorInRange;
    public bool shooting;
    public bool die;
    //tempo tra l'inizio di un attacco e l'inizio di un altro
    public float attackTime = 3f;
    public float normalSpeed;
    public float shieldSpeed;
    public bool debugMode;
    float timer;
    GameObject target;
    EnemyHealth health;
    Animator m_animator;

	// Use this for initialization
	void Start () {
        m_animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        agent = GetComponent<AILerp>();
        target = GameElements.getGladiator();
        if (target == null)
        {
            this.enabled = false;
            return;
        }
        agent.target = target.transform;
        
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
        chasing.AddTransition(new FSMTransition(ReceivedDamage, damaged));
        //shieldChasing
        shieldChasing.AddStayAction(ShieldChasing);
        shieldChasing.AddTransition(new FSMTransition(GladiatorInRange, attacking));
        shieldChasing.AddTransition(new FSMTransition(GladiatorStopShootingMe, chasing));
        shieldChasing.AddTransition(new FSMTransition(GoigToDie, dying));

        //attacking
        attacking.AddStayAction(Attacking);
        attacking.AddTransition(new FSMTransition(GladiatorOutOfRange, chasing));
        attacking.AddTransition(new FSMTransition(GoigToDie, dying));
        attacking.AddTransition(new FSMTransition(ReceivedDamage, damaged));

        //dying
        dying.AddEnterAction(Dying);


        //damage
        damaged.AddEnterAction(ReceiveDamage);
        damaged.AddTransition(new FSMTransition(DamageWait, chasing));
        tankFSM = new FSM(chasing);

	}
	
	// Update is called once per frame
	void Update () {
        tankFSM.Update();
	}

    bool GladiatorInRange() {
        distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= 2.5f)
        {
            return true;
        }
        return false;
    }


    bool GladiatorOutOfRange() {
        return !GladiatorInRange();
    }

    bool GladiatorShootingMe() {
        //TODO controlla se il gladiatore sta sparando. Come? Il gladiatore sta sparando e il tank è nel cono visivo
        return shooting;
    }

    bool GladiatorStopShootingMe() {
        return !GladiatorShootingMe();
    }

    bool GoigToDie() {
        if (health.getCurrentHealth() <= 0)
            return true;
        return false;
    }

    void Chasing() {
        DebugLine("Chasing");
        Chase(normalSpeed,false);
    }

    void ShieldChasing() {
        DebugLine("Shield chasing");
        Chase(shieldSpeed, true);
    }

    void Chase(float speed, bool shield) {
        agent.speed = speed;
        agent.canMove = true;
        m_animator.SetBool("Run", shield);
        m_animator.SetBool("Attack", false);
        
        

    }

    void ReceiveDamage()
    {

    }

    bool ReceivedDamage()
    {
        //TODO check if damaged
        return true;
    }

    bool DamageWait()
    {
        //TODO wait before acting after damage
        return true;
    }
    void Dying() {
        //TODO animazione dying e destroy del gameobject. Ricordati di mettere nell'OnDestroy l'eventuale rimozione dalla lista del player (se presente)
        DebugLine("Dead");
    }

    void Attacking() {
        Debug.Log("ATTACCO");
        agent.speed = 0f;
        m_animator.SetBool("Attack", true);
        agent.canMove =false;
        /*if (timer == 0) {
            
            
        }
        timer += Time.deltaTime;
        if (timer >= attackTime) {
            ResetTimer();
        }*/
    }

    void ResetTimer() {
        timer = 0;
    }

    void DebugLine(string text) {
       if(debugMode) Debug.Log(text);
    }
}
