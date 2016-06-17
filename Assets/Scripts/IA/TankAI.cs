using UnityEngine;
using System.Collections;

public class TankAI : MonoBehaviour {

    FSM tankFSM;

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


	// Use this for initialization
	void Start () {
        target = GameElements.getGladiator();
        health = GetComponent<EnemyHealth>();

        FSMState chasing = new FSMState();
        FSMState shieldChasing = new FSMState();
        FSMState attacking = new FSMState();
        FSMState dying = new FSMState();

        //chasing
        chasing.AddStayAction(Chasing);
        chasing.AddEnterAction(ResetTimer);
        chasing.AddTransition(new FSMTransition(GladiatorShootingMe, shieldChasing));
        chasing.AddTransition(new FSMTransition(GladiatorInRange, attacking));
        chasing.AddTransition(new FSMTransition(GoigToDie, dying));

        //shieldChasing
        shieldChasing.AddStayAction(ShieldChasing);
        shieldChasing.AddTransition(new FSMTransition(GladiatorInRange, attacking));
        shieldChasing.AddTransition(new FSMTransition(GladiatorStopShootingMe, chasing));
        shieldChasing.AddTransition(new FSMTransition(GoigToDie, dying));

        //attacking
        attacking.AddStayAction(Attacking);
        attacking.AddTransition(new FSMTransition(GladiatorOutOfRange, chasing));
        attacking.AddTransition(new FSMTransition(GoigToDie, dying));

        //dying
        dying.AddEnterAction(Dying);

        tankFSM = new FSM(chasing);

	}
	
	// Update is called once per frame
	void Update () {
        tankFSM.Update();
	}

    bool GladiatorInRange() {
        //TOTO controlla se il gladiatore è nel raggio d'azione del tank
        return gladiatorInRange;
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
        Chase(normalSpeed);
    }

    void ShieldChasing() {
        DebugLine("Shield chasing");
        Chase(shieldSpeed);
    }

    void Chase(float speed) {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.transform.position.x,
                                                                               transform.position.y,
                                                                               target.transform.position.z)
                                                                               , speed * Time.deltaTime);
    }

    void Dying() {
        //TODO animazione dying e destroy del gameobject. Ricordati di mettere nell'OnDestroy l'eventuale rimozione dalla lista del player (se presente)
        DebugLine("Dead");
    }

    void Attacking() {

        if (timer == 0) {
            //TODO animazione attacco e danno
            DebugLine("Attack "+Random.value);
        }
        timer += Time.deltaTime;
        if (timer >= attackTime) {
            ResetTimer();
        }
    }

    void ResetTimer() {
        timer = 0;
    }

    void DebugLine(string text) {
       if(debugMode) Debug.Log(text);
    }
}
