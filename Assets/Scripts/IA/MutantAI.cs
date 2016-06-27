using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MutantAI : MonoBehaviour {

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
    float distance;
    NetworkAnimator netAnim;
    float timer;

    // Use this for initialization
    void Start() {
       // target = GameElements.getGladiator();
        //health = GetComponent<EnemyHealth>();
        //agent = GetComponent<AILerp>();
        //anim = GetComponent<Animator>();
        //netAnim = GetComponent<NetworkAnimator>();


        FSMState chasing = new FSMState();
        FSMState attacking = new FSMState();
        FSMState damaged = new FSMState();
        FSMState dying = new FSMState();

        chasing.AddStayAction(Chase);
        chasing.AddEnterAction(ResetTimer);
        chasing.AddTransition(new FSMTransition(GladiatorInRange, attacking));
        chasing.AddTransition(new FSMTransition(IsDamaged, damaged));

        attacking.AddStayAction(Attack);
        attacking.AddTransition(new FSMTransition(GladiatorOutOfRange, chasing));
        attacking.AddTransition(new FSMTransition(IsDamaged, damaged));

        damaged.AddEnterAction(GetDamage);
        damaged.AddTransition(new FSMTransition(PerkUp, chasing));
        damaged.AddTransition(new FSMTransition(GoigToDie, dying));

        dying.AddEnterAction(Die);

        mutantFSM = new FSM(chasing);





    }

    // Update is called once per frame
    void Update() {
        mutantFSM.Update();
    }

    void Chase() {
        DebugLine("Chasing");
    }

    void Attack() {
        if (timer == 0) {

            //Invoke("Attack", 0.75f);
            DebugLine("Attack " + Random.value);
        }

        timer += Time.deltaTime;
        if (timer >= attackTime) {
            ResetTimer();
        }
    }

    bool GladiatorInRange() {
        /*distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= 2.5f) {
            return true;
        }
        //TOTO controlla se il gladiatore è nel raggio d'azione del tank
        return false;*/
        return inRange;
    }

    bool GladiatorOutOfRange() {
        return !GladiatorInRange();
    }

    bool IsDamaged() {
        return isDamaged;
    }

    bool GoigToDie() {
        /*if (health.getCurrentHealth() <= 0)
            return true;
        return false;*/
        return die;

    }

    void Chasing() {
        /*anim.SetBool("Run", charge);
        anim.SetBool("Attack", false);
        agent.canMove = true;
        agent.speed = speed;*/
    }

    void Attacking() {
        anim.SetBool("Attack", true);
        agent.speed = 0f;
        agent.canMove = false;
        if (timer == 0) {

            Invoke("Attack", 0.75f);
            DebugLine("Attack " + Random.value);
        }

        timer += Time.deltaTime;
        if (timer >= attackTime) {
            ResetTimer();
        }
    }

    void GetDamage() {

        //anim.SetTrigger("Damage");
       /* anim.SetBool("Attack", false);
        anim.SetBool("Run", false);
        netAnim.SetTrigger("Damage");*/
        isDamaged = false;
        DebugLine("damage");
    }

    void DebugLine(string text) {
        if (debugMode) Debug.Log(text);
    }

    bool PerkUp() {
        damageTimer += Time.deltaTime;
        if (damageTimer >= damageTime) {
            damageTimer = 0;

            return true;
        }
        return false;
    }

    void Die() {
        /*agent.speed = 0f;
        agent.canMove = false;
        //TODO animazione dying e destroy del gameobject. Ricordati di mettere nell'OnDestroy l'eventuale rimozione dalla lista del player (se presente)
        anim.SetTrigger("Death");
        netAnim.SetTrigger("Death");
        Invoke("Destroy", 2f);*/
        DebugLine("Dead");

    }

    void ResetTimer() {
        //agent.target = target.transform;
        timer = 0;
    }

    void OnDestroy() {
        target.GetComponent<GladiatorShooting>().RemoveTarget(gameObject.transform);
    }



}
