using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationPhoneTest : MonoBehaviour
{
    private Rigidbody _rigid;
    private Vector3  _grav;
    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        //no work
        _grav = Input.acceleration;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.acceleration.magnitude);
        if (Input.acceleration.magnitude > 2f)
        {   //PLAN: Also Hannah und Zukunfts Niklas. Wir machen das einfach so: Falls die acceleration über nen bestimmten Punkt kommt (public) dann adden wir force und es macht sich was im Kanister. 
            Debug.Log("WOHOOOOO");
            _rigid.AddForce(Input.acceleration - _grav);
        }
        //Debug.Log((Input.acceleration.x - _grav.x));
    }
}
