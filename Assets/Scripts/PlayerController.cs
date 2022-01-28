﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Weapon
{
    public float Firerate; //je to v sekundach, mby predelat do RPM
    public float Damage;
    public ParticleSystem MuzzleFlash;
    public GameObject ImpactEffect;
}


public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 26;
    [SerializeField] float health = 100;
    [SerializeField] List<Weapon> weapons = new List<Weapon>(); //M4 - firerate 0.25, damage 20
    [SerializeField] int selectedWeaponIndex;
    [SerializeField] Transform shotsOrigin;
    private float timeSinceLastShot = 0;

    [SerializeField] private new Camera camera;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    { }

    // Update is called once per frame
    void Update()
    {
        //TODO: rozhazet do metod Move(); ANimations(); Shoot();, nebo mby HandleInput(); a ANimations();

        timeSinceLastShot += Time.deltaTime;
        if (Input.GetButton("Fire1") && timeSinceLastShot >= weapons[selectedWeaponIndex].Firerate)//todo: pridat moznost ze nektere zbrane jsou single fire
        {
            timeSinceLastShot = 0;
            weapons[selectedWeaponIndex].MuzzleFlash.Play();

            RaycastHit hit;
            if (Physics.Raycast(shotsOrigin.position, shotsOrigin.forward, out hit))
            //if (Physics.Raycast(transform.position, transform.forward, out hit)) //todo: mby, chce to vic testing
            {
                //todo: jinej particle eff pokud dopadne hit na zombie 
                //todo: udelat ten vfx
                GameObject impactObj = Instantiate(weapons[selectedWeaponIndex].ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactObj, 0.2f);

                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.CompareTag("Enemy"))
                {
                    EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(weapons[selectedWeaponIndex].Damage);
                    }

                    //pridani knockbacku, mby udelat l8tr
                    //hit.rigidbody.addforce
                    //ale na to nesmi byt RB kinematic, takze na enemy scriptu checkovat rb.velocity a pokud tam neni velocity (rb nepohybuje objektem)
                    //  tak zapnout rb.kinematic a zapnout agent.enabled
                    // a ofc opacne kdyz je rb.velocity, tak zase vypnout agenta a kinematic.
                }
            }
        }


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        Vector3 pohyb = new Vector3(horizontalInput, 0, verticalInput);
        pohyb.Normalize();
        pohyb *= speed * Time.deltaTime;
        rb.MovePosition(transform.position + pohyb);
        //transform.Translate(pohyb, Space.World);

        //ANIMACE
        float otoceni = Mathf.Atan2(transform.forward.z, transform.forward.x); //v Rad
        float uhelChuze = Mathf.Atan2(verticalInput, horizontalInput); //v Rad
        float rozdil = otoceni - uhelChuze; //v Rad

        if (horizontalInput == 0 && verticalInput == 0)
        {
            animator.SetFloat("x", 0, 0.05f, Time.deltaTime);  //0.05f je cas prechodu mezi animacema aby to tak nesnapovalo
            animator.SetFloat("y", 0, 0.05f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("x", Mathf.Sin(rozdil), 0.05f, Time.deltaTime);
            animator.SetFloat("y", Mathf.Cos(rozdil), 0.05f, Time.deltaTime);
        }

        if (health <= 0)
        {
            animator.SetTrigger("Died");
            this.enabled = false;
        }

        //otaceni kamery
        Vector3 playerScreenPos = camera.WorldToScreenPoint(transform.position);
        Vector3 cursorScreenPos = Input.mousePosition;
        Vector3 directionToCursor = cursorScreenPos - playerScreenPos;
        Vector3 finalVector = new Vector3(directionToCursor.x, 0, directionToCursor.y);
        transform.rotation = Quaternion.LookRotation(finalVector);

    }

    public void TakeDamage(float damage)
    {

        // ridat red screen overlay pri hitu

        health -= damage;
        if (health <= 0)
        {
            //death screen
            animator.SetTrigger("Died");
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            foreach (Collider c in GetComponents<Collider>())
            {
                c.enabled = false;
            }
            Destroy(this);
        }
    }
}
