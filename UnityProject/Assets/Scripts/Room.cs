using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace SaARbotage
{
    public class Room : NetworkBehaviour
    {
        public NetworkVariable<int> roomId = new NetworkVariable<int>();
        public string roomName;
        public Station[] stations;
        private Player[] _players;
        private bool _isClosed;
        private Roomtask _roomtask;

        public void Setup()
        {
            Debug.Log("Start Setup");
            roomId.Settings.WritePermission = NetworkVariablePermission.Everyone;
            roomId.Settings.ReadPermission = NetworkVariablePermission.Everyone;
            Debug.Log("End Setup");
        }
        
        /*public Room(int roomId, string roomName, Station[] stations, bool isClosed = false, Roomtask roomtask = null)
        {
            //this.roomId.Value = roomId;
            //this.roomId.Value = roomId;
            this.roomName = roomName;
            this.stations = stations;
            _isClosed = isClosed;
            _roomtask = roomtask;
        }*/
    }
}
