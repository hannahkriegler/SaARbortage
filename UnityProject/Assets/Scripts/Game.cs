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

        public void Setup(int stationID)
        {
            launch.Value = false;
            registerdPlayers.Value = 0;
            this.stationID.Value = stationID;
            //gameObject.transform.SetParent(station.vuforiaTargetObj.transform); 
        }

        private void OnEnable()
        {
            foreach (var station in FindObjectsOfType<Station>())
            {
                if (station.stationId.Value.Equals(stationID.Value))
                {
                    gameObject.transform.SetParent(station.vuforiaTargetObj.transform); 
                }
            }
        }
        

        public virtual void RegisterPlayer()
        {
            registerdPlayers.Value++;
            if (requiredPlayers.Value == registerdPlayers.Value)
            {
                LaunchGame();
            }
        }

        public virtual void LaunchGame()
        {
            launch.Value = true;
        }

        public virtual void FinishGame()
        {
            launch.Value = false;
        }

       
       
    }
}
