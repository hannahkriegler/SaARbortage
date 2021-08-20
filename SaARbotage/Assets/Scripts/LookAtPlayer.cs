using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Camera _maincam; 
    // Start is called before the first frame update
    void Start()
    {
        _maincam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position - (_maincam.transform.position - transform.position));
    }
}
