using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

namespace SaARbotage
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager instance;

        public void Awake()
        {
            instance = this;
        }

        public void CreateLobby()
        {
            // give players id's
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                var player = NetworkManager.Singleton.ConnectedClients[client.Value.ClientId].PlayerObject;
                player.gameObject.GetComponent<Player>().playerId = client.Value.ClientId;
                Debug.Log("Added Player with " + client.Value.ClientId);
            }
            
        }
    }

}
