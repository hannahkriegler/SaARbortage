using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public class Room : MonoBehaviour
    {
        public int roomId;
        public string roomName;
        public Station[] stations;
        private Player[] _players;
        private bool _isClosed;
        private Roomtask _roomtask;

        public Room(int roomId, string roomName, Station[] stations, bool isClosed = false, Roomtask roomtask = null)
        {
            this.roomId = roomId;
            this.roomName = roomName;
            this.stations = stations;
            _isClosed = isClosed;
            _roomtask = roomtask;
        }
    }
}
