using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform hrac;
    [SerializeField] private float vyska = 8;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(hrac.position.x, hrac.position.y + vyska, hrac.position.z);
    }
}
