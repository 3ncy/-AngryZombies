using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 26;
    [SerializeField] float health = 100;
    [SerializeField] Transform shotsOrigin;
    [SerializeField] float firerate = 0.25f; //tohle presunout do propetries jednotlivych zbrani l8tr //je to v sekundach, mby predelat do RPM
    private float timeSinceLastShot = 0;
    [SerializeField] float damage = 9999999;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject impactEffect;

    private new Camera camera;
    private Animator animator;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: rozhazet do metod Move(); ANimations(); Shoot();, nebo mby HandleInput(); a ANimations();

        timeSinceLastShot += Time.deltaTime;
        if (Input.GetButton("Fire1") && timeSinceLastShot >= firerate)//todo: pridat moznost ze nektere zbrane jsou single fire
        {
            timeSinceLastShot = 0;
            muzzleFlash.Play();

            RaycastHit hit;
            if (Physics.Raycast(shotsOrigin.position, shotsOrigin.forward, out hit))
            {
                GameObject impactObj = Instantiate(impactEffect, hit.point, Quaternion.identity); //todo: pridat rotaci aby byla ven ze surface/ve spravnem uhlu k uhlu strely.
                Destroy(impactObj, 0.2f);

                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.CompareTag("Enemy"))
                {
                    EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
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
        Vector3 poziceHraceNaObrazovce = camera.WorldToScreenPoint(transform.position);
        Vector3 poziceMysiNaObrazovce = Input.mousePosition;
        Vector3 SmerKMysi = poziceMysiNaObrazovce - poziceHraceNaObrazovce;
        Vector3 FinalniVektor = new Vector3(SmerKMysi.x, 0, SmerKMysi.y);
        transform.rotation = Quaternion.LookRotation(FinalniVektor);

    }
}
