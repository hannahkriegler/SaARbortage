using MLAPI;
using UnityEngine;

namespace SaARbotage
{
    public class Player : NetworkBehaviour
    {
        public GameObject cam;
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
        }

        public void ShowUI(bool b)
        {
            cam.SetActive(b);
            GetComponentInChildren<Canvas>().gameObject.SetActive(b);
        }
    }
}
