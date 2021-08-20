using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public class SelectorSpawner : MonoBehaviour
    {
        public CommGameSelector selectror;

        public AnswerCanvas answerCanv;

        private GameObject _myObject;
        private int _key;
        private int _index;
      
        public void SpawnMyObject(GameObject tospawn, int key, int index)
        {
            _myObject = Instantiate(tospawn, transform.position, transform.rotation);
            _myObject.transform.parent = this.transform;
            GetComponent<Animator>().SetTrigger("Spawn");
            _key = key;
            _index = index;
            //Activate UI, Play Animation.
        }

        public void TurnInAnswer()
        {
            selectror.CheckAnswer(_key, _index);
        }

        private void OnMouseDown()
        {
            //activate UI which asks if sure. Give UI script the answer. 
            answerCanv.gameObject.SetActive(true);
            answerCanv.SetAnswer(_key, _index);
        }
    }
}
