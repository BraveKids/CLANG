using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MutantScriptTest : NetworkBehaviour
{
   
    Animator m_animator;
    GameObject target;
    //NavMeshAgent agent;
    public float distance;
    public float angle;
    Rigidbody rb;
    Vector3 direction;
    Vector3 velocity;
    public bool attack;
    public bool damaged;
    public int numOfAttack = 0;
    bool stoppedBefore;
    public GameObject attackTrigger;
    AILerp agent;
    IEnumerator cor;
    NetworkAnimator netAnim;
    // Use this for initialization
    
        /*
    public override void PreStartClient()
    {
        netAnim = GetComponent<NetworkAnimator>();
        for (int i = 0; i<3; i++)
        {
            netAnim.SetParameterAutoSend(i, true);
        }
    }*/

    void Start()
    {
        
       

        m_animator = GetComponent<Animator>();
        agent = GetComponent<AILerp>();
        
        rb = GetComponent<Rigidbody>();
        target = GameElements.getGladiator();
        if (target == null)
        {
            this.enabled = false;
            return;
        }
        agent.target = target.transform;
        cor = Attack();
        StartCoroutine(cor);
       

       
      
        stoppedBefore = false;
        attack = false;
        damaged = false;
        netAnim = GetComponent<NetworkAnimator>();
        /*for (int i = 0; i < 3; i++)
        {
            netAnim.SetParameterAutoSend(i, true);
        }*/

    }

    // Update is called once per frame
    void Update()
    {
       
        distance = Vector3.Distance(transform.position, target.transform.position);
        angle = Vector3.Angle(transform.forward, (target.transform.position - transform.position));


        if (distance <= 2.5f)
        {
            agent.canMove = false;
            transform.LookAt(target.transform);
            stoppedBefore = true;
            attack = true;
            
        }
        else
        {
            m_animator.SetBool("Attack", false);
            agent.canMove = true;
            attack = false;
            
        }
       
    }

    IEnumerator Attack()
    {
        while (true)
        {
            if (attack && !damaged)
            {

                m_animator.SetBool("Attack", true);
                Invoke("AttackUp", 0.6f);

                yield return new WaitForSeconds(2.3f);

            }
            else
            {
                
                yield return null;
            }
        }
    }

  
    

    private void AttackUp()
    {
        if (attack && !damaged)
        {
            attackTrigger.GetComponent<BoxCollider>().enabled = true;
            Invoke("AttackDown", 0.2f);
        }
    }

    private void AttackDown()
    {

        attackTrigger.GetComponent<BoxCollider>().enabled = false;
    }
    

 
    public void Damage()
    {
        m_animator.SetTrigger("Damage");
        netAnim.SetTrigger("Damage");
        damaged = true;
        attack = false;
        StopCoroutine(cor);
        
        attackTrigger.GetComponent<BoxCollider>().enabled = false;
        agent.canMove = false;
        agent.enableRotation = false;
        
        Invoke("DamageDown", 1f);

    }
    private void DamageDown()
    {
        damaged = false;
        StartCoroutine(cor);
        agent.canMove = true;
        agent.enableRotation = true;
        
        

    }

    public void Death()
    {
        m_animator.SetTrigger("Death");
        netAnim.SetTrigger("Death");
        attack = false;
        damaged = true;
        attackTrigger.GetComponent<BoxCollider>().enabled = false;
        attackTrigger.SetActive(false);
        StopCoroutine(cor);
      
        
        

        agent.canMove = false;
        agent.enableRotation = false;
        agent.enabled = false;
        
        
        
        
    }
    
   
}

