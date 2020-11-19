using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchRaycast : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //hit.transform.GetComponent<BoxHitEvents>().HitEventChangeColor();
                    //Debug.Log("Touch Hit :"+ hit.collider.name);
                }
            }
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                //hit.transform.GetComponent<BoxHitEvents>().HitEventChangeColor();
                //Debug.Log("Mouse Hit :" + hit.collider.name);
            }
        }
#endif
    }
}
