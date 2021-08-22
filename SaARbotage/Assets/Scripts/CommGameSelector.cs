using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI.NetworkVariable;

namespace SaARbotage { 
    public class CommGameSelector : Game
    {
        public GameObject[] Spawnpoints;

        //this should be changed in the future to allow random spawns.
        public CommGameDescriber describer;

        public Text timer;
        public float timeTillFailure = 60;
        private float _maxtime = 0f;

        private int _shapeindex;
        private int _shapekey;
        private Dictionary<(int, int), GameObject> _dic;
        private float _maxkey;

        private GameObject _solution;
        private GameObject _nearsolution;
        private int _nearsolShapeKey; 
        private GameObject _random;
        private int _randomShapeIndex;
        private int _randomShapeKey;

        private List<(GameObject, int, int)> _options;


        private void Awake()
        {
            _maxtime = timeTillFailure;
        }
        public override void LaunchGame()
        {
            base.LaunchGame();
            // Take the needed Variables and instantiate the objects. 
            //Activate UI after all is done.
            //Find Describer. 
            describer = FindObjectOfType<CommGameDescriber>();
            if (describer == null)
            {
                Debug.LogError("Describer not found");
                return;
            }
            _dic = describer.GetDic();
            _shapeindex = describer.GetShapeIndex();
            _shapekey = describer.GetShapeKey();
            _maxkey = describer.GetListLength();

            //Generate new random fields.
            _nearsolShapeKey = (int) UnityEngine.Random.Range(0, _maxkey);
            while (_nearsolShapeKey == _shapekey)
            {
                _nearsolShapeKey = (int)UnityEngine.Random.Range(0, _maxkey);
            }

            _randomShapeIndex = (int)UnityEngine.Random.Range(0, 3);
            _randomShapeKey = (int)UnityEngine.Random.Range(0, _maxkey);

            while(_randomShapeIndex == _shapeindex || _randomShapeKey == _shapekey)
            {
                _randomShapeIndex = (int)UnityEngine.Random.Range(0, 3);
                _randomShapeKey = (int)UnityEngine.Random.Range(0, _maxkey);
            }


            _dic.TryGetValue((_shapeindex, _shapekey), out _solution);
            _dic.TryGetValue((_shapeindex, _nearsolShapeKey), out _nearsolution);
            _dic.TryGetValue((_randomShapeIndex, _randomShapeKey), out _random);

            if(_solution != null && _nearsolution != null && _random != null)
            {
                SpawnOptions();
            }

        }

        private void SpawnOptions()
        {
            _options = new List<(GameObject, int, int)>();
            _options.Add((_solution, _shapekey, _shapeindex));
            _options.Add((_nearsolution,_nearsolShapeKey , _shapeindex));
            _options.Add((_random,_randomShapeKey,_randomShapeIndex));

            foreach (GameObject obj in Spawnpoints)
            {
                int index = (int) UnityEngine.Random.Range(0, _options.Count);
                obj.GetComponent<SelectorSpawner>().SpawnMyObject(_options[index].Item1, _options[index].Item2, _options[index].Item3);
                //Instantiate(_options[index], obj.transform.position, obj.transform.rotation);
                //Play Anim?
                _options.RemoveAt(index);
            }

            //Play Sounds?
           
        }

        private void Update()
        {
            if (!launch.Value) return;
            if (timeTillFailure <= 0)
            {
                timer.text = "Time is over";
                base.FinishGame(false);
            } else
            {
                timeTillFailure -= Time.deltaTime;
                timer.text = ((int)(timeTillFailure / 60f)).ToString() + ":" + ((int)(timeTillFailure % 60f)).ToString();

            }
            
        }

        public bool CheckAnswer(int key, int index)
        {
            if (key == _shapekey && index == _shapeindex)
            {
                Debug.Log("right answer");
                describer.IsSelected(true);
                base.FinishGame(true);
                return true;
            } else
            {
                Debug.Log("Wrong Answer");
                describer.IsSelected(false);
                base.FinishGame(false);
                return false;
            }
        }

        public override void RestartGame()
        {
            // Do we need to reset the network launch variable? or is it in Finishgame false
            timeTillFailure = _maxtime;
            base.RestartGame();
        }

    }
}
