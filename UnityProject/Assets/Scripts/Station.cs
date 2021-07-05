using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.NetworkVariable;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage
{
    [Serializable]
    public class Station : NetworkBehaviour
    {
        public NetworkVariable<int> stationId;
        public NetworkVariable<string> stationName;
        public NetworkVariable<bool> _isActive;
        private NetworkVariable<bool> _isDone;
        private NetworkVariable<bool> _isManipulated;
        private Room _room;
        private Item[] _items;
        private StationStatus _stationStatus;
        private enum StationStatus
        {
            Active, Inactive, Manipulated, Failed
        }

        public GameObject vuforiaTargetObj;
        private NetworkVariable<int> _failures;

        [Header("UI")] public GameObject uiStationPanel;
        public Text uiStationTitel;
        public Text uiStationInfo;
        public Text uiStationStatus;
        public GameObject uiStartGameButton;
        public GameObject uiWaitObj;

        public GameObject uiGameInfoPanel;

        private void Start()
        {
            _isManipulated.OnValueChanged += UpdateStationUi;
            _isActive.OnValueChanged += UpdateStationUi;
        }

        public void Setup(Room room, int stationNumber, bool status)
        {
            stationId.Value = room.roomId.Value * 10 + stationNumber;
            gameObject.name += stationId.Value;

            uiStationTitel.text = gameObject.name;

            _isActive.Value = status;
        }

        public void StartGame()
        {
            var game = gameObject.GetComponentInChildren<Game>();
            game.RegisterPlayer();

            if (game.waitForPlayersToRegister.Value)
            {
                uiStationPanel.SetActive(false);
                uiWaitObj.SetActive(true);
                uiStartGameButton.SetActive(false);
            }
            else
            {
                uiStationPanel.SetActive(false);
                uiGameInfoPanel.SetActive(false);
            }
        }

        public void FinishedGame(bool successful)
        {
            if (!successful)
            {
                _failures.Value++;
            }
        }

        public void ScanStation()
        {
            Debug.Log("Scanned Station " + stationId.Value);
        }

        private void UpdateStationUi(bool previousValue, bool newValue)
        {
            uiStationStatus.text = _stationStatus switch
            {
                StationStatus.Active => "Active",
                StationStatus.Inactive => "Inactive",
                StationStatus.Manipulated => "Manipulated",
                StationStatus.Failed => "Failed",
                _ => uiStationStatus.text
            };
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
