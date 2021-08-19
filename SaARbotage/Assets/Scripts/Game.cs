using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace SaARbotage
{
    public class Game : NetworkBehaviour
    {
        //public List<Player> players;
        public NetworkVariable<int> requiredPlayers;
        public NetworkVariable<bool> launch ;
        public NetworkVariable<int> registerdPlayers;
        public NetworkVariable<int> stationID;
        public NetworkVariable<bool> waitForPlayersToRegister;

        private Station _station;
        public bool isOnCoolDown;
        

        public void Start()
        {
            Debug.Log("Start() Game");
            launch.Value = false;
            registerdPlayers.Value = 0;
            _station = transform.parent.GetComponent<Station>();
            this.stationID.Value = _station.stationId.Value;
            waitForPlayersToRegister.Value = true;
            
            if (_station == null)
            {
                Debug.Log("No station was found for: " + gameObject.name);
            }
            
            SetupGame();
            Debug.Log("End Start() Game");
        }

        protected virtual void SetupGame()
        {
            
        }

        private void OnEnable()
        {
            foreach (var station in FindObjectsOfType<Station>())
            {
                if (station.stationId.Value.Equals(stationID.Value))
                {
                    //gameObject.transform.SetParent(station.vuforiaTargetObj.transform); 
                }
            }
        }
        

        public void RegisterPlayer()
        {
            registerdPlayers.Value++;
            if (requiredPlayers.Value == registerdPlayers.Value)
            {
                //Debug.Log("Register Player: " + registerdPlayers.Value + requiredPlayers.Value);
                waitForPlayersToRegister.Value = false;
                _station.uiStationPanel.SetActive(false);
                LaunchGame();
            }
        }

        public virtual void LaunchGame()
        {
            launch.Value = true;
        }

        public virtual void FinishGame(bool successful)
        {
            launch.Value = false;
            _station.FinishedGame(successful);
            isOnCoolDown = !successful;
        }

       
       
    }
}
