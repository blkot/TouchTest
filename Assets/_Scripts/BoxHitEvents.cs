using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxHitEvents : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(this.name + "  Activated");
        this.GetComponent<Renderer>().material.color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));

    }

    public void HitEventChangeColor()
    {
        Debug.Log(this.name + "  Activated");
        Material mat = this.GetComponent<Renderer>().material;
        mat.color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
        Debug.Log("Change Color to : " + mat.color);
    }
}
