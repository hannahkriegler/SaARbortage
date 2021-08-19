using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage
{
    public class AlignGame : Game
    {

        public NetworkVariable<float> innerRingX = new NetworkVariable<float>();        
        public NetworkVariable<float> innerRingZ = new NetworkVariable<float>();


        public NetworkVariable<float> middleRingX = new NetworkVariable<float>();
        public NetworkVariable<float> middleRingZ = new NetworkVariable<float>();

        public NetworkVariable<float> outerRingX = new NetworkVariable<float>();
        public NetworkVariable<float> outerRingZ = new NetworkVariable<float>();

        private float _iRX;
        private float _iRZ;
        private float _mRX;
        private float _mRZ;
        private float _oRX;
        private float _oRZ;

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

        public float timeConstraint = 60f;
        public Text timerLeft;
        public Text timerRight;

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

        protected override void SetupGame()
        {
            base.SetupGame();
            if (IsHost)
            {
                Debug.Log("Giving permissions to everyone.");
                GivePermissions();
            }
            InnerEmpty = innerRing.transform.GetChild(0).transform;
            middleEmpty = middleRing.transform.GetChild(0).transform;
            OuterEmpty = outerRing.transform.GetChild(0).transform;
            //GivePermissions();
            SetUpAlign();
        }

        // Also hier werden die Winkel erstmals verstellt, dass es replayable is.
        private void SetUpAlign()
        {
            UnScrubScribe();
            ScrubScribe();

            // Changes the solution. So its not always the rings at normal horizontal level, but slightly different. There might be more solutions to it though.
            float OffsetAngl = UnityEngine.Random.Range(1, 359);
            innerRing.transform.Rotate(new Vector3(OffsetAngl, OffsetAngl, OffsetAngl));
            innerRingUp = innerRing.transform.up;
            middleRing.transform.Rotate(new Vector3(OffsetAngl, OffsetAngl, OffsetAngl));
            middleRingUp = outerRing.transform.up;
            outerRing.transform.Rotate(new Vector3(OffsetAngl, OffsetAngl, OffsetAngl));
            outerRingUP = outerRing.transform.up;
            Debug.Log("L�sung " + innerRing.transform.rotation.ToString());

            // Here we save the wanted distance of the empty target objects located at the inner Rings. 
            _targetdistance1 = Vector3.Distance(InnerEmpty.position, middleEmpty.position);
            _targetdistance2 = Vector3.Distance(middleEmpty.position, OuterEmpty.position);
            Debug.Log(_targetdistance1 + " " + _targetdistance2);
            
            // Here we randomly set the angle of the Rings.
            // This may need a pure rework for the network variables.
            float OffsetAngle1 = UnityEngine.Random.Range(45,90);
            innerRingX.Value +=  OffsetAngle1;
            innerRingZ.Value +=  OffsetAngle1;

            middleRingX.Value += OffsetAngle1;
            middleRingZ.Value += -OffsetAngle1;

            outerRingX.Value += -OffsetAngle1;
            outerRingZ.Value += -OffsetAngle1;

            Indicator.SetColor("_EmissionColor", new Color (255f, 0f, 0f));

        }

        private void ScrubScribe()
        {
            innerRingX.OnValueChanged += UpdateInnerX;
            innerRingZ.OnValueChanged += UpdateInnerZ;

            middleRingX.OnValueChanged += UpdateMiddleX;
            middleRingZ.OnValueChanged += UpdateMiddleZ;

            outerRingX.OnValueChanged += UpdateOuterX;
            outerRingZ.OnValueChanged += UpdateOuterZ;
        }

        private void UnScrubScribe()
        {
            innerRingX.OnValueChanged -= UpdateInnerX;
            innerRingZ.OnValueChanged -= UpdateInnerZ;

            middleRingX.OnValueChanged -= UpdateMiddleX;
            middleRingZ.OnValueChanged -= UpdateMiddleZ;

            outerRingX.OnValueChanged -= UpdateOuterX;
            outerRingZ.OnValueChanged -= UpdateOuterZ;
        }

        private void GivePermissions()
        {
            innerRingX.Settings.WritePermission = NetworkVariablePermission.Everyone;
            innerRingX.Settings.ReadPermission = NetworkVariablePermission.Everyone;

            innerRingZ.Settings.WritePermission = NetworkVariablePermission.Everyone;
            innerRingZ.Settings.ReadPermission = NetworkVariablePermission.Everyone;

            middleRingX.Settings.WritePermission = NetworkVariablePermission.Everyone;
            middleRingX.Settings.ReadPermission = NetworkVariablePermission.Everyone;
            middleRingZ.Settings.WritePermission = NetworkVariablePermission.Everyone;
            middleRingZ.Settings.ReadPermission = NetworkVariablePermission.Everyone;

            outerRingX.Settings.WritePermission = NetworkVariablePermission.Everyone;
            outerRingX.Settings.ReadPermission = NetworkVariablePermission.Everyone;
            outerRingZ.Settings.WritePermission = NetworkVariablePermission.Everyone;
            outerRingZ.Settings.ReadPermission = NetworkVariablePermission.Everyone;



        }

        private void Update()
        {
            if (!launch.Value) return;
            if (isOnCoolDown) return;
            UpdateRingRotation();
            TickingCountDown();  
            if (Input.GetMouseButtonUp(0)) {
                DeactivatePulse();
                TestAlignment();
                
            }
            
        }

        void UpdateRingRotation()
        {
            Vector3 innerRingRot = new Vector3(_iRX, 0f,_iRZ);
            Vector3 middleRingRot = new Vector3(_mRX, 0f, _mRZ);
            Vector3 outerRingRot = new Vector3(_oRX, 0f, _oRZ);
            innerRing.transform.rotation = Quaternion.Euler(innerRingRot);
            middleRing.transform.rotation = Quaternion.Euler(middleRingRot);
            outerRing.transform.rotation = Quaternion.Euler(outerRingRot);
        }

        void UpdateInnerX(float old, float newval)
        {
             _iRX = newval;
            
        }

        void UpdateInnerZ(float old, float newval)
        {
             _iRZ = newval;
        }

        void UpdateMiddleX(float old, float newval)
        {
             _mRX = newval;
        }

        void UpdateMiddleZ(float old, float newval)
        {
           _mRZ = newval;

        }

        void UpdateOuterX(float old, float newval)
        {
            _oRX = newval;
        }

        void UpdateOuterZ(float old, float newval)
        {
           _oRZ = newval;
        }

        private void TickingCountDown()
        {
            if (timeConstraint <= 0)
            {
                FinishGame(false);
                return;
            }
            var min = 0;
            var seconds = 0;
            if (timeConstraint >= 60)
            {
                min = (int) (timeConstraint / 60); 
            }
            seconds = (int)(timeConstraint % 60);

            timerLeft.text = (min.ToString() + ":" + seconds.ToString());
            timerRight.text = (min.ToString() + ":" + seconds.ToString());

            timeConstraint -= Time.deltaTime; 
            // der folgende Part ist für TextFelder da. 

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

        // Das rotieren des �u�eren Zahnrads dreht um die y Achse des 1 und 2. Rings, aber um die X-Achse des 3. Rings. 
        /*
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
                Debug.Log("Outer Ring: " + outerRing.transform.localRotation.x +  outerRing.transform.localRotation.y +  outerRing.transform.localRotation.z);
                outerRingX.Value = (int) (outerRing.transform.localRotation.x * 180);
                outerRingY.Value = (int) (outerRing.transform.localRotation.y* 180);
                outerRingZ.Value = (int) (outerRing.transform.localRotation.z* 180);
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

        // Das rotieren des �u�eren Zahnrads dreht um die x und Z Achse des 1 und 2. Rings, aber um die Z-Achse des 3. Rings. 
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
        */

        public void RotateZahnrad(Side side , float Rotationspeed)
        {
            if (side == Side.left)
            {
                if (Duration < MaxDur)
                {
                    InnerMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    middleMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    Duration += Time.deltaTime;
                }
                else
                {
                    InnerMat.SetFloat("Strength", 1);
                    middleMat.SetFloat("Strength", 1);
                }
                // Change the network variables affiliated with it and in Update change the rotation of the Rings I guess..
                //Note: Also need to change the network variables at the start of the minigame. 
                // Get the online Variables. 
                
                innerRingX.Value += Rotationspeed;
                innerRingZ.Value += Rotationspeed;
                middleRingX.Value += Rotationspeed;
                

            }
            else
            {
                if (Duration < MaxDur)
                {
                    OuterMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    middleMat.SetFloat("Strength", Mathf.Lerp(0, 1, Duration / MaxDur));
                    Duration += Time.deltaTime;
                }
                else
                {
                    OuterMat.SetFloat("Strength", 1);
                    middleMat.SetFloat("Strength", 1);
                }
                // Change the network variables affiliated with it and in Update change the rotation of the Rings I guess..
                //Note: Also need to change the network variables at the start of the minigame. 

                
                middleRingZ.Value += Rotationspeed;
                outerRingX.Value += Rotationspeed;
                outerRingZ.Value += Rotationspeed;
                
            }

        }

        private void TestAlignment()
        {
            Debug.Log(Vector3.Distance(InnerEmpty.position, OuterEmpty.position));
            if (Vector3.Distance(InnerEmpty.position, middleEmpty.position) <= _targetdistance1 + FairnessThreshold/100 && Vector3.Distance(middleEmpty.position, OuterEmpty.position) <= _targetdistance2 + FairnessThreshold / 100) FinishGame(true);
            /*//Problem: Manchmal tackt das beim rotieren doch noch die angles slighty. Also wird die achse die bspw. nicht rotiert werden soll auf einmal -179,999 anstatt 0 bspw. Das k�nnte problematisch werden..
            Debug.Log(Quaternion.Angle(innerRing.transform.rotation, outerRing.transform.rotation).ToString());
            if (Quaternion.Angle(innerRing.transform.rotation, outerRing.transform.rotation) <= FairnessThreshold)
            {
                EndGame(); } */
        }


        public override void FinishGame(bool successful)
        {
            if (successful)
            {
                //Debug.Log("End");
                // TODO: Was passiert hier, wenn beide es gleichzeitig finishen?? Gibt das Probleme??
                //TODO: Auch hier sollte noch eine UI (am Besten für alle Games so wie GameUI) aufploppen mit einer Erklärung, dass man verloren hat, nun gesperrt ist fr die Station und ein Button mit okay oderso. 
                Indicator.SetColor("_EmissionColor", new Color(0f, 255f, 0f));
            } else
            {
                Indicator.SetColor("_EmissionColor", new Color(255f, 0f, 0f));
                //Debug.Log("Lost");
            }
            base.FinishGame(successful);
        }

        public override void LaunchGame()
        {
            base.LaunchGame();
            //mesh.SetActive(true);
        }
        
    }
}
