using MLAPI;

using UnityEngine.UI;

namespace SaARbotage
{
    public class QuestOverview : NetworkBehaviour
    {
        public Text questDayValue;

        public override void NetworkStart()
        {
            base.NetworkStart();
            GameManager.Instance.openStationsForDay.OnValueChanged += UpdateQuestUi;
        }

        private void UpdateQuestUi(int prevI, int newI)
        {
            questDayValue.text = GameManager.Instance.openStationsForDay.Value.ToString();
        }
    }
}