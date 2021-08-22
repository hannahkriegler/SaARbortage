using System.Collections;
using System.Collections.Generic;
using SaARbotage;
using UnityEngine;

namespace SaARbotage
{
    public class EnergyBallReturnGame : Game
    {
        private EnergyBallGame _firstPart;

        protected override void SetupGame()
        {
            base.SetupGame();
            _firstPart = GameObject.FindObjectOfType<EnergyBallGame>();
        }

        private GameObject capsule;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("EnergyBall")) return;
            
            FinishGame(true);
            _firstPart.FinishGame(true);
        }
        
        public override void FinishGame(bool successful)
        {
            // ehm this is kinda not needed..
            launch.Value = false;
            capsule = GameObject.FindWithTag("EnergyBallParent");
            capsule.transform.SetParent(gameObject.transform);
            capsule.transform.position = new Vector3(0,0,0.0012f);
            //finishButton.gameObject.SetActive(true);
            //B_targetHolding = other.transform;
            
            //When the finishButton is pressed. 
            // The cannister will be positioned as child of the target. 
            //capsule.transform.parent = _targetHolding;
            //capsule.transform.localPosition = _mypos;
            capsule.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _firstPart.FinishGame(successful);
            //endmessage.gameObject.SetActive(true);
            //_finished = true;
            base.FinishGame(successful);
        }
    }
}
