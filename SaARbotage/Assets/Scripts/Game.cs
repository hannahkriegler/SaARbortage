using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace SaARbotage
{
    public class Game : NetworkBehaviour
    {
        //public List<Player> players;
        public NetworkVariable<int> requiredPlayers = new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
        public NetworkVariable<bool> launch = new NetworkVariable<bool>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
        public NetworkVariable<int> registerdPlayers = new NetworkVariable<int>((new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone}));
        public NetworkVariable<int> stationID = new NetworkVariable<int>((new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone}));
        public NetworkVariable<bool> waitForPlayersToRegister;

        private Station _station;
        private AudioSource _audiosrc;

        [Header("Audio-Game")]
        public AudioClip Sucess;
        public AudioClip Fail;

        [Header("Informations")]
        public String Title;
        [TextArea]
        public String Description;
        public AudioClip uiAudioDescription;

        public void RestartDay()
        {
            launch.Value = false;
            registerdPlayers.Value = 0;
            _station = transform.parent.GetComponent<Station>();
            this.stationID.Value = _station.stationId.Value;
            waitForPlayersToRegister.Value = true;
            
            if (_station == null)
            {
                Debug.Log("No station was found for: " + gameObject.name);
            }
            
            SetupGame();
        }

        protected virtual void SetupGame()
        {
            
        }

        public virtual void RestartGame()
        {
            SetupGame();
            RestartDay();
        }

        private void OnEnable()
        {
            foreach (var station in FindObjectsOfType<Station>())
            {
                if (station.stationId.Value.Equals(stationID.Value))
                {
                    //gameObject.transform.SetParent(station.vuforiaTargetObj.transform); 
                }
            }
        }
        

        public void RegisterPlayer()
        {
            registerdPlayers.Value++;
            if (requiredPlayers.Value == registerdPlayers.Value)
            {
                //Debug.Log("Register Player: " + registerdPlayers.Value + requiredPlayers.Value);
                waitForPlayersToRegister.Value = false;
                _station.uiStationPanel.SetActive(false);
                LaunchGame();
            }
        }

        public virtual void LaunchGame()
        {
            launch.Value = true;
        }

        public virtual void FinishGame(bool successful)
        {
            if (successful) PlaySuccessSound();
            else PlayFailSound();
            launch.Value = false;
            _station.FinishedGame(successful);
            _station._isInCooldown = !successful;
        }

        protected virtual void PlaySound(AudioClip clip)
        {
            if (!TryGetComponent<AudioSource>(out _audiosrc))
            {
                _audiosrc = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                _audiosrc.playOnAwake = false;
                _audiosrc.loop = false;
            }
            if (clip == null) return;
            _audiosrc.Stop();
            _audiosrc.PlayOneShot(clip);

        }

        protected void PlaySuccessSound()
        {
            PlaySound(Sucess);
            _audiosrc.volume = 1.0f;
        }

        protected void PlayFailSound()
        {
            PlaySound(Fail);
            _audiosrc.volume = 0.5f;
        }

        protected void PlaySoundWithVolume(AudioClip clip, float volume)
        {
            PlaySound(clip);
            _audiosrc.volume = volume;

        }

        protected bool IsStillPlayingSound()
        {
            return (_audiosrc.isPlaying);
        }



    }
}
