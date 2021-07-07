using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public class MeetingManager : MonoBehaviour
    {
        public int meetingId;
        public Player[] registeredPlayers;
        public Vote[] votes;

        public bool StartMeeting(int playerAlive)
        {
            if (registeredPlayers.Length != playerAlive) return false;
            
            votes = new Vote[playerAlive];
            return true;
        }

        public bool StopMeeting(int playerAlive)
        {
            return false;
        }

    }
}
