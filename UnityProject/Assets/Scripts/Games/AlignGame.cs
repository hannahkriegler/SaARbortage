using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public class AlignGame : Game
    {
        public GameObject rightZahnrad;
        public GameObject leftZahnrad;

        public GameObject innerRing;
        private Vector3 innerRingUp;
        private Transform InnerEmpty;
        public GameObject outerRing;
        private Vector3 outerRingUP;
        private Transform OuterEmpty;

       

        private float targetdistance;

        public GameObject mesh;

        public Material Indicator;

        public float RingRotationSpeed = 1f;
        public float RotationSpeed = 20;

        public float FairnessThreshold = 5f;

        private float OffsetAngl;

        private void Awake()
        {
            InnerEmpty = innerRing.transform.GetChild(0).transform;
            OuterEmpty = outerRing.transform.GetChild(0).transform;
            SetUpGame();
        }

        // Also hier werden die Winkel erstmals verstellt, dass es replayable is.
        private void SetUpGame()
        {
            OffsetAngl = UnityEngine.Random.Range(1, 359);
            innerRing.transform.Rotate(new Vector3(OffsetAngl, OffsetAngl, OffsetAngl));
            innerRingUp = innerRing.transform.up;
            outerRing.transform.Rotate(new Vector3(OffsetAngl, OffsetAngl, OffsetAngl));
            outerRingUP = outerRing.transform.up;
            Debug.Log("L�sung " + innerRing.transform.rotation.ToString() + " and " + outerRing.transform.rotation.ToString());

            targetdistance = Vector3.Distance(InnerEmpty.position, OuterEmpty.position);
            Debug.Log(targetdistance);
            

            float OffsetAngle1 = UnityEngine.Random.Range(45,360);
            //float OffsetAngle2 = UnityEngine.Random.Range(45, 360);
            innerRing.transform.Rotate(new Vector3 (OffsetAngle1, OffsetAngle1, 0));
            outerRing.transform.Rotate(new Vector3(0, OffsetAngle1, OffsetAngle1));
            Indicator.SetColor("_EmissionColor", new Color (255f, 0f, 0f));

        }

        private void Update()
        {
            //if (!launch) return;
            //if(!IsLocalPlayer) return;   
            if(Input.GetMouseButtonUp(0)) TestAlignment();
        }

        public void RotateRing(GameObject ring, float prog)
        {
            if (ring == innerRing) {
                //L�sung aus: https://forum.unity.com/threads/two-rotatearound-calls-lead-to-3-axis-rotation.362943/ ABER is ehrlich gesagt nicht so geil tbh...
                innerRing.transform.RotateAround(innerRing.transform.position, innerRingUp, prog * RingRotationSpeed);
                innerRing.transform.RotateAround(innerRing.transform.position, innerRing.transform.right, prog * RingRotationSpeed);
            } else
                {
                outerRing.transform.RotateAround(outerRing.transform.position, outerRingUP, prog * RingRotationSpeed);
                outerRing.transform.RotateAround(outerRing.transform.position, outerRing.transform.forward, prog * RingRotationSpeed);
            }

        }

        private void TestAlignment()
        {

            Debug.Log(Vector3.Distance(InnerEmpty.position, OuterEmpty.position));
            if (Vector3.Distance(InnerEmpty.position, OuterEmpty.position) <= targetdistance + FairnessThreshold/100) EndGame();
            /*//Problem: Manchmal tackt das beim rotieren doch noch die angles slighty. Also wird die achse die bspw. nicht rotiert werden soll auf einmal -179,999 anstatt 0 bspw. Das k�nnte problematisch werden..
            Debug.Log(Quaternion.Angle(innerRing.transform.rotation, outerRing.transform.rotation).ToString());
            if (Quaternion.Angle(innerRing.transform.rotation, outerRing.transform.rotation) <= FairnessThreshold)
            {
                EndGame(); } */
        }

        private void EndGame()
        {
            Debug.Log("End");
            Indicator.SetColor("_EmissionColor", new Color(0f, 255f, 0f));
        }

        public override void LaunchGame()
        {
            base.LaunchGame();
            mesh.SetActive(true);
        }

        
        private void RotateRing(Player player, GameObject ring)
        {
            // first player action
            if (player.Equals(players[0])) 
            {
                
            }

            // second player action
            else
            {
                
            }
        }
    }
}
