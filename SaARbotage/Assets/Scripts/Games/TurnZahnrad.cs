using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public enum Part { inner, outer };
    public enum Side { left, right};
    public class TurnZahnrad : MonoBehaviour
    {
        //TODO: Einfügen, dass nur Spieler X das benutzen darf, I guess. Hannah Fragen was sie denkt.
        [Range(0f,360f)]
        public float RotationSpeed = 0f;
        AlignGame AG = null;
        public Part isme;
        public Side side;

        private void Awake()
        {
            AG = GetComponentInParent<AlignGame>();
            RotationSpeed = AG.RotationSpeed;
  
        }

        private void OnMouseDown()
        {
            //float rotx = Input.GetAxis("Mouse X") * RotationSpeed * Mathf.Deg2Rad;
            float rotx = 0;
            if (isme == Part.inner)
            {
                rotx = RotationSpeed * Time.deltaTime;
            } else
            {
                rotx = -RotationSpeed * Time.deltaTime;
            }
            transform.Rotate(Vector3.up, rotx);
            
            AG.RotateZahnrad(side, rotx);
            AG.RotateZahnrad(side, rotx);
            


        }

    }
}
