using UnityEngine;
using System.Collections;


public class MutantScriptTest : MonoBehaviour {
    Animator m_animator;
    GameObject target;
    NavMeshAgent agent;
    public float distance;
    public float angle;
    Vector3 direction;
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
        agent.updateRotation = false;

    }
	
	// Update is called once per frame
	void Update () {
        distance = Vector3.Distance(transform.position, target.transform.position);
        angle = Vector3.Angle(transform.forward, (target.transform.position - transform.position));
    


        if (distance <= 2.5f)
        {

            agent.velocity = Vector3.zero;
            agent.Stop();
            //m_animator.SetFloat("Speed", 0f);
            m_animator.SetBool("Run", false);
            transform.LookAt(target.transform);
            stoppedBefore = true;
            attack = true;
        }
        else
        {
            if (angle > 45 && distance > 2.5f)
            {
                stoppedBefore = true;
                attack = false;
                agent.velocity = Vector3.zero;
                agent.Stop();
                m_animator.SetBool("Run", false);
                m_animator.SetBool("Attack", false);
                //m_animator.SetFloat("Speed", 0f);
                Quaternion targetRotation = Quaternion.LookRotation((target.transform.position - transform.position), Vector3.up);

                Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
                //GetComponent<Rigidbody>().MoveRotation(newRotation);
                gameObject.transform.rotation = newRotation;
                //transform.LookAt(target.transform);
                
            }
            else
            {
                //transform.LookAt(target.transform);
                //transform.LookAt(target.transform);
                if (stoppedBefore)
                {
                    agent.Resume();
                    agent.ResetPath();
                }
                stoppedBefore = false;
                attack = false;
                agent.SetDestination(target.transform.position);
                //agent.Resume();
                //m_animator.SetFloat("Speed", agent.speed);
                m_animator.SetBool("Run", true);
            }
        }

        


       


        
	}

    IEnumerator Attack()
    {
        while (true)
        {
            if (attack)
            {

                m_animator.SetBool("Attack",true);
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
        m_animator.SetBool("Attack", false);
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

