using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARTrackedImageManager))]
public class imagetrackingtut : MonoBehaviour
{
    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private ARTrackedImageManager ImageManager;

    private void Awake()
    {
        ImageManager = FindObjectOfType<ARTrackedImageManager>();

        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    private void OnEnable()
    {
        ImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        ImageManager.trackedImagesChanged += ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedI in eventArgs.added)
        {
            UpdateImage(trackedI);
        }

        foreach (ARTrackedImage trackedI in eventArgs.updated)
        {
            UpdateImage(trackedI);
        }

        foreach (ARTrackedImage trackedI in eventArgs.removed)
        {
            spawnedPrefabs[trackedI.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedI)
    {
        string name = trackedI.referenceImage.name;
        Vector3 position = trackedI.transform.position;
        GameObject prefab = spawnedPrefabs[name];
        prefab.transform.position = position;
        prefab.SetActive(true);

        foreach(GameObject go in spawnedPrefabs.Values)
        {
            if (go.name != name)
            {
                go.SetActive(false);
            }
        }
    }

}
