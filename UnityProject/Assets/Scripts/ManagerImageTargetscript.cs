using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerImageTargetscript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Imagetargets;
    [SerializeField]
    private GameObject[] Prefabs;

    public void TestButton ()
    {
        foreach (GameObject target in Imagetargets)
        {
            GameObject New = Instantiate(Prefabs[UnityEngine.Random.Range(0, Prefabs.Length)]);
            GameObject old = target.transform.GetChild(0).gameObject;
            New.transform.position = old.transform.position;
            GameObject.Destroy(old);
            New.transform.parent = target.transform;
        }
    }
}
