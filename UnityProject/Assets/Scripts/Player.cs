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
        public string name;

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
            Debug.Log("HERE!");
            playerId = OwnerClientId;
            name = "name " + playerId.ToString();
            if (IsServer)
            {
                ConnectionManager.instance.AddPlayerToLobby(playerId, name);
            }
            else
            {
                ConnectionManager.instance.AddPlayerToLobbyServerRpc(playerId, name);
            }
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

        public void FindGame()
        {
            var obj = GameObject.FindWithTag("Game");
            var game = obj.GetComponent<AlignGame>();
            game.RegisterPlayer(this);
        }
    }
}
