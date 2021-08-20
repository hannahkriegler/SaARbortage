using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public class SelectorSpawner : MonoBehaviour
    {
        public CommGameSelector selectror;

        private GameObject _myObject;
        private int _key;
        private int _index;
      
        public void SpawnMyObject(GameObject tospawn, int key, int index)
        {
            _myObject = Instantiate(tospawn, transform.position, transform.rotation);
            _myObject.transform.parent = this.transform;
            _key = key;
            _index = index;
            //Activate UI, Play Animation.
        }

        public void TurnInAnswer()
        {
            selectror.CheckAnswer(_key, _index);
        }
    }
}
