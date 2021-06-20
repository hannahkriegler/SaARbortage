using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage
{
    public class Oxygen : NetworkBehaviour
    {
        public Image oxygenBar;
        public float _timeLeft;
        private float _fillAmount;
        private float _onePercent;
        private float _timeTotal;
    
        private void Start()
        {
           _timeLeft = GameManager.Instance.time;
           _timeTotal = GameManager.Instance.time;
           if(IsHost)
               gameObject.GetComponent<NetworkObject>().Spawn();
        }

        public void ChangeTime(float value)
        {
            ChangeTimeServerRpc(value);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void ChangeTimeServerRpc(float value)
        {
            Debug.Log("Decrease Time Server");
            if (true)
            {
                Debug.Log("Im the host!");
                //_timeLeft += value;
                //synchTime.Value += value;
                ChangeTimeClientRpc(value);
            }
        }

        [ClientRpc]
        private void ChangeTimeClientRpc(float value)
        {
            if(IsHost) return;
            Debug.Log("Decrease Time Client " + OwnerClientId);
            _timeLeft += value;
        }

        private void Update()
        {
            //_timeLeft -= Time.deltaTime;
            _fillAmount = _timeLeft / _timeTotal ;
            oxygenBar.fillAmount = _fillAmount;
        }
    }
}
