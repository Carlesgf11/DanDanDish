using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsAppearControl : MonoBehaviour
{
    public Animator ButtonsPanelAnim;

    public void ShowButtons()
    {
        ButtonsPanelAnim.SetBool("Show", true);
    }
    public void HideButtons()
    {
        ButtonsPanelAnim.SetBool("Show", false);
    }
}
