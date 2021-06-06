using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace SaARbotage
{
    public class Game : NetworkBehaviour
    {
        public List<Player> players;
        public int requiredPlayers;
        public bool launch = false;
        private int _registerdPlayers = 0;

        public virtual void Start()
        {
            players = new List<Player>();
        }
        
        public virtual void LaunchGame()
        {
            launch = true;
            Debug.Log("Launch Game!!");
        }
        
        public void RegisterPlayer(Player player)
        {
            Debug.Log("Register player: " + player.playerId);
            RegisterPlayerTestServerRpc(player.playerId);
            Debug.Log("Done");
        }

        [ServerRpc(RequireOwnership = false)]
        void RegisterPlayerTestServerRpc(ulong clientID)
        {
            Debug.Log("StartedServer RPC");
            RegisterPlayersClientRpc(clientID);
            Debug.Log("Finished ClientRPC");
            var player = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
            if (players.Contains(player.gameObject.GetComponent<Player>()))
            {
                Debug.Log("Player already registered");
                return;
            }
            
            players.Add(player.gameObject.GetComponent<Player>());
            Debug.Log("FinishedServer RPC");
            
        }

        [ClientRpc]
        void RegisterPlayersClientRpc(ulong clientID)
        {
            Debug.Log("registerd client: " + clientID);
            _registerdPlayers++;
            
            if (requiredPlayers == _registerdPlayers)
            {
                LaunchGame();
            }
            Debug.Log("Finished Client RPC");
        }
    }
}
