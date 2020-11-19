using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;




public class IsPointerOverUI_IOS : MonoBehaviour
{
    public static bool isPointerOverUI;
    void Start()
    {

    }
#if UNITY_IOS || UNITY_ANDROID
    private void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {

            isPointerOverUI = !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
    }
#endif
}
