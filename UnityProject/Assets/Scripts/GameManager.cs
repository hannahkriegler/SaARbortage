using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

        [Header("Game Settings")]
        [SerializeField]
        public List<RoomSettings> stationsPerRoom;
        public Station[] stationPrefabs;
        public GameObject roomHolder;
        public GameObject roomPrefab;

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
            if (IsHost)
            {
                foreach (var roomSettingse in stationsPerRoom)
                {
                    // create new room
                    var roomObj = Instantiate(roomPrefab, roomHolder.transform, true);
                    //Room room = roomObj.AddComponent(typeof(Room)) as Room;
                    /*
                    // fill room with station
                    for (int i = 0; i < roomSettingse.stations; i++)
                    {
                        var stationIndex = Random.Range(0, stationPrefabs.Length - 1);
                        var stationObj = Instantiate(stationPrefabs[stationIndex], roomObj.transform, true);
                        var station = stationObj.GetComponent<Station>();
                        
                        // get a game for the station
                        // TODO
                    }*/
                    //room = new Room(roomSettingse.GetHashCode(), roomSettingse.name, );
                }
            }
            
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
    
    [System.Serializable]

    public class RoomSettings
    {
        public string name;
        public int stations;
    }

}
