using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage
{
    public class StoryUi : MonoBehaviour
    {
        public GameObject storyUiPanel;
      
        public Text meetingStartsCountdownText;
        

        public void SetMeetingStartCountdownText(string s)
        {
            meetingStartsCountdownText.text = s;
        }
        
        public void ActivateStoryPanel(bool b)
        {
            storyUiPanel.SetActive(b);
        }
    }
}
