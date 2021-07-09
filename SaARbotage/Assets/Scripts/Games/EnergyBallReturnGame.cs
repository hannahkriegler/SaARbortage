using System.Collections;
using System.Collections.Generic;
using SaARbotage;
using UnityEngine;

namespace SaARbotage
{
    public class EnergyBallReturnGame : Game
    {

        private GameObject capsule;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("EnergyBall")) return;
            capsule = GameObject.FindWithTag("EnergyBallParent");
            capsule.transform.SetParent(gameObject.transform);
            //finishButton.gameObject.SetActive(true);
            //B_targetHolding = other.transform;
            FinishGame(true);
        }
        
        public override void FinishGame(bool successful)
        {
            //When the finishButton is pressed. 
            // The cannister will be positioned as child of the target. 
            //capsule.transform.parent = _targetHolding;
            //capsule.transform.localPosition = _mypos;
            capsule.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //endmessage.gameObject.SetActive(true);
            //_finished = true;
            base.FinishGame(true);
        }
    }
}
