using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RamailoGames;

public class UIManager : MonoBehaviour
{
    public GameObject[] tutorialWindows;
    public Button[] tutorialWindowsButton;
    public Sprite notSelectedSprite;
    public Sprite selectedSprite;
    public BombController mainMenuBomb;
    public static UIManager instance;
    public List<UIPanel> uiPanels;

    public UIPanel activeUIPanel;
    public Slider soundSlider;
    public Slider musicSlider;

    public TMP_Text highscoreText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SwitchCanvas(uiPanels[0].uiPanelType);
        soundSlider.value = soundManager.instance.soundeffectVolume;
        musicSlider.value = soundManager.instance.backGroundAudioVolume;
        OnMusicVolumeChanged();
        OnSoundVolumeChanged();
        soundSlider.onValueChanged.AddListener(delegate { OnSoundVolumeChanged(); });
        musicSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChanged(); });

        soundManager.instance.PlaySound(SoundType.backgroundSound);
        if(highscoreText!=null)
            GetHighScore();
        
    }
    void GetHighScore()
    {
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                highscoreText.text = d.high_score.ToString();
            }
        });
    }


    public void SwitchCanvas(UIPanelType targetPanel)
    {

        foreach (UIPanel panel in uiPanels)
        {

            if (panel.uiPanelType == targetPanel)
            {
                
                activeUIPanel = panel;
            }
            else
            {
                panel.gameObject.SetActive(false);
            }
        }
        
        activeUIPanel.gameObject.SetActive(true);
    }

    public void ActivateTutorialWindows(int winIndex)
    {
        int index = 0;
        foreach (GameObject item in tutorialWindows)
        {
            if (index == winIndex)
            {
                item.SetActive(true);
                tutorialWindowsButton[index].image.sprite = selectedSprite;
            }
            else
            {
                item.SetActive(false);
                tutorialWindowsButton[index].image.sprite = notSelectedSprite;
            }
            index++;
        }
    }

    public void OnMusicVolumeChanged()
    {

        soundManager.instance.MusicVolumeChanged(musicSlider.value);
        soundManager.instance.SaveMusicVoulme(musicSlider.value);
    }

    public void OnSoundVolumeChanged()
    {
        soundManager.instance.SoundVolumeChanged(soundSlider.value);
        soundManager.instance.SaveSoundVoulme(soundSlider.value);
    }


    public void PlayGame()
    {
        mainMenuBomb.manualDestroy();
    }
}
