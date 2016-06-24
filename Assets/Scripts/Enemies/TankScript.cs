using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TankScript : NetworkBehaviour
{
    public float normalSpeed;
    public float shieldSpeed;
    Animator m_animator;
    GameObject target;
    //NavMeshAgent agent;
    public float distance;
    public float angle;
    Rigidbody rb;
    public bool attack;
    public bool damaged;
    public int numOfAttack = 0;
    public bool shielded;
    public GameObject attackTrigger;
    AILerp agent;
    IEnumerator cor;
    NetworkAnimator netAnim;
    // Use this for initialization


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




        shielded = false;
        attack = false;
        damaged = false;
        netAnim = GetComponent<NetworkAnimator>();
      

    }

    // Update is called once per frame
    void Update()
    {

        distance = Vector3.Distance(transform.position, target.transform.position);
        angle = Vector3.Angle(transform.forward, (target.transform.position - transform.position));


        if (distance <= 2.5f)
        {
            shielded = false;
            agent.speed = 0f;
            agent.canMove = false;
            transform.LookAt(target.transform);

            attack = true;

        }
        else
        {
            m_animator.SetBool("Attack", false);
            if (target.GetComponent<GladiatorShooting>().specialAttack)
            {
                m_animator.SetBool("Run", true);
                shielded = true;
                agent.speed = shieldSpeed;
                GetComponent<EnemyHealth>().m_Resistance = 11f;
            }
            else
            {
                shielded = false;
                m_animator.SetBool("Run", false);
                agent.speed = normalSpeed;
                GetComponent<EnemyHealth>().m_Resistance = 9f;
            }
            
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
                Invoke("AttackUp", 0.8f);

                yield return new WaitForSeconds(2f);

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
        if (!shielded)
        {
        
            netAnim.SetTrigger("Damage");
            damaged = true;
            attack = false;
            StopCoroutine(cor);
            agent.speed = 0f;
            attackTrigger.GetComponent<BoxCollider>().enabled = false;
            agent.canMove = false;
            agent.enableRotation = false;

            Invoke("DamageDown", 1f);
        }
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
   
        netAnim.SetTrigger("Death");
        attack = false;
        damaged = true;
        attackTrigger.GetComponent<BoxCollider>().enabled = false;
        attackTrigger.SetActive(false);
        StopCoroutine(cor);



        agent.speed = 0f;
        agent.canMove = false;
        agent.enableRotation = false;
        agent.enabled = false;




    }


}

