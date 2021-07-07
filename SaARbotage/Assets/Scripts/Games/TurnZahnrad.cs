using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaARbotage
{
    public enum Part { inner, outer };
    public class TurnZahnrad : MonoBehaviour
    {
        //TODO: Einfügen, dass nur Spieler X das benutzen darf, I guess. Hannah Fragen was sie denkt.
        float RotationSpeed = 0f;
        AlignGame AG = null;
        public GameObject AssignedRing = null;
        public Part isme; 

        private void Awake()
        {
            AG = GetComponentInParent<AlignGame>();
            RotationSpeed = AG.RotationSpeed;
  
        }

        private void OnMouseDrag()
        {
            //Debug.Log("Hello this is RotationSpeed" + RotationSpeed.ToString());
            float rotx = Input.GetAxis("Mouse X") * RotationSpeed * Mathf.Deg2Rad;
            //float roty = Input.GetAxis("Mouse Y") * RotationSpeed * Mathf.Deg2Rad;

            //transform.RotateAround(Vector3.up, -rotx);
            //transform.RotateAround(Vector3.right, -roty);
            transform.Rotate(Vector3.up, rotx);
            if (AssignedRing != null)
            {
                if (isme == Part.inner) AG.RotateRingInnerZahnrad(AssignedRing, rotx);
                if (isme == Part.outer) AG.RotateRingOuterZahnrad(AssignedRing, rotx);
            }


        }

    }
}
