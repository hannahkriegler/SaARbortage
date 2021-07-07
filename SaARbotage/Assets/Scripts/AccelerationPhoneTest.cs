using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccelerationPhoneTest : MonoBehaviour
{

    //TODO: Umbauen auf Slider, Farbe von Holding ändern usw!
    public Text hpBar;

    public Button startButton;
    public Button finishButton;
    public Text endmessage; 

    public GameObject capsule;
    private Vector3 _mypos;
    private GameObject _maincam;
    private Transform _targetHolding;
    private Boolean _finished = false;
    private Boolean _started = false;

    [Range(0,1f)]
    public float balanceMultiplier = 1f;
    public Material energyBall;
    public Material outerTesseract;
    public Color oldcol;
    [Range(0,10f)]
    public float outerStrength = 1f;
    private Rigidbody _rigid;
    private Vector3  _grav;

    [Range(0, 5f)]
    public float threshold = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        if (_rigid == null)
        {
            Debug.Log("No RB!!");
        }
        //no work
        _grav = Input.acceleration;
        _mypos = capsule.transform.localPosition;
        _maincam = GameObject.FindGameObjectWithTag("MainCamera");


    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.acceleration.magnitude);
        if (Input.acceleration.magnitude > threshold && _started)
        {   //PLAN: Also Hannah und Zukunfts Niklas. Wir machen das einfach so: Falls die acceleration über nen bestimmten Punkt kommt (public) dann adden wir force und es macht sich was im Kanister. 
            Debug.Log("WOHOOOOO");
            Vector3 accel = Input.acceleration;
            _rigid.AddForce(accel - _grav);
            DrainLife(accel);
        }
        //Debug.Log((Input.acceleration.x - _grav.x));
    }

    private void DrainLife(Vector3 accel)
    {
        float hp = float.Parse(hpBar.text);
        hp = hp - accel.magnitude * balanceMultiplier;
        if (hp <= 0) hp = 0;
        if (energyBall != null)
        {
            float duration = 1.0f;
            StartCoroutine(ColorChange(Color.red, duration));
        }
        int sHP = (int)hp;
        hpBar.text = sHP.ToString();
    }

    IEnumerator  ColorChange(Color red, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            energyBall.SetColor("_Color", red * 100);
            outerTesseract.SetColor("_EmissionColor", red * outerStrength);
            time += Time.deltaTime;
            yield return null;
        }
        energyBall.SetColor("_Color", oldcol * 100);
        outerTesseract.SetColor("_EmissionColor", oldcol * outerStrength);
        yield return null;
    }

    public void StartChallenge()
    {
        //Here the cannister is detached from the holding mechanism and attached to the camera. 
        // The player then needs to transport it to the Target Holding thing. 
        // If it collides with the target a button should appear to finish the task.

        capsule.transform.parent = _maincam.transform;
        startButton.gameObject.SetActive(false);
        _started = true;
    }

    public void FinishChallenge()
    {
        //When the finishButton is pressed. 
        // The cannister will be positioned as child of the target. 
        capsule.transform.parent = _targetHolding;
        capsule.transform.localPosition = _mypos;
        capsule.transform.localRotation = Quaternion.Euler(0, 0, 0);
        endmessage.gameObject.SetActive(true);
        _finished = true;
        finishButton.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cannister" && !_finished)
        {
            finishButton.gameObject.SetActive(true);
            _targetHolding = other.transform;
        }
    }
}
