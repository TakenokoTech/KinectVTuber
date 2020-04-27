using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCopyMono : MonoBehaviour
{
    [SerializeField] private GameObject targetObj;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = targetObj.transform.position;
        transform.rotation = targetObj.transform.rotation;
    }
}
