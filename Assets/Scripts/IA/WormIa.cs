using UnityEngine;
using System.Collections;

public class WormIA : MonoBehaviour {
    public float normalSpeed;
    public float boostSpeed;
    float timer = 0f;
    public GameObject target;
    public float itemEatingProbability;
    public float bareTime = 5f;
    public WurmTrigger trigger;
    public WurmAttack attackScript;
    public EnemyHealth myHealth;
    public float timeBeforeAttack = 1f;
    public float timeBeforChasing = 2f;
    float timerChase = 0f;
    float timerAttack = 0f;
    bool collided = false;
    FSM myIa;
    //EnemyHealth myHealth;
    Animator anim;
    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
        //myHealth = gameObject.GetComponent<EnemyHealth>();
        FSMState off = new FSMState();
        FSMState on = new FSMState();



        FSMState hide = new FSMState();
        FSMState chasing = new FSMState();
        FSMState attacking = new FSMState();
        FSMState angryChasing = new FSMState();
        FSMState angryAttacking = new FSMState();
        FSMState dying = new FSMState();
        FSMState waiting = new FSMState();
        FSMState angryWaiting = new FSMState();
       

        //hide
        hide.AddEnterAction(delegate () { target = GameElements.getGladiator(); });
        hide.AddTransition(new FSMTransition(StayNormal, chasing));
        hide.AddTransition(new FSMTransition(BeAggressive, angryChasing));

        //angry chasing
        angryChasing.AddEnterAction(BecomeAggressive);
        angryChasing.AddEnterAction(FindTarget);
        angryChasing.AddStayAction(AngryChase);
        angryChasing.AddTransition(new FSMTransition(CheckCollision, angryWaiting));

        //angryWaiting
        angryWaiting.AddTransition(new FSMTransition(GoToAttack, angryAttacking));

        //angryAttacking
        angryAttacking.AddEnterAction(Attack);
        angryAttacking.AddEnterAction(ResetTimer);
        angryAttacking.AddTransition(new FSMTransition(BareTime, hide));
        angryAttacking.AddTransition(new FSMTransition(LifeOver, dying));
        angryAttacking.AddExitAction(Hide);

        //chasing
        chasing.AddStayAction(Chase);
        chasing.AddTransition(new FSMTransition(CheckCollision, waiting));

        //waiting
        waiting.AddTransition(new FSMTransition(GoToAttack, attacking));

        //attacking
        attacking.AddEnterAction(Attack);
        attacking.AddEnterAction(ResetTimer);
        attacking.AddTransition(new FSMTransition(BareTime, hide));
        attacking.AddTransition(new FSMTransition(LifeOver, dying));
        attacking.AddExitAction(Hide);

        //dying
        dying.AddEnterAction(Die);





        myIa = new FSM(chasing);
        target = GameElements.getGladiator();
        //StartCoroutine(Patrol());

    }

    // Update is called once per frame
    void Update() {
        myIa.Update();
    }

    bool StayNormal() {
        timerChase += Time.deltaTime;
        if (timerChase >= timeBeforChasing)
        {
            if (myHealth.getCurrentHealth() > myHealth.getMaxHealth() / 2)
            {
                timerChase = 0f;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    bool BeAggressive() {
        timerChase += Time.deltaTime;
        if (timerChase >= timeBeforChasing)
        {
            if (myHealth.getCurrentHealth() <= myHealth.getMaxHealth() / 2)
            {
                timerChase = 0f;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    void BecomeAggressive() {
        //TODO
        //check if aggressive color is set
        //eventually change it
        FindTarget();
    }

    bool LifeOver() {
        return myHealth.getCurrentHealth() <= 0 ? true : false;
    }

    void FindTarget() {
        if (Random.value <= itemEatingProbability && GameElements.itemSpawned.Count > 0) {
            target = ChooseItem();
        }
        else {
            target = GameElements.getGladiator();
        }
    }

    GameObject ChooseItem() {
        int n = GameElements.itemSpawned.Count;
        int chosen = Random.Range(0, n - 1);
        return GameElements.itemSpawned[chosen];
    }

    void AngryChase() {
        Movement(boostSpeed);
    }

    void Chase() {
        Movement(normalSpeed);
    }

    void Movement(float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.transform.position.x-5f,
                                                                                transform.position.y,
                                                                                target.transform.position.z)
                                                                                , speed* Time.deltaTime);
    }
    bool GoToAttack() {
        timerAttack += Time.deltaTime;
        if (timerAttack >= timeBeforeAttack) {
            timerAttack = 0f;
            return true;
        }
        return false;
    }

    bool CheckCollision() {
        return trigger.getTriggered();         
    }

    void ResetTimer() {
        timer = 0f;
    }

    void Attack() {
        attackScript.alreadyAttack = false;
        anim.SetBool("Attack", true);
    }

    bool BareTime() {
        timer += Time.deltaTime;
        return timer >= bareTime ? true : false;
    }
    void Hide() {

        anim.SetBool("Attack", false);
        
    }
    void Die() {

    }



}

