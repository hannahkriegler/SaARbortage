using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.NetworkVariable;
using Unity.VisualScripting;
using UnityEngine;

namespace SaARbotage
{
    [Serializable]
    public class Station : NetworkBehaviour
    {
        public NetworkVariable<int> stationId;
        public NetworkVariable<string> stationName;
        private NetworkVariable<bool> _isActive;
        private NetworkVariable<bool> _isDone;
        private NetworkVariable<bool> _isManipulated;
        private Room _room;
        private Item[] _items;
        //private Game _game;
        private NetworkVariable<int> _failures;

        public void Setup(Room room, int stationNumber)
        {
            stationId.Value = room.roomId.Value * 10 + stationNumber;
            gameObject.name += stationId.Value;
        }

        public void StartGame()
        {
            gameObject.GetComponentInChildren<Game>().RegisterPlayer();
        }

        public void ScanStation()
        {
            // TODO Enable station UI
            
            Debug.Log("Scanned Station " + stationId.Value);
        }

        /*public Station(int stationId, string stationName, bool isActive, Room room,
            Item[] items, Game game, int failures = 0, bool isDone = false, bool isManipulated = false)
        {
            this.stationId.Value = stationId;
            this.stationName.Value = stationName;
            _isActive.Value = isActive;
            _isDone.Value = isDone;
            _isManipulated.Value = isManipulated;
            _room = room;
            _items = items;
            _game = game;
            _failures.Value = failures;
        }*/
        
    }
}
