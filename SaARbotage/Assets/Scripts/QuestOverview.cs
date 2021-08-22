using MLAPI;

using UnityEngine.UI;

namespace SaARbotage
{
    public class QuestOverview : NetworkBehaviour
    {
        public Text questDayValue;
        
        public void UpdateQuestUi(int openStations)
        {
            if (openStations == 0)
            {
                
            }
            questDayValue.text = "Open Stations for day:/n" + openStations.ToString();
        }
        
        
    }
}