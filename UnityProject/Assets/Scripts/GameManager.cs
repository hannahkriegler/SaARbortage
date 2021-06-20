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
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        public Dictionary<Player, ulong> players;
        public GameObject cam;
        
        public NetworkVariable<float> syncTime = new NetworkVariable<float>();

        [Header("Oxygen:")]
        public float time;


        public void Awake()
        {
            Instance = this;
        }

        public override void NetworkStart()
        {
            syncTime.Value = time;
            syncTime.Settings.WritePermission = NetworkVariablePermission.Everyone;
            syncTime.Settings.ReadPermission = NetworkVariablePermission.Everyone;
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
            
            //SetUpGame();
            
        }

        public void SetUpGame()
        {
            // distribute roles
            
            // distribute stations and games
            
            // start counter
            InvokeRepeating(nameof(UpdateOxygen), 1, 1);
        }

        #region Oxygen
        public void ChangeTime(float value)
        {
            syncTime.Value += value;
        }

        private void UpdateOxygen()
        {
            syncTime.Value --;
        }
        #endregion
    }

}
