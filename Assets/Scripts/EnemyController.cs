using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float health = 100;

    [SerializeField] Transform player;
    [SerializeField] NavMeshAgent agent;

    Weapon punch = new Weapon
    {
        Firerate = 4,
        Damage = 10
    };
    private float timeSinceLastPunch = 0;

    //mby add ragdolls?
    //[SerializeField] Collider mainCollider;//todo: predelat na list collidery, incase velky enemy je postaveny z vice tvaru
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //todo: najit hrace a priradit do player promenne
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.position);


        animator.SetBool("Running", agent.velocity.magnitude > 0); //todo: mby setovat jenom pokud se zacina/prestava hybat, mozna pomuze optimalizaci

        //todo: je tu offset animace a kdy se actually dava dmg.
        //mby courotines? idk jdu spat

        bool attacking = agent.remainingDistance < 1.5f;
        animator.SetBool("Attacking", attacking);
        timeSinceLastPunch += Time.deltaTime;// actually mozna neni potreba a hodilo by se to opacne. For now to nebudu resit a uvidi se, jake problemy to dela pri balancovani// * (attacking ? 1 : 0); //resetuje punch cooldown kdyz zrovna zombie neni v attack distance
        if (attacking && timeSinceLastPunch > punch.Firerate)
        {
            timeSinceLastPunch = 0;
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null) playerController.TakeDamage(punch.Damage);
            Debug.LogWarning(gameObject.name + " Hit player");

        }
        
        //agent.enabled = rb.isKinematic = rb.velocity.magnitude < 0.01;  // vypnout agenta a zapnout RB pokud je nejaka force na rb


    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //rb.isKinematic = false;
            agent.enabled = false;
            animator.SetTrigger("Died");
            //mainCollider.enabled = false;
            //rb.useGravity = false;
            foreach (Collider c in GetComponents<Collider>())
            {
                c.enabled = false;
            }
            Destroy(this);    //this.enabled = false;
            //rb.constraints = RigidbodyConstraints.FreezeAll;
            //Destroy(this, 5); //mby
        }
    }
}
