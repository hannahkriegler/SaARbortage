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
        public NetworkVariable<bool> _isActive = new NetworkVariable<bool>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public NetworkVariable<bool> _isDone = new NetworkVariable<bool>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public NetworkVariable<bool> _isManipulated = new NetworkVariable<bool>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public NetworkVariable<bool> _isCurrentlyPlaying = new NetworkVariable<bool>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public bool _iCurrentlyPlayIt = false;
        public bool _isInCooldown;
        private Room _room;
        private Game _game;
        private Item[] _items;
        private StationStatus _stationStatus;
        private enum StationStatus
        {
            Active, // station is active for this day
            Inactive, // station is not active for this day
            Manipulated, // station is manipulated
            Completed, // station is active for this day and completed
            Cooldown // the current player tried the station, but failed
        }

        private NetworkVariable<int> _failures = new NetworkVariable<int>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});

        [Header("UI")] public GameObject uiStationPanel;

        public GameObject uiStationInfoPanel;
        public Text uiStationTitel;
        public Text uiStationInfo;
        public Text uiStationStatus;
        
        
        [Header("GameInfo UI")]
        public GameObject uiGameInfoPanel;
        [Header("GameInfo UI")]
        public GameObject gameInstructionPanel;
        public GameObject uiStartGameButton;
        
        [Header("Wait For Players")]
        public GameObject uiWaitForPlayersPanel;
        
        [Header("Cooldown UI")] 
        public GameObject uiCooldownPanel;
        public Text uiCooldownValue;
        
        [Header("Game Finished Status UI")] 
        public GameObject uiStatusPanel;

        [Header("Game Played by Another Player")]
        public GameObject uiPlayedByAnotherPanel;

        [Header("Station is not Active")] 
        public GameObject uiStationNotActivePanel;
        
        [Header("Station is Manipulated")] 
        public GameObject uiStationIsManipulatedPanel;

        private void Start()
        {
            _isManipulated.OnValueChanged += UpdateStationUi;
            _isActive.OnValueChanged += UpdateStationUi;
            _isDone.OnValueChanged += UpdateStationUi;
            _isCurrentlyPlaying.OnValueChanged += UpdateStationUi;
            _game = GetComponentInChildren<Game>();
            _isInCooldown = false;
            
            Setup(null, 0, true);
        }

        public void Setup(Room room, int stationNumber, bool status)
        {
            //stationId.Value = room.roomId.Value * 10 + stationNumber;
            gameObject.name += stationId.Value;

            uiStationTitel.text = gameObject.name;

            //_isActive.Value = status;
        }

        public void StartGame()  // run on Start button click
        {
            _game.registerdPlayers.Value++;

            if (_game.requiredPlayers.Value >= _game.registerdPlayers.Value)
            {
                uiWaitForPlayersPanel.SetActive(false);
                _game.waitForPlayersToRegister.Value = false;
                uiStationPanel.SetActive(false);
                _game.LaunchGame();
                _isCurrentlyPlaying.Value = true;
                _iCurrentlyPlayIt = true;
            }
            else 
            {
                uiWaitForPlayersPanel.SetActive(true);
                gameInstructionPanel.SetActive(false);
            }
        }

        IEnumerator WaitUntilAllPlayersAreRegistered()
        {
            uiWaitForPlayersPanel.SetActive(true);
            //yield return new WaitUntil(_game.registerdPlayers.Value == _game.requiredPlayers.Value);
            while (_game.registerdPlayers.Value < _game.requiredPlayers.Value)
            {
                // TODO
            }
            uiWaitForPlayersPanel.SetActive(false);
            _game.waitForPlayersToRegister.Value = false;
            uiStationPanel.SetActive(false);
            _game.LaunchGame();
            yield break;
        }

        public void FinishedGame(bool successful)
        {
            Debug.Log("### Station is called Finish ###" + successful);
            if (!successful)
                _isInCooldown = true;
            else
            {
                _isInCooldown = false;
                _isDone.Value = true;
            }
            ScanStation();
            
            if (!successful)
            {
                _failures.Value++;
            }
        }
        

        private void UpdateStationUi(bool previousValue, bool newValue)
        {
            uiStationStatus.text = _stationStatus switch
            {
                StationStatus.Active => "Active",
                StationStatus.Inactive => "Inactive",
                StationStatus.Manipulated => "Manipulated",
                StationStatus.Completed => "Completed",
                _ => uiStationStatus.text
            };
        }
        
        public void ScanStation()
        {
            uiStationPanel.SetActive(true);
            
            // set all to false, and only enable the one that is needed:
            gameInstructionPanel.SetActive(false);
            uiWaitForPlayersPanel.SetActive(false);
            uiCooldownPanel.SetActive(false);
            uiStatusPanel.SetActive(false);
            uiPlayedByAnotherPanel.SetActive(false);
            uiStationNotActivePanel.SetActive(false);
            uiStationIsManipulatedPanel.SetActive(false);

            // Station is not active
            if (!_isActive.Value)
            {
                uiStationInfoPanel.SetActive(true);
                uiGameInfoPanel.SetActive(true);
                uiStationNotActivePanel.SetActive(true);
            }
            else 
            {
                uiGameInfoPanel.SetActive(true);
                // Station is active - someone is playing already
                if (_isCurrentlyPlaying.Value)
                {
                    uiPlayedByAnotherPanel.SetActive(true);
                }
                // Station is active, game is finished successfully or not
                else if (_isDone.Value)
                {
                    uiStatusPanel.SetActive(true);
                    // TODO value here
                }
                // Station is active, game is on cooldown because it failed previously
                else if (_isInCooldown)
                {
                    uiCooldownPanel.SetActive(true);
                }
                // Station is active and manipulated
                else if (_isManipulated.Value)
                {
                    uiStationIsManipulatedPanel.SetActive(true);
                }
                // Station is active, no one plays / it requires players
                else
                {
                    gameInstructionPanel.SetActive(true);
                }
            }

            if (_iCurrentlyPlayIt)
            {
                uiStationPanel.SetActive(false);
            }


                if (_game is EnergyBallReturnGame && (!_isInCooldown && !_isManipulated.Value))
            {
                //Debug.Log("###### HERE");
                uiGameInfoPanel.SetActive(false);
            }
        }


        #region Cooldown

        public void StartCooldownCounter()
        {
            ShowCooldownUi(true);
            StartCoroutine(Countdown(20));
        }

        private IEnumerator Countdown(float timeToWait)
        {
            while (timeToWait > 0)
            {
                timeToWait -= Time.deltaTime;
                uiCooldownValue.text = ((int) timeToWait).ToString() + "s";
            }
            _game.isOnCoolDown = false;
            ShowCooldownUi(false);
            yield break;
        }
        
        private void ShowCooldownUi(bool b)
        {
            uiCooldownPanel.SetActive(b);
        }

        #endregion

        
        public void ShowStatusUi(bool b)
        {
            uiGameInfoPanel.SetActive(!b);
            uiCooldownPanel.SetActive(b);

        }

    }
}
