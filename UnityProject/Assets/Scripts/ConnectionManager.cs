using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
using UnityEngine.UI;

namespace SaARbotage
{
    public class ConnectionManager : NetworkBehaviour
    {
        public static ConnectionManager instance;
        
        public GameObject lobbyUI;
        public List<GameObject> playerUIs;
        private int _indexLobbyUI = 0;
        public Dictionary<ulong, string> _playersInLobby;
        public int playersCounter;

        private void Awake()
        {
            instance = this;
            _playersInLobby = new Dictionary<ulong, string>();
            playersCounter = 0;
        }

        public void Host()
        {
            lobbyUI.SetActive(true);
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.StartHost(Vector3.zero, Quaternion.identity);
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback)
        {
            Debug.Log("Approving Connection");
            callback(true, null, true, Vector3.zero, Quaternion.identity);
        }

        public void Join()
        {
            NetworkManager.Singleton.StartClient();
        }

        /// <summary>
        /// Clients (NOT SERVER ITSELF) call this method after a successful connection. It is executed only on the server.
        /// Therefore, the server broadcast the new information to all clients
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="playerName"></param>
        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerToLobbyServerRpc(ulong playerID, String playerName)
        {
            if (!_playersInLobby.ContainsKey(playerID))
                _playersInLobby.Add(playerID, playerName);
            
            // broadcast it to all players
            foreach (var id in _playersInLobby)
            {
                AddPlayerToLobbyClientRpc(id.Key, id.Value);
            }
            UpdateLobby();
        }

        /// <summary>
        /// Gets called from a server on all registered clients, but not the server itself
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="playerName"></param>
        [ClientRpc]
        private void AddPlayerToLobbyClientRpc(ulong clientID, String playerName)
        {
            AddPlayerToLobby(clientID, playerName);
        }

        
        public void AddPlayerToLobby(ulong clientID, String playerName)
        {
            if (_playersInLobby.ContainsKey(clientID)) return;
            _playersInLobby.Add(clientID, playerName);
            UpdateLobby();
        }

        /// <summary>
        /// is called  locally by each client, can create lobby screen from _playersInLobby dictonary
        /// </summary>
        void UpdateLobby()
        {
            lobbyUI.SetActive(true);
            _indexLobbyUI = 0;
            foreach (var id in _playersInLobby)
            {
                var b = playerUIs[_indexLobbyUI];
                b.SetActive(true);
                b.GetComponentInChildren<Text>().text = _playersInLobby[id.Key];
                _indexLobbyUI++;
            }
            
        }
        
        
        
    }
}
