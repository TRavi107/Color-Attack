using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIPanelType
{
    none,
    mainmenu,
    setting,
    mainGame,
    pauseMenu,
    GameOver,
    about,
    howToplay,
    levelUp,
}
public class UIPanel : MonoBehaviour
{
    public UIPanelType uiPanelType;

}
