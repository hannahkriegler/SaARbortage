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
        public Material InnerMat;

        public GameObject middleRing;
        private Vector3 middleRingUp;
        private Transform middleEmpty;
        public Material middleMat;

        public GameObject outerRing;
        private Vector3 outerRingUP;
        private Transform OuterEmpty;
        public Material OuterMat;

        private float Duration = 0;
        public float MaxDur = 2;
        
        private float _targetdistance1;
        private float _targetdistance2;

        public GameObject mesh;

        public Material Indicator;

        public float outerRingRotationSpeed = 1f;
        public float middleRingRotationSpeed = 1f;
        public float innerRingRotationSpeed = 1f;
        public float RotationSpeed = 20;

        public float FairnessThreshold = 5f;

        private void Awake()
        {
            InnerEmpty = innerRing.transform.GetChild(0).transform;
            middleEmpty = middleRing.transform.GetChild(0).transform;
            OuterEmpty = outerRing.transform.GetChild(0).transform;
            SetUpGame();
        }

        // Also hier werden die Winkel erstmals verstellt, dass es replayable is.
        private void SetUpGame()
        {
            float OffsetAngl = UnityEngine.Random.Range(1, 359);
            innerRing.transform.Rotate(new Vector3(OffsetAngl, OffsetAngl, OffsetAngl));
            innerRingUp = innerRing.transform.up;
            middleRing.transform.Rotate(new Vector3(OffsetAngl, OffsetAngl, OffsetAngl));
            middleRingUp = outerRing.transform.up;
            outerRing.transform.Rotate(new Vector3(OffsetAngl, OffsetAngl, OffsetAngl));
            outerRingUP = outerRing.transform.up;
            Debug.Log("Lösung " + innerRing.transform.rotation.ToString());

            _targetdistance1 = Vector3.Distance(InnerEmpty.position, middleEmpty.position);
            _targetdistance2 = Vector3.Distance(middleEmpty.position, OuterEmpty.position);
            Debug.Log(_targetdistance1 + " " + _targetdistance2);
            

            float OffsetAngle1 = UnityEngine.Random.Range(45,360);
            float OffsetAngle2 = UnityEngine.Random.Range(45, 360);
            innerRing.transform.Rotate(new Vector3 (OffsetAngle1, OffsetAngle2, 0));
            middleRing.transform.Rotate(new Vector3(0, OffsetAngle1, OffsetAngle2));
            outerRing.transform.Rotate(new Vector3( OffsetAngle1, 0, OffsetAngle2));
            Indicator.SetColor("_EmissionColor", new Color (255f, 0f, 0f));

        }

        private void Update()
        {
            if (!launch.Value) return;
            //if(!IsLocalPlayer) return;   
            if (Input.GetMouseButtonUp(0)) {
                TestAlignment();
                DeactivatePulse();
            }
            
        }

        private void DeactivatePulse()
        {
            InnerMat.SetFloat("Strength", 0);
            Duration = 0;

            middleMat.SetFloat("Strength", 0);
            Duration = 0;

            OuterMat.SetFloat("Strength", 0);
            Duration = 0;
        }

        // Das rotieren des äußeren Zahnrads dreht um die y Achse des 1 und 2. Rings, aber um die X-Achse des 3. Rings. 
        public void RotateRingOuterZahnrad(GameObject ring, float prog)
        {
            if (ring == innerRing) {
                if (Duration < MaxDur) {
                    InnerMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    OuterMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    Duration += Time.deltaTime;
                } else
                {
                    InnerMat.SetFloat("Strength", 1);
                }
                innerRing.transform.RotateAround(innerRing.transform.position, innerRing.transform.up, prog * innerRingRotationSpeed);
                outerRing.transform.RotateAround(outerRing.transform.position, outerRing.transform.right, prog * outerRingRotationSpeed);
            } else
                {
                if (Duration < MaxDur)
                {
                    OuterMat.SetFloat("Strength", Mathf.Lerp(0,1, Duration / MaxDur));
                    middleMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    Duration += Time.deltaTime;
                } else
                {
                    OuterMat.SetFloat("Strength", 1);
                }
                middleRing.transform.RotateAround(middleRing.transform.position, middleRing.transform.up, prog * middleRingRotationSpeed);
                outerRing.transform.RotateAround(outerRing.transform.position, outerRing.transform.right, prog * outerRingRotationSpeed);
            }

        }

        // Das rotieren des äußeren Zahnrads dreht um die x und Z Achse des 1 und 2. Rings, aber um die Z-Achse des 3. Rings. 
        public void RotateRingInnerZahnrad(GameObject ring, float prog)
        {
            if (ring == innerRing)
            {
               if (Duration < MaxDur) {
                    InnerMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    OuterMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    Duration += Time.deltaTime;
                } else
                {
                    InnerMat.SetFloat("Strength", 1);
                    OuterMat.SetFloat("Strength", 1);
                }
                innerRing.transform.RotateAround(innerRing.transform.position, innerRing.transform.right, prog * innerRingRotationSpeed);
                outerRing.transform.RotateAround(outerRing.transform.position, outerRing.transform.forward, prog *outerRingRotationSpeed);
            }
            else
            {
                if (Duration < MaxDur)
                {
                    OuterMat.SetFloat("Strength", Mathf.Lerp(0,1, Duration / MaxDur));
                    middleMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    Duration += Time.deltaTime;
                } else
                {
                    OuterMat.SetFloat("Strength", 1);
                    middleMat.SetFloat("Strength", 1);
                }
                middleRing.transform.RotateAround(middleRing.transform.position, middleRing.transform.forward, prog * middleRingRotationSpeed);
                outerRing.transform.RotateAround(outerRing.transform.position, outerRing.transform.forward, prog * outerRingRotationSpeed);
            }

        }

        private void TestAlignment()
        {

            Debug.Log(Vector3.Distance(InnerEmpty.position, OuterEmpty.position));
            if (Vector3.Distance(InnerEmpty.position, middleEmpty.position) <= _targetdistance1 + FairnessThreshold/100 && Vector3.Distance(middleEmpty.position, OuterEmpty.position) <= _targetdistance2 + FairnessThreshold / 100) EndGame();
            /*//Problem: Manchmal tackt das beim rotieren doch noch die angles slighty. Also wird die achse die bspw. nicht rotiert werden soll auf einmal -179,999 anstatt 0 bspw. Das könnte problematisch werden..
            Debug.Log(Quaternion.Angle(innerRing.transform.rotation, outerRing.transform.rotation).ToString());
            if (Quaternion.Angle(innerRing.transform.rotation, outerRing.transform.rotation) <= FairnessThreshold)
            {
                EndGame(); } */
        }

        private void EndGame()
        {
            Debug.Log("End");
            Indicator.SetColor("_EmissionColor", new Color(0f, 255f, 0f));
            FinishGame();
        }

        public override void LaunchGame()
        {
            base.LaunchGame();
            mesh.SetActive(true);
        }
        
    }
}
