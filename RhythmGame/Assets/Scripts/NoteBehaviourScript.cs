using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehaviourScript : MonoBehaviour
{

    public int noteType;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * GameManager.instance.noteSpeed);
    }
}
