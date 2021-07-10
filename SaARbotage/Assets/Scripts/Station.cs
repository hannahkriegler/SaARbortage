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
        private NetworkVariable<bool> _isCurrentlyPlaying;
        private bool _isInCooldown;
        private Room _room;
        private Game _game;
        private Item[] _items;
        private StationStatus _stationStatus;
        private enum StationStatus
        {
            Active, Inactive, Manipulated, Failed, Cooldown
        }

        //public GameObject vuforiaTargetObj;
        private NetworkVariable<int> _failures;

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
        

        private void Start()
        {
            _isManipulated = new NetworkVariable<bool>(false);
            _isActive = new NetworkVariable<bool>();
            _isManipulated.OnValueChanged += UpdateStationUi;
            _isActive.OnValueChanged += UpdateStationUi;
            _game = GetComponentInChildren<Game>();
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
            _game.registerdPlayers.Value++;

            if (_game.requiredPlayers == _game.registerdPlayers)
            {
                uiWaitForPlayersPanel.SetActive(false);
                _game.waitForPlayersToRegister.Value = false;
                uiStationPanel.SetActive(false);
                _game.LaunchGame();
            }
            else
            {
                
            }

            if (_game.waitForPlayersToRegister.Value)
            {
                uiWaitForPlayersPanel.SetActive(true);
                
            }
        }

        IEnumerator WaitUntilAllPlayersAreRegistered()
        {
            uiWaitForPlayersPanel.SetActive(true);
            yield return new WaitUntil(_game.registerdPlayers.Value == _game.requiredPlayers.Value);
            while (_game.registerdPlayers.Value < _game.requiredPlayers.Value)
            {
                
            }
            uiWaitForPlayersPanel.SetActive(false);
            _game.waitForPlayersToRegister.Value = false;
            uiStationPanel.SetActive(false);
            _game.LaunchGame();
            yield break;
        }

        public void FinishedGame(bool successful)
        {
            ShowCooldownUi(false);
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
                StationStatus.Failed => "Failed",
                _ => uiStationStatus.text
            };
        }

        
        
        public void ScanStation()
        {
            uiStationPanel.SetActive(true);

            // Station is not active
            if (!_isActive.Value)
            {
                uiStationInfoPanel.SetActive(true);
                uiGameInfoPanel.SetActive(false);
                
                // set all to false, and only enable the one that is needed:
                gameInstructionPanel.SetActive(false);
                uiWaitForPlayersPanel.SetActive(false);
                uiCooldownPanel.SetActive(false);
                uiStatusPanel.SetActive(false);
                uiPlayedByAnotherPanel.SetActive(false);
            }
            else
            {
                uiGameInfoPanel.SetActive(true);
                // Station is active - someone is playing already
                if (_isCurrentlyPlaying.Value)
                {
                    uiPlayedByAnotherPanel.SetActive(true);
                }
                // Station is active, game is finished successfully
                else if (_isDone.Value)
                {
                    uiStatusPanel.SetActive(true);
                }
                // Station is active, game is on cooldown because it failed previously
                else if (_isInCooldown)
                {
                    uiCooldownPanel.SetActive(true);
                }
                // Station is active, no one plays / it requires players
                else
                {
                    gameInstructionPanel.SetActive(true);
                }
            }

            // Station is manipulated
            if (_isManipulated.Value)
            {
                
            }
            
            
            if (_game.GetType() == typeof(EnergyBallReturnGame))
            {
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
            if (b)
            {
                uiStationPanel.SetActive(true);
            }
            uiGameInfoPanel.SetActive(!b);
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
