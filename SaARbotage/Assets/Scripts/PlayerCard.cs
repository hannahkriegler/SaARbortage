using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage
{
    public class PlayerCard : MonoBehaviour
    {
        [Header("Panels")] 
        [SerializeField] private GameObject waitingForPlayerPanel;

        [SerializeField] private GameObject playerDataPanel;

        [Header("Data Display")] 
        [SerializeField] private Text playerDisplayNameText;

        [SerializeField] private Sprite selectedCharacterImage;
        [SerializeField] private Toggle isReadyToToggle;
    }
}
