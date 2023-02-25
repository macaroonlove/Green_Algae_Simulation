using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour
{
    public GameObject DesUI;

    public void MEnter()
    {
        DesUI.SetActive(true);
    }

    public void MExit()
    {
        DesUI.SetActive(false);
    }

    public void UD_Button()
    {
        if (!DesUI.activeSelf)
        {
            DesUI.SetActive(true);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            DesUI.SetActive(false);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
