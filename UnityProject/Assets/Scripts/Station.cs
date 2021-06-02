using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public class Station : MonoBehaviour
    {
        public int stationId;
        public string stationName;
        private bool _isActive;
        private bool _isDone;
        private bool _isManipulated;
        private Room _room;
        private Item[] _items;
        private Game _game;
        private int _failures;

        public Station(int stationId, string stationName, bool isActive, Room room,
            Item[] items, Game game, int failures = 0, bool isDone = false, bool isManipulated = false)
        {
            this.stationId = stationId;
            this.stationName = stationName;
            _isActive = isActive;
            _isDone = isDone;
            _isManipulated = isManipulated;
            _room = room;
            _items = items;
            _game = game;
            _failures = failures;
        }
    }
}
