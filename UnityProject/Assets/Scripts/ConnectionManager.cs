using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;
using UnityEngine.UI;

namespace SaARbotage
{
    public class ConnectionManager : MonoBehaviour
    {
        public static ConnectionManager instance;
        
        public GameObject lobbyUI;
        public List<GameObject> playerUIs;
        private int _indexLobbyUI = 0;

        private void Awake()
        {
            instance = this;
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

        public void AddPlayerToLobby(Player player)
        {
            var box = playerUIs[_indexLobbyUI];
            box.SetActive(true);
            box.GetComponentInChildren<Text>().text = player.gameObject.name;
            _indexLobbyUI++;
        }
    }
}
