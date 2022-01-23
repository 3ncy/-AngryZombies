using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float health = 100;
    //mby add ragdolls?
    [SerializeField] Collider mainCollider;//todo: predelat na list collidery, incase velky enemy je postaveny z vice tvaru
    Rigidbody rb; 

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            animator.SetTrigger("Died");
            rb.useGravity = false;
            mainCollider.enabled = false;
            //Destroy(this, 5); //mby
        }
    }
}
