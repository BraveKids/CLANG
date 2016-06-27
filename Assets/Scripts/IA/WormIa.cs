﻿using UnityEngine;
using System.Collections;

public class WormIA : MonoBehaviour {
    public float normalSpeed;
    public GameObject[] wurmModel;
    public float boostSpeed;
    public GameObject dust;
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
    Animator anim;
    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
        myHealth = gameObject.GetComponent<EnemyHealth>();
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
        

    }

    // Update is called once per frame
    void Update() {
        myIa.Update();
    }

    bool StayNormal() {
        timerChase += Time.deltaTime;
        if (timerChase >= timeBeforChasing)         //Wait in hide before chasing again
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
    //Become aggressive if health is less the half healt
    bool BeAggressive()
    {

        
        timerChase += Time.deltaTime;
        if (timerChase >= timeBeforChasing)         //Wait in hide before chasing again
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
        FindTarget();
        foreach (GameObject model in wurmModel)
        {
            model.GetComponent<MeshRenderer>().material.color = Color.red;
        }
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
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.transform.position.x-4f,
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
        dust.SetActive(false);
        Invoke("Attacked", 0.5f);
    }


    void Attacked()
    {
        attackScript.alreadyAttack = true;
    }

    bool BareTime() {
        timer += Time.deltaTime;
        return timer >= bareTime ? true : false;
    }

    void Hide() {

        anim.SetBool("Attack", false);
        dust.SetActive(true);
        Invoke("NotAttacked", 2f);

    }
    void NotAttacked()
    {
        attackScript.alreadyAttack = false;
    }

    void Die() {
        if (anim.GetBool("Attack") == false)
        {
            anim.SetBool("Attack", false);
        }
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



}

