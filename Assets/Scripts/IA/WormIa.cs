using UnityEngine;
using System.Collections;

public class WormIA : MonoBehaviour {

    float timer = 0f;
    GameObject target;
    public float itemEatingProbability;
    public float bareTime = 2f;
    FSM myIa;
    EnemyHealth myHealth;
    // Use this for initialization
    void Start() {
        myHealth = gameObject.GetComponent<EnemyHealth>();
        FSMState off = new FSMState();
        FSMState on = new FSMState();



        FSMState hide = new FSMState();
        FSMState chasing = new FSMState();
        FSMState attacking = new FSMState();
        FSMState angryChasing = new FSMState();
        FSMState angryAttacking = new FSMState();

        //hide
        hide.AddEnterAction(delegate () { target = GameElements.getGladiator(); });
        hide.AddTransition(new FSMTransition(StayNormal, chasing));
        hide.AddTransition(new FSMTransition(BeAggressive, angryChasing));

        //angry chasing
        angryChasing.AddEnterAction(BecomeAggressive);
        angryChasing.AddEnterAction(FindTarget);
        angryChasing.AddStayAction(AngryChase);
        angryChasing.AddTransition(new FSMTransition(CheckCollision, attacking));

        //angryAttacking
        angryAttacking.AddEnterAction(Attack);
        angryAttacking.AddEnterAction(ResetTimer);
        angryAttacking.AddTransition(new FSMTransition(BareTime, hide));

        //chasing
        chasing.AddStayAction(Chase);
        chasing.AddTransition(new FSMTransition(CheckCollision, attacking));

        //attacking
        attacking.AddEnterAction(Attack);
        attacking.AddEnterAction(ResetTimer);
        attacking.AddTransition(new FSMTransition(BareTime, hide));



        myIa = new FSM(hide);
        //StartCoroutine(Patrol());

    }

    // Update is called once per frame
    void Update() {
        myIa.Update();
    }

    bool StayNormal() {
        return myHealth.getCurrentHealth() > myHealth.getMaxHealth() / 2 ? true : false;
    }

    bool BeAggressive() {
        return !StayNormal();
    }

    void BecomeAggressive() {
        //TODO
        //check if aggressive color is set
        //eventually change it
        FindTarget();
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
        //TODO 
        //Insegue l'obiettivo
    }

    void Chase() {
        //TODO insegui il gladiatore (che poi è il target per semplicità)
    }

    bool CheckCollision() {
        //TODO 
        //Controlla collisioni con l'oggetto target
        return true;
    }

    void ResetTimer() {
        timer = 0f;
    }

    void Attack() {
        //TODO animazione attacco
    }

    bool BareTime() {
        timer += Time.deltaTime;
        return timer >= bareTime ? true : false;
    }



}

