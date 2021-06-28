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

        public void Setup()
        {
            launch.Value = false;
            registerdPlayers.Value = 0;
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
