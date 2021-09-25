using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage
{
    public class VotingManager : NetworkBehaviour
    {
        public NetworkVariable<float> timeUntilVoteingStarts = new NetworkVariable<float>();
        public NetworkVariable<bool> meetingStarted = new NetworkVariable<bool>();
        public StoryUi storyUi;
        
        public List<Player> registeredPlayers = new List<Player>();
        private const float MEETINGCOOLDOWN = 60f;
        
        // This method is called by the Game manager, afterwards the logic is placed in here
        public void TriggerVoting()
        {
            // Setup voting
            if (IsHost)
            {
                this.timeUntilVoteingStarts.Settings.ReadPermission = NetworkVariablePermission.Everyone;
                
                this.meetingStarted.Settings.ReadPermission = NetworkVariablePermission.Everyone;
                StartCoroutine(StartCountdown());
            }
            
            // Show UI
            var playerUi = GameObject.FindWithTag("GameUi");
            storyUi = playerUi.GetComponentInChildren<StoryUi>();
            storyUi.ActivateStoryPanel(true);

            // call everone to meeting

            // activate station
        }

        private IEnumerator StartCountdown()
        {
            timeUntilVoteingStarts.Value = MEETINGCOOLDOWN;
            while (timeUntilVoteingStarts.Value > 0)
            {
                timeUntilVoteingStarts.Value -= Time.deltaTime;
                if(storyUi!= null)
                    storyUi.SetMeetingStartCountdownText( "Meeting Starts in: " + ((int) timeUntilVoteingStarts.Value).ToString());
                yield return null;

                if (timeUntilVoteingStarts.Value < 1)
                {
                    break;
                }
            }
            StartMeeting();
        }

        private void StartMeeting()
        {
            // show voting ui
            
            
            foreach (var registeredPlayer in registeredPlayers)
            {
                // align voting ui
            }
        }

        private void EndMeeting()
        {
            GameManager.Instance.StartNewDay();
        }

        public void ScanMeetingRoom()
        {
            if(!IsLocalPlayer) return;

            var player =
                GameManager.Instance.players.FirstOrDefault(
                    x => x.Value == NetworkManager.Singleton.LocalClientId).Key;
            Debug.Log("### register player with id: " + player.playerId + " ###");
            RegisterPlayer(player);
        }

        public void RegisterPlayer(Player player)
        {
            // tell server, player joined
            
            // server tells players who joined
        }
    }
}
