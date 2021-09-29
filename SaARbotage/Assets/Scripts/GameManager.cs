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
        public VotingManager votingManager;

        [Header("Days:")] 
        public const int DAYSTOTAL = 3;
        public NetworkVariable<int> currentDay = new NetworkVariable<int>(0);
        public NetworkVariable<int> openStationsForDay = new NetworkVariable<int>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        private bool isInDaySettings = true;
        public List<Station> stations;
        public int[] timesForDays = new int[3];

        [Header("Oxygen:")]
        public NetworkVariable<float> syncTime = new NetworkVariable<float>();
        public float time;
        public float timeFailedStation;
        public float timeSucceedStation;
        public float timeManipulatedStationmodificator;
        public NetworkVariable<int> manipulatedStationCounter = new NetworkVariable<int>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone}, 0);
        public float timeManipulatedStationFactor;
        public bool _decreaseOxygen = false;
        
        [Header("Game Settings")]
        //[SerializeField]
        //public List<RoomSettings> stationsPerRoom;
        //public GameObject stationPrefab;
        //public GameObject roomHolder;
        //public GameObject roomPrefab;

        public GameObject[] gamePrefabs;
        public Dictionary<Room, List<Station>> rooms = new Dictionary<Room, List<Station>>();
        
        
        //public GameObject dummyScannedStation;

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
            currentDay.Settings.ReadPermission = NetworkVariablePermission.Everyone;

            // get initial room distribution

            var roomsObj = FindObjectsOfType<Room>();

            foreach (var room in roomsObj)
            {
                rooms.Add(room, new List<Station>());
                var stationFromRoom = room.gameObject.GetComponentsInChildren<Station>();
                foreach (var station in stationFromRoom)
                {
                    rooms[room].Add(station);
                }
            }
        }

        public void StartNewDay()
        {
            if(!IsHost) return;
            isInDaySettings = true;
            Debug.Log("### Start a new Day! ###");
            
            // reset day
            currentDay.Value++;
            
            // reset oxygen
            syncTime.Value = time;
            _decreaseOxygen = true;

            // reset androids
            // TODO let them manipulate again
            
            // reset games
            
            openStationsForDay.Value = 0;
            ResetGameIndexFromStations();
            foreach (var station in rooms.SelectMany(room => room.Value))
            {
                station.ResetNetworkVariables();
                if (station._isActive.Value)
                {
                    openStationsForDay.Value++;
                    
                }
            }
            
            isInDaySettings = false;
        }
        
        public void EndDay()
        {
            isInDaySettings = true;
            Debug.Log("### CALL END DAY HOST ###");
            _decreaseOxygen = false;
            timesForDays[currentDay.Value] = (int) syncTime.Value;
            if (currentDay.Value == DAYSTOTAL)
            {
                EndGame();
            }
            if(!IsHost) return;
            EndDayClientRpc();
            //votingManager.TriggerVoting();
        }

        [ClientRpc]
        public void EndDayClientRpc()
        {
            isInDaySettings = true;
            Debug.Log("### TRIGGERD VOTING FOR CLIENT");
            votingManager.gameObject.SetActive(true);
            votingManager.TriggerVoting();
        }

        public void EndGame()
        {
            // TODO Show highscore
            Debug.Log("### Game Finished! ###");
        }

        #region day logic helper

        private void ResetGameIndexFromStations()
        {
            // reset all game indexies to -1, which means inactive
            foreach (var station in rooms.SelectMany(room => room.Value))
            {
                station.gameIndex.Value = -1;
            }
            
            // distribute new games to stations
            var keys = rooms.Keys.ToList();
            for (var i = 0; i < gamePrefabs.Length; i++)
            {
                var randomRoom = keys[Random.Range(0, rooms.Keys.Count)];
                var stations = rooms[randomRoom];
                while (IsRoomFull(stations))
                {
                    randomRoom = keys[Random.Range(0, rooms.Keys.Count)];
                    stations = rooms[randomRoom];
                }
               
                var randomStation = stations[Random.Range(0, stations.Count)];
                while (randomStation.gameIndex.Value >= 0)
                {
                    randomStation = stations[Random.Range(0, stations.Count)];
                }

                randomStation.gameIndex.Value = i;
                
                // game 0 is specific and needs two stations
                if (i == 0)
                {
                    while (randomStation.gameIndex.Value >= 0)
                    {
                        randomStation = stations[Random.Range(0, stations.Count)];
                    }
                    randomStation.gameIndex.Value = i + 1;
                    i++;
                }
            }
        }
        

        private bool IsRoomFull(List<Station> stations)
        {
            foreach (var station in stations)
            {
                if (station.gameIndex.Value < 0)
                {
                    return false;
                }
            }

            return true;
        }


        #endregion
        
        
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
            
            SetUpGame();
            
        }

        private void Update()
        {
            if(!_decreaseOxygen) return;
            syncTime.Value -= (Time.deltaTime * (1+ manipulatedStationCounter.Value) * (1 + timeManipulatedStationFactor));

            if (!IsHost) return;
            if (!isInDaySettings && openStationsForDay.Value == 0)
            {
                EndDay();
            }

        }

        public void SetUpGame()
        {
            StartNewDay();
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

            //var gameObj = Instantiate(gamePrefabsMultiplayer[0], stationObj.GetComponent<Station>().vuforiaTargetObj.transform);
            //gameObj = stationObj.GetComponentInChildren<Game>().gameObject;
            //gameObj.GetComponent<Game>().Setup(stationObj.GetComponent<Station>().stationId.Value);
            //gameObj.GetComponent<NetworkObject>().Spawn();
            
        }



        public void PlayGame()
        {
            dummyScannedStation.GetComponent<Station>().StartGame();
        }*/
    

        #region Oxygen and ui

        public void TimeFailedStation()
        {
            ChangeTime(timeFailedStation);
        }

        public void TimeSucceedStation()
        {
            ChangeTime(timeSucceedStation);
        }

        /// <summary>
        /// if b, then a new station got manipulated.
        /// if not b, then a station got freed from manipulation
        /// </summary>
        /// <param name="b"></param>
        public void TimeManipulatedStation(bool b)
        {
            if (b) manipulatedStationCounter.Value++;
            else manipulatedStationCounter.Value--;
        }
        
        private void ChangeTime(float value)
        {
            syncTime.Value += value;
        }

        #endregion
        
        private void AssignRoles()
        {
            var numPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;
            if (numPlayers <= 1) return;
            var numCrew = 0;
            var numAndroid = 0;
            
            // 20% of all players are androids
            numAndroid = (int)Mathf.Ceil((float) (numPlayers * 0.2));
            numCrew = numPlayers - numAndroid;
            Debug.Log("Android: " + numAndroid + ", Crew: " + numCrew + ", Players: " + numPlayers);

            var playersList = players.Keys.ToList();

            for (var i = numAndroid; i > 0; i --)
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
