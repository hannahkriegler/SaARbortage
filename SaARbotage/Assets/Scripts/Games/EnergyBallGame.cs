using System;
using System.Collections;
using System.Collections.Generic;
using SaARbotage;
using UnityEngine;
using UnityEngine.UI;


public class EnergyBallGame : Game
{

    //TODO: Umbauen auf Slider, Farbe von Holding ändern usw!
    public Text hpBar;
    private float _hp = 100f;

    public Button startButton;
    public Button finishButton;
    public Text endmessage;

    public float timeConstraint = 60f;

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
    public Rigidbody ballthingRigidbody;
    private Vector3  _grav;

    [Range(0, 5f)]
    public float threshold = 2f;

    void Start()
    {
        //ballthingRigidbody = GetComponent<Rigidbody>();
        if (ballthingRigidbody == null)
        {
            Debug.Log("No RB!!");
        }
        _grav = Input.acceleration;
        _mypos = capsule.transform.localPosition;
        _maincam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        if (!launch.Value) return;
        TickingTimer();
        //Debug.Log(Input.acceleration.magnitude);
        if (!(Input.acceleration.magnitude > threshold)
        ) return; //PLAN: Also Hannah und Zukunfts Niklas. Wir machen das einfach so: Falls die acceleration über nen bestimmten Punkt kommt (public) dann adden wir force und es macht sich was im Kanister. 
        
        var accel = Input.acceleration;
        ballthingRigidbody.AddForce(accel - _grav);
        DrainLife(accel);
        
    }

    private void TickingTimer()
    {
        _hp -= (100f / timeConstraint * Time.deltaTime);
        if (_hp <= 0)
        {
            //Do the Failstate
            hpBar.text = "0";
        }
        else
        {
            int hpI = (int)_hp;
            hpBar.text = hpI.ToString();
        }

    }

    private void DrainLife(Vector3 accel)
    {
        _hp -= accel.magnitude * balanceMultiplier;
        if (_hp <= 0) _hp = 0;
        if (energyBall != null)
        {
            var duration = 1.0f;
            StartCoroutine(ColorChange(Color.red, duration));
        }
        var sHP = (int)_hp;
        hpBar.text = sHP.ToString();
    }

    private IEnumerator  ColorChange(Color red, float duration)
    {
        var time = 0f;
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

    public override void LaunchGame()
    {
        //Here the cannister is detached from the holding mechanism and attached to the camera. 
        // The player then needs to transport it to the Target Holding thing. 
        // If it collides with the target a button should appear to finish the task.
        base.LaunchGame();
        capsule.transform.parent = _maincam.transform;
        startButton.gameObject.SetActive(false);
        
    }

    public override void FinishGame()
    {
        //When the finishButton is pressed. 
        // The cannister will be positioned as child of the target. 
        capsule.transform.parent = _targetHolding;
        capsule.transform.localPosition = _mypos;
        capsule.transform.localRotation = Quaternion.Euler(0, 0, 0);
        endmessage.gameObject.SetActive(true);
        _finished = true;
        base.FinishGame();
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
