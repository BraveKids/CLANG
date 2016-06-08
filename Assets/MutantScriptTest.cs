using UnityEngine;
using System.Collections;


public class MutantScriptTest : MonoBehaviour {
    Animator m_animator;
    GameObject target;
    NavMeshAgent agent;
    public float distance;
    Vector3 velocity;
    public bool attack;
    public int numOfAttack =0;
    bool stoppedBefore;
    public GameObject attackTrigger;
	// Use this for initialization
	void Start () {
        target = GameElements.getGladiator();
        StartCoroutine(Attack());
        if (target == null)
        {
            this.enabled = false;
            return;
        }
        
        m_animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stoppedBefore = false;
        attack = false;
    }
	
	// Update is called once per frame
	void Update () {
        distance = Vector3.Distance(transform.position, target.transform.position);

        
        if (distance <=2f)
        {
           
            agent.velocity = Vector3.zero;
            agent.Stop();
            m_animator.SetFloat("Speed", 0f);
            transform.LookAt(target.transform);
            stoppedBefore = true;
            attack = true;
        }
        else {
            if (stoppedBefore)
            {
                agent.ResetPath();
            }
            attack = false;
            agent.SetDestination(target.transform.position);
            //agent.Resume();
            m_animator.SetFloat("Speed", agent.speed);
        }

        


       


        
	}

    IEnumerator Attack()
    {
        while (true)
        {
            if (attack)
            {

                m_animator.SetTrigger("Attack");
                Invoke("AttackUp", 0.6f);
                Invoke("AttackDown", 1f);
                yield return new WaitForSeconds(3);

            }
            else
            {
                yield return null;
            }
        }
    }

    private void AttackUp()
    {
        attackTrigger.SetActive(true);
    }

    private void AttackDown()
    {
        attackTrigger.SetActive(false);
    }
    /*
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("MeleeAttackTrigger"))
        {
            //m_animator.SetTrigger("Damage");
            gameObject.transform.FindChild("Model").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.green;
        }

    }*/
}

