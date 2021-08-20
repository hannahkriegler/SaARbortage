using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI.NetworkVariable;

namespace SaARbotage
{
    public class CommGameDescriber : Game
    {
        public NetworkVariable<int> shapeindex = new NetworkVariable<int>();
        public NetworkVariable<int> shapekey = new NetworkVariable<int>();

        // For now all 3 Lists should be equal in size.
        public List<GameObject> Objectlist1;
        public List<GameObject> Objectlist2;
        public List<GameObject> Objectlist3;

        public Text timer;
        public float timeTillFailure = 60F;

        private Dictionary< (int, int), GameObject> dic;

        public GameObject Spawnpoint; 
        protected override void SetupGame()
        {
            //Here we search for one random Object out of the different Shapes we got. 
            // We save the shapekeyes in networkvariables, so for each player the same solution is shown.
            // The object will be spawned when Start is being pressed. 
            // TOCO: Find out when the Startbutton is pressed. Which function is called, so the object is then spawned. 
            base.SetupGame();
            dic = new Dictionary<(int, int), GameObject>();
            foreach(GameObject obj in Objectlist1)
            {
                dic.Add((0, Objectlist1.IndexOf(obj)), obj);
            }

            foreach (GameObject obj in Objectlist2)
            {
                dic.Add((1, Objectlist2.IndexOf(obj)), obj);
            }

            foreach (GameObject obj in Objectlist3)
            {
                dic.Add((2, Objectlist3.IndexOf(obj)), obj);
            }
            if (IsHost)
            {
                shapeindex.Value = (int)UnityEngine.Random.Range(0, 3);
                shapekey.Value = (int)UnityEngine.Random.Range(0, Objectlist1.Count);
            }
        }

        public override void LaunchGame()
        {
            base.LaunchGame();
            GameObject Solution;
            dic.TryGetValue((shapeindex.Value, shapekey.Value), out Solution);
            if (Solution != null) {
                var newrot = Quaternion.Euler(0f , UnityEngine.Random.Range(0,180), 0f);
                GameObject solution = Instantiate(Solution, Spawnpoint.transform.position, newrot);
                solution.transform.parent = Spawnpoint.transform;
                solution.transform.localScale = solution.transform.localScale * 2;
                Spawnpoint.GetComponent<Animator>().SetTrigger("Spawn");
                //Play Sound and maybe Animation
                }
        }

        private void Update()
        {
            if (!launch.Value) return;
            if (timeTillFailure <= 0)
            {
                timer.text = "Time is over";
                base.FinishGame(false);
            }
            else
            {
                timeTillFailure -= Time.deltaTime;
                timer.text = ((int)(timeTillFailure / 60f)).ToString() + ":" + ((int)(timeTillFailure % 60f)).ToString();

            }

        }

        public void IsSelected (bool succesful)
        {
            base.FinishGame(succesful);
        }

        public Dictionary< (int,int), GameObject > GetDic()
        {
            if (dic != null) return dic;
            else return null; 
        }

        public int GetShapeIndex()
        {
            return shapeindex.Value;
        }

        public int GetShapeKey()
        {
            return shapekey.Value;
        }

        public float GetListLength()
        {
            return Objectlist1.Count;
        }
    }
}
