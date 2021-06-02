using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public class Player : MonoBehaviour
    {
        public int playerId;
        public Role role;
        public bool isAlive;

        public Player(int playerId, Role role)
        {
            this.playerId = playerId;
            this.role = role;
            this.isAlive = true;
        }

        public void Die()
        {
            isAlive = false;
        }
    }
}
