using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace SaARbotage
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        public Dictionary<Player, ulong> players;
        public GameObject cam;
        public float time;
        

        public void Awake()
        {
            Instance = this;
        }
        
        [ClientRpc]
        public void CreateLobbyClientRpc()
        {
            Debug.Log("Create Lobby");
            players = new Dictionary<Player, ulong>();
            // give players id's
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                var player = NetworkManager.Singleton.ConnectedClients[client.Value.ClientId].PlayerObject;
                player.gameObject.GetComponent<Player>().playerId = client.Value.ClientId;
                // add player to dict
                players.Add(player.gameObject.GetComponent<Player>(), client.Value.ClientId);
                Debug.Log("Added Player with " + client.Value.ClientId);
            }

            cam.SetActive(false);
            // Launch custom camera foreach player
            foreach (var pair in players)
            {
                pair.Key.ShowUI(true);
            }
            
        }
    }

}
