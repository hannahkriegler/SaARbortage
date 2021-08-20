using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

            shapeindex.Value = (int) UnityEngine.Random.Range(0, 3);
            shapekey.Value = (int) UnityEngine.Random.Range(0, Objectlist1.Count);
        }

        public override void LaunchGame()
        {
            base.LaunchGame();
            GameObject Solution;
            dic.TryGetValue((shapeindex.Value, shapekey.Value), out Solution);
            if (Solution != null) {
                GameObject solution = Instantiate(Solution, Spawnpoint.transform.position, Spawnpoint.transform.rotation);
                solution.transform.parent = Spawnpoint.transform;
                solution.transform.localScale = solution.transform.localScale * 2;
                //Play Sound and maybe Animation
                }
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
