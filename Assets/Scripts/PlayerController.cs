using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Weapon
{
    public string Name;
    public float Firerate; //je to v sekundach, mby predelat do RPM
    public float Damage;
    public float Range;
    public WeaponType Type;
    public ParticleSystem MuzzleFlash;
    public GameObject ImpactEffect;
    public GameObject ImpactEffectBlood;
    public GameObject WeaponModel;
    public AudioClip Sound;
}

public enum WeaponType
{
    Automatic, Shotgun,
}


public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 26;
    [SerializeField] float health = 100;
    [SerializeField] List<Weapon> weapons; 
    [SerializeField] int selectedWeaponIndex;
    [SerializeField] Transform shotsOrigin;
    private float timeSinceLastShot = 0;

    [SerializeField] private new Camera camera;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioSource audioSource;

    private float horizontalInput;
    private float verticalInput;

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        if (weapons[selectedWeaponIndex].Type == WeaponType.Automatic && Input.GetButton("Fire1") && timeSinceLastShot >= weapons[selectedWeaponIndex].Firerate)
        {
            timeSinceLastShot = 0;
            weapons[selectedWeaponIndex].MuzzleFlash.Play();
            //audioSource.PlayOneShot(weapons[selectedWeaponIndex].Sound);
            audioSource.clip = weapons[selectedWeaponIndex].Sound;
            audioSource.Play();

            RaycastHit hit;
            if (Physics.Raycast(shotsOrigin.position, shotsOrigin.forward, out hit, weapons[selectedWeaponIndex].Range))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(weapons[selectedWeaponIndex].Damage);
                    }

                    GameObject impactObj = Instantiate(weapons[selectedWeaponIndex].ImpactEffectBlood, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactObj, 0.2f);
                }
                else
                {
                    GameObject impactObj = Instantiate(weapons[selectedWeaponIndex].ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactObj, 0.2f);
                }
            }
        }
        else if (weapons[selectedWeaponIndex].Type == WeaponType.Shotgun && Input.GetButtonDown("Fire1") && timeSinceLastShot >= weapons[selectedWeaponIndex].Firerate) //ok vim ze toho tu je hodne duplicitniho, ale ted tu neni a nebude vice typu zbrani
        {
            timeSinceLastShot = 0;
            weapons[selectedWeaponIndex].MuzzleFlash.Play();
            audioSource.clip = weapons[selectedWeaponIndex].Sound;
            audioSource.Play();


            RaycastHit hit;
            shotsOrigin.Rotate(0, -6 / 2 * 2.5f, 0); //2.5 je magicke cislo indikujici spread peletkek shotguny ve stupnich. Pokud by bylo vice typu zbrani, tohle by se ofc presunulo do Weapon classky

            for (int i = 1; i <= 6; i++)
            {
                shotsOrigin.Rotate(new Vector3(0, 2.5f, 0));

                if (Physics.Raycast(shotsOrigin.position, shotsOrigin.forward, out hit, weapons[selectedWeaponIndex].Range))
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(weapons[selectedWeaponIndex].Damage);
                        }

                        GameObject impactObj = Instantiate(weapons[selectedWeaponIndex].ImpactEffectBlood, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(impactObj, 0.2f);
                    }
                    else
                    {
                        GameObject impactObj = Instantiate(weapons[selectedWeaponIndex].ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(impactObj, 0.2f);
                    }
                }
            }
            shotsOrigin.localEulerAngles = Vector3.zero;
        }


        //vice zbrani by se asi scalovalo pres new input system
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            uiManager.SwitchWeapon("M4");
            weapons[selectedWeaponIndex].WeaponModel.SetActive(false);
            weapons[0].WeaponModel.SetActive(true);
            selectedWeaponIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Count > 1)
        {
            uiManager.SwitchWeapon("ump");
            weapons[selectedWeaponIndex].WeaponModel.SetActive(false);
            weapons[1].WeaponModel.SetActive(true);
            selectedWeaponIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Count > 2)
        {
            uiManager.SwitchWeapon("shotgun");
            weapons[selectedWeaponIndex].WeaponModel.SetActive(false);
            weapons[2].WeaponModel.SetActive(true);
            selectedWeaponIndex = 2;
        }

        #region pohyb+anim
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
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

        if (Time.timeScale > 0) //fix aby se hrac neotacel v pause menu
            transform.rotation = Quaternion.LookRotation(finalVector);
        #endregion
    }

    public void FixedUpdate()
    {

        Vector3 pohyb = new Vector3(horizontalInput, 0, verticalInput);
        pohyb.Normalize();
        pohyb *= speed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + pohyb);
    }

    public void TakeDamage(float damage)
    {
        // ridat red screen overlay pri hitu

        health -= damage;

        //StartCoroutine(uiManager.ChangeHealth(health, damage < 0));
        uiManager.ChangeHealth(health, damage < 0);

        if (health <= 0)
        {
            //todo: konec hry
            GameManager.Instance.GameOver();
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

    void OnTriggerEnter(Collider c)
    {
        //ok, vim ze tyhle namingy nejsou uplne nejlepsi, ale az zas tolik to nevadi

        if (c.gameObject.name == "box_med")
        {
            health += health <= 40 ? 60 : 100 - health; //bascially prida 60hp nebo do 100
            uiManager.ChangeHealth(health, true);
            Destroy(c.gameObject);
        }
        else if (c.gameObject.name == "UMP-45 Variant")
        {
            weapons.Add(GameManager.Instance.Weapons[1]);
            uiManager.EnableWeapon(1);
            Destroy(c.gameObject);
        }
        else if (c.gameObject.name == "Object001")//shotgun
        {
            weapons.Add(GameManager.Instance.Weapons[2]);
            uiManager.EnableWeapon(2);
            Destroy(c.gameObject);
        }
    }
}
