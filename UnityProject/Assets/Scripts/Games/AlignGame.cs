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

        public float RingRotationSpeed = 1f;
        public float RotationSpeed = 20;

        private void Update()
        {
            //if (!launch) return;
            //if(!IsLocalPlayer) return;   

            // Rotiere Ringe jenachdem wie Z jetzt steht bei Dingens.
        }

        public void RotateRing(GameObject ring, float prog)
        {
            if (ring == innerRing) {
                innerRing.transform.Rotate(new Vector3(prog * RingRotationSpeed, prog * RingRotationSpeed, 0f));
                } else
                {
                    outerRing.transform.Rotate(new Vector3(0f, prog * RingRotationSpeed, prog * RingRotationSpeed));
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
