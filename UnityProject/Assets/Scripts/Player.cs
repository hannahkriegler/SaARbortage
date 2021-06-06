using MLAPI;
using UnityEngine;

namespace SaARbotage
{
    public class Player : NetworkBehaviour
    {
        public GameObject cam;
        public GameObject ui;
        public ulong playerId;
        public Role role;
        public bool isAlive;

        public void AssignPlayer( Role role)
        {
            this.role = role;
            this.isAlive = true;
        }

        public void Die()
        {
            isAlive = false;
        }
        
        public override void NetworkStart()
        {
            //if (!NetworkManager.Singleton.IsServer) return;
            ConnectionManager.instance.AddPlayerToLobby(this);
            Debug.Log("Stated Network for: " + OwnerClientId);
        }

        public void ShowUI(bool b)
        {
            if (IsLocalPlayer)
            {
                cam.SetActive(b);
                ui.SetActive(b);
            }
            else
            {
                cam.SetActive(!b);
                ui.SetActive(!b);
            }
        }
    }
}
