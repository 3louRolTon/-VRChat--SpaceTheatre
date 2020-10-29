
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class MenuController : UdonSharpBehaviour
{
    public GameObject localPanel;
    public GameObject globalPanel;
    public GameObject mirror;
    public GameObject videoMenu;
    public GameObject debugPanel;

    public GameObject[] Screens_189;
    public GameObject[] Screens_169;
    public GameObject[] Screens_43;

    public Image[] buttonScreens;

    //public Text buttonToggle;

    private Vector4 green = new Vector4(17 / 255.0f, 160 / 255.0f, 0 / 255.0f, 1);
    private Vector4 gray = new Vector4(194 / 255.0f, 194 / 255.0f, 194 / 255.0f, 1);

    public void MirrorToggle()
    {
        mirror.SetActive(!mirror.activeSelf);
    }

    public void VideoMenuToggle()
    {
        videoMenu.SetActive(!videoMenu.activeSelf);
    }

    public void LocalPanelToggle()
    {
        localPanel.SetActive(!localPanel.activeSelf);
    }

    public void GlobalPanelToggle()
    {
        globalPanel.SetActive(!globalPanel.activeSelf);
    }

    public void DebugPanelToggle()
    {
        debugPanel.SetActive(!debugPanel.activeSelf);
    }

    public void On189Screen()
    {
        foreach (var obj in Screens_189)
        {
            obj.SetActive(true);
        }
        foreach (var obj in Screens_169)
        {
            obj.SetActive(false);

        }
        foreach (var obj in Screens_43)
        {
            obj.SetActive(false);
        }
        buttonScreens[0].color = green;
        buttonScreens[1].color = gray;
        buttonScreens[2].color = gray;
    }

    public void On169Screen()
    {
        foreach (var obj in Screens_189)
        {
            obj.SetActive(false);
        }
        foreach (var obj in Screens_169)
        {
            obj.SetActive(true);

        }
        foreach (var obj in Screens_43)
        {
            obj.SetActive(false);
        }
        buttonScreens[0].color = gray;
        buttonScreens[1].color = green;
        buttonScreens[2].color = gray;
    }

    public void On43Screen()
    {
        foreach (var obj in Screens_189)
        {
            obj.SetActive(false);
        }
        foreach (var obj in Screens_169)
        {
            obj.SetActive(false);

        }
        foreach (var obj in Screens_43)
        {
            obj.SetActive(true);
        }
        buttonScreens[0].color = gray;
        buttonScreens[1].color = gray;
        buttonScreens[2].color = green;
    }
}
