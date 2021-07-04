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
        public GameObject stationPrefab;
        public GameObject roomHolder;
        public GameObject roomPrefab;

        public GameObject[] gamePrefabsMultiplayer;


        public GameObject dummyScannedStation;

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
                for (int i = 0; i < stationsPerRoom.Count; i++)
                {
                    // spawn new rooms with stations
                    SpawnRoomsServerRpc(i, stationsPerRoom[i].name, stationsPerRoom[i].stations, stationsPerRoom[i].status);
                }
                
                // start counter
                InvokeRepeating(nameof(UpdateOxygen), 1, 1);
            }
        }

        [ServerRpc]
        private void SpawnRoomsServerRpc(int roomId, string roomName, int numStations, bool stationStatus)
        {
            var roomObj = Instantiate(roomPrefab, roomHolder.transform, true);
            roomObj.GetComponent<NetworkObject>().Spawn();
            roomObj.GetComponent<Room>().Setup(roomId, roomName, numStations);

            for (int i = 0; i < numStations; i++)
            {
                SpawnStationsPerRoom(roomObj, i, stationStatus);
            }
            
        }
    
        private void SpawnStationsPerRoom(GameObject room, int stationNumber, bool status)
        {
            var stationObj = Instantiate(stationPrefab, room.transform, true);
            stationObj.GetComponent<NetworkObject>().Spawn();
            stationObj.GetComponent<Station>().Setup(room.GetComponent<Room>(), stationNumber, status);
            
            if(!status) return;

            var gameObj = Instantiate(gamePrefabsMultiplayer[0], stationObj.GetComponent<Station>().vuforiaTargetObj.transform, true);
            gameObj = stationObj.GetComponentInChildren<Game>().gameObject;
            gameObj.GetComponent<Game>().Setup(stationObj.GetComponent<Station>().stationId.Value);
            gameObj.GetComponent<NetworkObject>().Spawn();
            
        }

        public void ScanStation()
        {
            var allStations = FindObjectsOfType<Station>();
            foreach (var staton in allStations)
            {
                if (staton.stationId.Value == 0)
                {
                    staton.ScanStation();
                    dummyScannedStation = staton.gameObject;
                }
            }
        }

        public void PlayGame()
        {
            dummyScannedStation.GetComponent<Station>().StartGame();
            
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
        public bool status;
    }

}
