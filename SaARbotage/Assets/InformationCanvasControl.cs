using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage {
    public class InformationCanvasControl : MonoBehaviour
    {
        public GameObject Success;
        public GameObject Failure;
        public GameObject PlayerInfo;

        public Text role;
        //private playerscript;
        private Player _player;
        public Text roleDescription;
        [TextArea]
        public string androidRole;
        [TextArea]
        public string crewRole;

        public void ShowSuccess()
        {
            Success.SetActive(true);
        }

        public void ShowFailure()
        {
            Failure.SetActive(true);
        }

        public void ShowPlayerInfo()
        {
            PlayerInfo.SetActive(true);
            _player = transform.parent.parent.GetComponent<Player>();
            if (_player == null) return;
            role.text = "Your role: " + _player.roleString.Value; 

            if (_player.roleString.Value == "Android")
            {
                roleDescription.text = androidRole;
            } else
            {
                roleDescription.text = crewRole;
            }
            // get role and so forth from playerscript and write depending on the Outcome. 

        }

        public void OK()
        {
            Success.SetActive(false);
            Failure.SetActive(false);
            PlayerInfo.SetActive(false);

            //this.gameObject.SetActive(false);
        }
    }
}
