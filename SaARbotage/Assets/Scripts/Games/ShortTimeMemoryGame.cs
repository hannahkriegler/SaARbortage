using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SaARbotage
{
    public class ShortTimeMemoryGame : Game
    {
        public int rounds = 1;
        public int solutionLength = 5;
        public float animTime = 1f;
        public float time = 10f;
        public int life = 3;

        private bool _isstarted = false;
        private bool _playing = false;

        public Image[] imageField;
        private bool[] _rdyCheck;

        private int[] _solution;
        private int _counter = 0;
        private List<int> _input;
        private Camera _maincam;

        public Color signalColor;
        private Color _initialCol;
        public Color failColor;
        public Color winColor;

        private void Awake()
        {
            _solution = new int[solutionLength];
            _rdyCheck = new bool[solutionLength];
            for (int i = 0; i < _rdyCheck.Length; i++)
            {
                _rdyCheck[i] = false;
            }
            _maincam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            if (_maincam)
            {
                Debug.Log("Found");
                transform.GetChild(0).GetComponent<Canvas>().worldCamera = _maincam;
            }


        }

        //TODO: Lebensanzeige und Zeit für jede Runde left.

        public override void LaunchGame()
        {
            _initialCol = imageField[0].color;
            SetUpRound();
            _isstarted = true;
            base.LaunchGame();
        }

        // Update is called once per frame
        void Update()
        {
            //TODO: Hier gibt es nen Bug. Die buttons sind nicht anwählbar wenn wir den Canvas rotieren..
           /* if (!_playing && !_isstarted && rounds > 0)
            {
                _initialCol = imageField[0].color;
                SetUpRound();
                _isstarted = true;

            }
           */

            if (_isstarted&&!_playing && rounds > 0)
            {
                bool rdy = true;
                foreach(bool a in _rdyCheck)
                {
                    rdy = a && rdy;                       
                }
                if (rdy) _playing = true;
            }

            if (_playing && rounds > 0)
            {
                if (life <= 0) {
                    Debug.Log("COMPLETE FAILURE!!!");
                    base.FinishGame(false);
                            }
                if (_counter == solutionLength) {
                    Debug.Log("ROUND SUCCEESSS!!!");
                    _isstarted = false;
                    _playing = false;
                    _counter = 0;
                    rounds--;
                    for (int i = 0; i <_rdyCheck.Length; i++)
                    {
                        _rdyCheck[i] = false;
                    }
                    foreach(Image im in imageField)
                    {
                        im.color = _initialCol;
                    }
                        }
                if (rounds <= 0)
                {
                    //TODO: Wird hier sonst noch was gemacht?
                    base.FinishGame(true);
                    Debug.Log("Complete Success!!");
                }
            }
            //LookAtPlayer();


        }

        private void LookAtPlayer()
        {
            transform.GetChild(0).transform.LookAt(_maincam.transform);
        }

        private void SetUpRound()
        {
            for (int i = 0; i < solutionLength; i++)
            {
                int num = (int)UnityEngine.Random.Range(0, imageField.Length);
                _solution[i] = num;
                //Start Color animation which goes to Color X and back to standard.
                StartCoroutine(Colorchange((float) i, imageField[num] ));
                //imageField[_solution[i]].color = signalColor;
                Debug.Log("the " + i.ToString() + " value is: " + num.ToString());
            }
        }



        public void InputField(int numb)
        {
            Debug.Log("Hi");
            if (!_playing) return;
            if (numb == _solution[_counter])
            {
                Debug.Log("Correct");
                _counter++;
                imageField[numb].color = winColor;
                //this.CrossFadeColor(Color.green, speed, false, false);

            }
            else {
                Debug.Log("false");
                life -= 1;
                imageField[numb].color = failColor;
            }

        }

        IEnumerator Colorchange(float order, Image im)
        {
            //TODO: Mehr Richtung LERP undso.. Ist mir noch zu hart von der transition.
            float timeelapsed = 0f;
            while(timeelapsed < order) {
                timeelapsed += Time.deltaTime;
                yield return null;
            }
            while (timeelapsed >= order && timeelapsed < order+animTime)
            {
                im.color = signalColor;
                timeelapsed += Time.deltaTime;
                yield return null;
            }
            im.color = _initialCol;
            _rdyCheck[(int)order] = true;
        }
    }
}
