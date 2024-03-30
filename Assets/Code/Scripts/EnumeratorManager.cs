using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnumeratorManager : MonoBehaviour
{
    public static EnumeratorManager Instance;
    public static MonoBehaviour MonoBehaviourInstance;

    private void Awake()
    {
        //Set Up Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("WARNING: MULTIPLE ENUMERATOR MANAGER FOUND!");
            Destroy(this.gameObject);
            return;
        }
        MonoBehaviourInstance = GetComponent<MonoBehaviour>();
    }

    public static void AddEnumerator(IEnumerator enumerator)
    {
        MonoBehaviourInstance.StartCoroutine(enumerator);
    }
}