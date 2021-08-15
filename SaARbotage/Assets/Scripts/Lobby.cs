using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage
{
    public class Lobby : NetworkBehaviour
    {
        public static int MINPLAYERNUMBERS = 1;
        [Header("References")] 
        [SerializeField] private PlayerCard[] _playerCards;

        [SerializeField] private Button startGameButton;
        
        private NetworkList<LobbyPlayerState> _lobbyPlayerStates = new NetworkList<LobbyPlayerState>();

        public override void NetworkStart()
        {
            if (IsClient)
            {
                _lobbyPlayerStates.OnListChanged += HandleLobbyPlayersStateChanged;
            }

            if (IsServer)
            {
                startGameButton.gameObject.SetActive(true);

                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    HandleClientConnected(client.ClientId);
                }
            }
        }

        private void OnDestroy()
        {
            _lobbyPlayerStates.OnListChanged -= HandleLobbyPlayersStateChanged;

            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
            }
        }

        private bool IsEveryoneReady()
        {
            if (_lobbyPlayerStates.Count < MINPLAYERNUMBERS)
            {
                return false;
            }

            foreach (var player in _lobbyPlayerStates)
            {
                if (!player.IsReady)
                {
                    return false;
                }
            }

            return true;
        }
        
        private void HandleClientConnected(ulong clientId)
        {
            //ServerGameNetPortal.
        }

        private void HandleClientDisconnected(ulong obj)
        {
            throw new System.NotImplementedException();
        }
        
        private void HandleLobbyPlayersStateChanged(NetworkListEvent<LobbyPlayerState> changeEvent)
        {
            
        }
    }
}
