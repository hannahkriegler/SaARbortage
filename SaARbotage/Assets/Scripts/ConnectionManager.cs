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
        
        public List<GameObject> playerUIs;
        private int _indexLobbyUI = 0;
        public Dictionary<ulong, string> _playersInLobby;
        public int playersCounter;

        [Header("Menu Objects")] 
        public GameObject buttonHost;
        public GameObject buttonJoin;
        public GameObject buttonCreateLobby;
        public GameObject lobbyUI;
        public InputField inputField;

        private void Awake()
        {
            instance = this;
            _playersInLobby = new Dictionary<ulong, string>();
            playersCounter = 0;
            
            buttonHost.SetActive(true);
            buttonJoin.SetActive(true);
            buttonCreateLobby.SetActive(false);
            lobbyUI.SetActive(false);
        }

        public void Host()
        {
            lobbyUI.SetActive(true);
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.StartHost(Vector3.zero, Quaternion.identity);
            buttonHost.SetActive(false);
            buttonJoin.SetActive(false);
            buttonCreateLobby.SetActive(true);
            inputField.gameObject.SetActive(false);
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback)
        {
            Debug.Log("Approving Connection");
            callback(true, null, true, Vector3.zero, Quaternion.identity);
        }

        public void Join()
        {
            NetworkManager.Singleton.StartClient();
            inputField.gameObject.SetActive(false);
            buttonHost.SetActive(false);
            buttonJoin.SetActive(false);
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

        /*public void CreateLobby()
        {
            Debug.Log("Create Lobby");
            var players = new Dictionary<Player, ulong>();
            // give players id's
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                var player = NetworkManager.Singleton.ConnectedClients[client.Value.ClientId].PlayerObject;
                player.gameObject.GetComponent<Player>().playerId = client.Value.ClientId;
                // add player to dict
                players.Add(player.gameObject.GetComponent<Player>(), client.Value.ClientId);
                Debug.Log("Added Player with " + client.Value.ClientId);
            }
            
            // do role logic
            AssignRoles();
            // give players roles
            foreach (var client in NetworkManager.ConnectedClientsList)
            {
                
            }

            //cam.SetActive(false);
            // Launch custom camera foreach player
            foreach (var pair in players)
            {
                pair.Key.ShowUI(true);
            }*/
        
    }
}
