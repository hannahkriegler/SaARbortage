using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public GameObject arCam;
        public GameObject uiConnection;
        
        public NetworkVariable<float> syncTime = new NetworkVariable<float>();
        public NetworkVariable<int> openStationsForDay = new NetworkVariable<int>();
        public List<Station> stations;

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
            //arCam.GetComponent<Camera>().enabled = false;
        }

        public override void NetworkStart()
        {
            syncTime.Value = time;
            syncTime.Settings.WritePermission = NetworkVariablePermission.Everyone;
            syncTime.Settings.ReadPermission = NetworkVariablePermission.Everyone;
            openStationsForDay.Settings.WritePermission = NetworkVariablePermission.Everyone;
            openStationsForDay.Settings.ReadPermission = NetworkVariablePermission.Everyone;
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
            
            // assign roles
            AssignRoles();
            
            
            uiConnection.SetActive(false);
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
                
                // Call Setup on all stations
                var id = 0;
                foreach (var station in FindObjectsOfType<Station>())
                {
                    //SetupStationServerRpc(station, id);
                    id++;
                }
            }
        }

        /*[ServerRpc]
        private void SetupStationServerRpc(Station station, int id)
        {
            station.Setup(null, id, true );
            SetupStationClientRpc(station, id);
        }

        [ClientRpc]
        private void SetupStationClientRpc(Station station, int id)
        {
            station.Setup(null, id, true );
        }*/

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

            //var gameObj = Instantiate(gamePrefabsMultiplayer[0], stationObj.GetComponent<Station>().vuforiaTargetObj.transform);
            //gameObj = stationObj.GetComponentInChildren<Game>().gameObject;
            //gameObj.GetComponent<Game>().Setup(stationObj.GetComponent<Station>().stationId.Value);
            //gameObj.GetComponent<NetworkObject>().Spawn();
            
        }



        public void PlayGame()
        {
            dummyScannedStation.GetComponent<Station>().StartGame();
            
        }
    

        #region Oxygen and ui
        public void ChangeTime(float value)
        {
            syncTime.Value += value;
        }

        private void UpdateOxygen()
        {
            syncTime.Value --;
        }
        
        #endregion
        
        private void AssignRoles()
        {
            var numPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;
            var numCrew = 0;
            var numAndroid = 0;
            
            // 20% of all players are androids
            numAndroid = (int)Mathf.Ceil((float) (numPlayers * 0.2));
            numCrew = numPlayers - numAndroid;
            Debug.Log("Android: " + numAndroid + ", Crew: " + numCrew + ", Players: " + numPlayers);

            var playersList = players.Keys.ToList();

            for (var i = numAndroid; i >= 0; i --)
            {
                var randomIndex = Random.Range(0, playersList.Count);
                var player = playersList[randomIndex];
                player.roleString.Value = "Android";
                playersList.Remove(player);
            }
            
            if(playersList.Count != numCrew)
                Debug.Log("WROONG player calculation!");

            foreach (var player in playersList)
            {
                player.roleString.Value = "Crew";
            }
            
        }
        
        
    }
    
    [System.Serializable]

    public class RoomSettings
    {
        public string name;
        public int stations;
        public bool status;
    }

}
