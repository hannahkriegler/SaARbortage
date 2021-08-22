using MLAPI;

using UnityEngine.UI;

namespace SaARbotage
{
    public class Oxygen : NetworkBehaviour
    {
        public Image oxygenBar;
        public Text timeText;
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
            var time = GameManager.Instance.syncTime.Value;
            var day = GameManager.Instance.currentDay.Value;
            _fillAmount =  time / _timeTotal ;
            oxygenBar.fillAmount = _fillAmount;
            timeText.text = "Day " + day + " - " + (int) time + "s left";
        }
        
        
    }
}
