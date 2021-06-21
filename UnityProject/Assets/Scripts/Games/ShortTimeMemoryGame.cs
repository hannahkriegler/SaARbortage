using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage
{
    public class ShortTimeMemoryGame : MonoBehaviour
    {
        public int rounds = 1;
        public int solutionLength = 5;
        public float animTime = 1f;
        public float time = 10f;
        public int life = 3;

        private bool _isstarted = false;
        private bool _playing = false;

        public Image[] imageField;
        private int _last;

        private int[] _solution;
        private int _counter = 0;
        private List<int> _input;

        private void Awake()
        {
            _solution = new int[solutionLength];
        }

        // Update is called once per frame
        void Update()
        {
            if (!_playing && !_isstarted && rounds > 0)
            {
                for(int i = 0; i< solutionLength; i++)
                {
                    int num = (int) UnityEngine.Random.Range(0, imageField.Length);
                    _solution[i] = num;
                    imageField[_solution[i]].color = Color.blue;
                    Debug.Log("the " + i.ToString() + " value is: " + num.ToString());
                }
                _isstarted = true;

            }
            if (!_playing && rounds > 0)
            {   // hier coroutinen für den Farbwechsel. Wenn alle Coroutinen done sind, dann darf man spielen!
                
                _playing = true;
            }

            if (_playing && rounds > 0)
            {
                if (life <= 0) Debug.Log("COMPLETE FAILURE!!!");
                if (_counter == solutionLength) {
                    Debug.Log("ROUND SUCCEESSS!!!");
                    _isstarted = false;
                    _playing = false;
                    rounds--;
                        }
                if (rounds <= 0)
                {
                    Debug.Log("Complete Success!!");
                }
            }


        }

        public void InputField(int numb)
        {
            if (!_playing) return;
            if (numb == _solution[_counter])
            {
                Debug.Log("Correct");
                _counter++;
                //this.CrossFadeColor(Color.green, speed, false, false);

            }
            else {
                Debug.Log("false");
                life -= 1; }
        }
    }
}
