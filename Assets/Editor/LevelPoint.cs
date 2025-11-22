using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPoint : MonoBehaviour
{
    [SerializeField] private Vector3 position;

    void Awake()
    {
        position = transform.position;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
