using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public class AlignGame : Game
    {
        public GameObject rightZahnrad;
        public GameObject leftZahnrad;

        public GameObject innerRing;
        public GameObject outerRing;

        public GameObject mesh;
        private void Update()
        {
            if (!launch) return;
            if(!IsLocalPlayer) return;
            
            if (Input.GetMouseButton(0))
            {
                
            }
            
        }

        public override void LaunchGame()
        {
            base.LaunchGame();
            mesh.SetActive(true);
        }

        
        private void RotateRing(Player player, GameObject ring)
        {
            // first player action
            if (player.Equals(players[0])) 
            {
                
            }

            // second player action
            else
            {
                
            }
        }
    }
}
