using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockPoint : MonoBehaviour
{
    public bool isAvailable = true;
    List<GameObject> lockPoints;

    private void Awake()
    {
        lockPoints = new List<GameObject>();
        for (int i = 0; i < transform.childCount - 1; i ++)
        {
            lockPoints.Add(transform.GetChild(i).gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
