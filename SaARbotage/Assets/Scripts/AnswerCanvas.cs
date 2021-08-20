using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage {
    public class AnswerCanvas : MonoBehaviour
    {
        private int _key;
        private int _index;

        public CommGameSelector selectror;

        public void SetAnswer(int key, int index)
        {
            _key = key;
            _index = index;
        }

        public void Yes()
        {
            selectror.CheckAnswer(_key, _index);
            this.gameObject.SetActive(false);
        }

        public void No()
        {
            this.gameObject.SetActive(false);
        }
    }
}
