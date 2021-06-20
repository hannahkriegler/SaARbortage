using MLAPI;
using UnityEngine;

namespace SaARbotage
{
    public class Player : NetworkBehaviour
    {
        public GameObject cam;
        public GameObject ui;
        public GameObject uiPrefab;
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
            //ui.SetActive(false);
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
                if (ui == null)
                {
                    ui = Instantiate(uiPrefab, gameObject.transform, true);
                    if (IsHost)
                        ui.GetComponent<NetworkObject>().Spawn();
                }
                
                cam.SetActive(b);
                ui.SetActive(b);
                
            }
            else
            {
                cam.SetActive(!b);
                if(ui != null)
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
