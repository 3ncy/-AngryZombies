using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float health = 100;

    [SerializeField] Transform player;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] float damage = 10;

    //mby add ragdolls?

    //[SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;

    void Start()
    {
        player = GameManager.Instance.Player;
    }

    void Update()
    {
        agent.SetDestination(player.position);

        animator.SetBool("Running", agent.velocity.magnitude > 0);

        animator.SetBool("Attacking", agent.remainingDistance < 1.5f); //todo: mby vypnout animace, po smrti hrace //actually ne, zombie budou utocit na tu mrtvolu
       

        //agent.enabled = rb.isKinematic = rb.velocity.magnitude < 0.01;  // vypnout agenta a zapnout RB pokud je nejaka force na rb
    }

    public void Attack()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null) playerController.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {


            agent.enabled = false;
            animator.SetTrigger("Died");
            foreach (Collider c in GetComponents<Collider>())
            {
                c.enabled = false;
            }
            transform.position -= new Vector3(0, 0.05f, 0); //dirty fix na levitujici mrtvoly
            this.enabled = false;
            
            GameManager.Instance.AddScore(1, this);
            //Destroy(gameObject, 5); //zmizeni mrtvoly po 5s 
        }
    }

    public void Restart()
    {
        agent.enabled = true;
        foreach(Collider c in GetComponents<Collider>())
        {
            c.enabled = true;
        }
        this.enabled = true;

        animator.Rebind();
    }
}