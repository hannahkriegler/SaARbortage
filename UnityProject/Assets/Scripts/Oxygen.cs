using MLAPI;

using UnityEngine.UI;

namespace SaARbotage
{
    public class Oxygen : NetworkBehaviour
    {
        public Image oxygenBar;
        private float _fillAmount;
        private float _onePercent;
        private float _timeTotal;

        private void Start()
        {
           _timeTotal = GameManager.Instance.time;
           if(IsHost)
               gameObject.GetComponent<NetworkObject>().Spawn();
        }
        
        public void ChangeTime(float value)
        {
            GameManager.Instance.syncTime.Value += value;
        }

        private void Update()
        {
            _fillAmount = GameManager.Instance.syncTime.Value / _timeTotal ;
            oxygenBar.fillAmount = _fillAmount;
        }

        public void TestScanButton()
        {
            GameManager.Instance.ScanStation();
        }

        public void TestPlayGameButton()
        {
            GameManager.Instance.PlayGame();
        }
    }
}
