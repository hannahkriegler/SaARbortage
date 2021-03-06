using System;
using MLAPI;
using MLAPI.NetworkVariable;
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
        public NetworkVariable<string> roleString = new NetworkVariable<string>();
        public bool isAlive;
        public string name;
        public Sprite icon;
        public Color color;
        
        public void AssignPlayerToRole()
        {
            if (roleString.Value.Length < 1) 
                Debug.Log("Player has no role!");
            if (roleString.Value.Equals("Crew"))
            {
                role = new Crewmate();
            }
            this.role = role;
            this.isAlive = true;
        }

        public void Die()
        {
            isAlive = false;
        }
        
        public override void NetworkStart()
        {
            if (!IsLocalPlayer) return;
            //ui.SetActive(false);
            playerId = OwnerClientId;
            var input = ConnectionManager.instance.inputField.text;
            if (input.Length > 1)
            {
                name = input;
            }
            else
            {
                name = "name " + playerId.ToString();
            }
            if (IsServer)
            {
                ConnectionManager.instance.AddPlayerToLobby(playerId, name);
            }
            else
            {
                ConnectionManager.instance.AddPlayerToLobbyServerRpc(playerId, name);
            }
            
            roleString.Settings.WritePermission = NetworkVariablePermission.Everyone;
            roleString.Settings.ReadPermission = NetworkVariablePermission.Everyone;
        }

        public void ShowUI(bool b)
        {
            Debug.Log("CALLED SOW UI");
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
                GameManager.Instance.arCam.GetComponent<Camera>().enabled = true;
                
            }
            else
            {
                cam.SetActive(!b);
                if(ui != null)
                    ui.SetActive(!b);
            }
        }

      
        
    }
}
