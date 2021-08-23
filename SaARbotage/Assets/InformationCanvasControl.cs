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
        public Text roleDescription;

        private void Start()
        {

        }

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
            // get role and so forth from playerscript and write depending on the Outcome. 

        }

        public void OK()
        {
            Success.SetActive(false);
            Failure.SetActive(false);
            PlayerInfo.SetActive(false);

            this.gameObject.SetActive(false);
        }
    }
}
