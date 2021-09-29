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

namespace SaARbotage
{
    [Serializable]
    public class Station : NetworkBehaviour
    {
        public NetworkVariable<int> stationId = new NetworkVariable<int>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.OwnerOnly});
        public NetworkVariable<string> stationName = new NetworkVariable<string>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.OwnerOnly});
        public NetworkVariable<bool> _isActive = new NetworkVariable<bool>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public NetworkVariable<bool> _isDone = new NetworkVariable<bool>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public NetworkVariable<bool> _isManipulated = new NetworkVariable<bool>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public NetworkVariable<bool> _isCurrentlyPlaying = new NetworkVariable<bool>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public NetworkVariable<int> gameIndex = new NetworkVariable<int>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public bool _iCurrentlyPlayIt = false;
        public bool _isInCooldown;
        private Room _room;
        public Game _game;
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

        public Text uiTaskName;
        public Text uiTaskDescription;

        public Image sabotagefill_Instruction;
        List<Coroutine> sabotages = new List<Coroutine>();
        public Image sabotagefill_repair;
        List<Coroutine> repairs = new List<Coroutine>();
        
        
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

        private InformationCanvasControl _infoCanvas;

        private void Start()
        {
            _isManipulated.OnValueChanged += UpdateStationUi;
            _isActive.OnValueChanged += UpdateStationUi;
            _isDone.OnValueChanged += UpdateStationUi;
            _isCurrentlyPlaying.OnValueChanged += UpdateStationUi;
            _game = GetComponentInChildren<Game>();
            
            _isInCooldown = false;

            gameIndex.OnValueChanged += ResetStation;
            
            Setup(null, 0, true);
        }

        

        public void Setup(Room room, int stationNumber, bool status)
        {
            //stationId.Value = room.roomId.Value * 10 + stationNumber;
            gameObject.name += stationId.Value;

            uiStationTitel.text = gameObject.name;

            //_isActive.Value = status;
        }

        public void ResetNetworkVariables()
        {
            // if game index > 0, this station has a playable game. otherwise this station will be inactive
            if (gameIndex.Value < 0)
            {
                _isActive.Value = false;
            }
            else
            {
                _isActive.Value = true;
                _isDone.Value = false;
                _isManipulated.Value = false;
                _isCurrentlyPlaying.Value = false;
                _iCurrentlyPlayIt = false;
                _isInCooldown = false;
            }
        }

        private void ResetStation(int previousvalue, int newvalue)
        {
            Debug.Log("Resetting client");
            if (_game != null)
            {
                Destroy(_game.gameObject);
                _game = null;
            }

            StartCoroutine(ResertGameInformation());
            
        }

        private IEnumerator ResertGameInformation()
        {
            if(gameIndex.Value < 0 ) yield break;
            
            // only the host has to instantiate and spawn a prefab
            if (IsHost)
            {
                var gamePrefab = GameManager.Instance.gamePrefabs[gameIndex.Value];
                _game = Instantiate(gamePrefab, this.gameObject.transform, true).GetComponent<Game>();
                _game.gameObject.transform.localPosition = Vector3.zero;
                _game.GetComponent<NetworkObject>().Spawn();
            }
            // wait a bit, until the host is done spawning the prefabs
            yield return new WaitForSeconds(1);
            
            // host spawns objects only into scene, not as child to a corresponding object, so let the clients do it themselves
            var gameName = GameManager.Instance.gamePrefabs[gameIndex.Value].gameObject.name;
            _game = GameObject.Find(gameName + "(Clone)").GetComponent<Game>();
            _game.transform.parent = this.transform;
            foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = false;
            }
            
            _game.RestartGame();
            WriteUiText();
        }

        [ClientRpc]
        private void RemoveGamesClientRpc()
        {
            if (_game != null)
            {
                Destroy(_game.gameObject);
                _game = null;
            }
        }

        [ClientRpc]
        private void SpawnAndResetStationClientRpc()
        {
            //Debug.Log(gameIndex.Value);
            var gamePrefab = GameManager.Instance.gamePrefabs[gameIndex.Value];
            _game = Instantiate(gamePrefab, this.gameObject.transform, true).GetComponent<Game>();
            
            _game.gameObject.transform.localPosition = Vector3.zero;
            Debug.Log("Station " + gameObject.name + " found game: " + _game.gameObject);
            if (_game != null)
            {
                _game.RestartGame();
                foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
                {
                    mesh.enabled = false;
                }
            }
        }
        
        private void WriteUiText()
        {
            if(_game == null) return;
            uiTaskName.text = _game.Title;
            uiTaskDescription.text = _game.Description;
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

        public void FinishedGame(bool successful)
        {
            var playerUi = GameObject.FindWithTag("GameUi");
            _infoCanvas = playerUi.GetComponentInChildren<InformationCanvasControl>();
            _iCurrentlyPlayIt = false;
            _isCurrentlyPlaying.Value = false;
            _game.registerdPlayers.Value--;
            if (!successful)
            {
                _failures.Value++;
                _isInCooldown = true;
                _infoCanvas.gameObject.SetActive(true);
                _infoCanvas.ShowFailure();
                StartCooldownCounter();
                _game.RestartGame();
            }
            else
            {
                _infoCanvas.gameObject.SetActive(true);
                _infoCanvas.ShowSuccess();
                _isInCooldown = false;
                _isDone.Value = true;
            }
            ScanStation();
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

        public void HideStation()
        {
            foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = false;
            }
        }
        
        public void ScanStation()
        {
            this.gameObject.SetActive(true);
            foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = true;
            }
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
                uiGameInfoPanel.SetActive(false);
            }
        }


        #region Cooldown

        public void StartCooldownCounter()
        {
            //ShowCooldownUi(true);
            StartCoroutine(Countdown(5));
        }

        private IEnumerator Countdown(float timeToWait)
        {
            while (timeToWait > 0)
            {
                timeToWait -= Time.deltaTime;
                uiCooldownValue.text = ((int) timeToWait).ToString() + "s";
                yield return null;
                
                if (timeToWait < 1)
                {
                    break;
                }
            }
            _isInCooldown = false;
            ScanStation();
        }
        
        #endregion

        
        public void ShowStatusUi(bool b)
        {
            uiGameInfoPanel.SetActive(!b);
            uiCooldownPanel.SetActive(b);

        }

        public void Minimize()
        {
            Animator anim = GetComponentInChildren<Animator>();
            anim.SetBool("isMinimized", !anim.GetBool("isMinimized"));
        }

        public void onPointerDownSabotage()
        {
            var player =    GameManager.Instance.players.FirstOrDefault(
                    x => x.Value == NetworkManager.Singleton.LocalClientId).Key;

            Debug.Log(player.playerId.ToString());
            if (player.roleString.Value != "Crew")
            {
                if (sabotages.Count < 1)
                {
                    Coroutine co = StartCoroutine(sabotage());
                    sabotages.Add(co);

                    //Debug.Log("Sabotaging");
                }
            }
        }

        public void onPointerDownRepair()
        {
            if (repairs.Count <1)
            {
                Coroutine co = StartCoroutine(repair());
                repairs.Add(co);
            }
        }

        public void onPointerUpSabotage()
        {
            //Debug.Log("Stop Sabotage");
            foreach (Coroutine co in sabotages) {
                StopCoroutine(co);
            }
            sabotages.Clear();
            sabotagefill_Instruction.fillAmount = 0;
        }

        public void onPointerUpRepair()
        {
            foreach (Coroutine co in repairs)
            {
                StopCoroutine(co);
            }
            repairs.Clear();
            sabotagefill_repair.fillAmount = 0;

        }

        IEnumerator sabotage()
        {
            while (sabotagefill_Instruction.fillAmount < 1)
            {
            if (uiStationPanel.GetComponent<Canvas>().enabled)
            {
                sabotagefill_Instruction.fillAmount += Time.deltaTime * .2f;
                yield return null;
            } else
                {
                    sabotagefill_Instruction.fillAmount = 0f;
                    yield return null;
                }
            }
            sabotagefill_Instruction.fillAmount = 1f;
            sabotagefill_repair.fillAmount = 1f;
            _isManipulated.Value = true;
            var player = GameManager.Instance.players.FirstOrDefault(
                    x => x.Value == NetworkManager.Singleton.LocalClientId).Key;
            //TODO: Use up the Sabotage Resources.
            ScanStation();
            
            // change the status of the Station to sabotaged.
        }

        IEnumerator repair()
        {
            while (sabotagefill_repair.fillAmount > 0)
            {
                if (uiStationPanel.GetComponent<Canvas>().enabled)
                {
                    sabotagefill_repair.fillAmount -= Time.deltaTime * .1f;
                    yield return null;
                } else
                {
                    sabotagefill_repair.fillAmount = 1f;
                    yield return null;
                }
            }
            sabotagefill_repair.fillAmount = 0f;
            sabotagefill_Instruction.fillAmount = 0f;
            _isManipulated.Value = false;
            ScanStation();
        }

    }
}
