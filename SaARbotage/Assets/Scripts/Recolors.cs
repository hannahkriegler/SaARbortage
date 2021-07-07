using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public class Recolors : MonoBehaviour
    {
        public Color color1 = Color.black;
        public Color color2= Color.black;
        public Color color3 = Color.black;

        private List<GameObject> _mychilds; 
       // Start is called before the first frame update
        void Start()
        {
            Color[] _colorlist = { color1, color2, color3 };
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Renderer>().material.SetColor("Emission_Color", _colorlist[Random.Range(0,3)]);

            }

        }
    }
}
