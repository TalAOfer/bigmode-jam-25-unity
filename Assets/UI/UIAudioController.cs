using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioController : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXslider;

    public void Start()
    {
        InitializeVolume();
    }

    private void InitializeVolume()
    {
        if (!PlayerPrefs.HasKey("Master_Volume"))
        {
            PlayerPrefs.SetFloat("Master_Volume", 100f);
        } 
        
        if (!PlayerPrefs.HasKey("SFX_Volume"))
        {
            PlayerPrefs.SetFloat("SFX_Volume", 100f);
        }

        if (!PlayerPrefs.HasKey("Music_Volume"))
        {
            PlayerPrefs.SetFloat("Music_Volume", 100f);
        }

        PlayerPrefs.Save();

        masterSlider.value = PlayerPrefs.GetFloat("Master_Volume");
        SFXslider.value = PlayerPrefs.GetFloat("SFX_Volume");
        musicSlider.value = PlayerPrefs.GetFloat("Music_Volume");
    }

    public void OnMasterSliderChanged(float sliderValue)
    {
        float value = sliderValue * 10;
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Master_Volume", value);
        PlayerPrefs.Save();
    }

    public void OnSFXSliderChanged(float sliderValue)
    {

        float value = sliderValue * 10;
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SFX_Volume", value);
        PlayerPrefs.Save();
    }

    public void OnMusicSliderChanged(float sliderValue)
    {
        float value = sliderValue * 10;
       // FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Music_Volume", value);
        PlayerPrefs.Save();
    }
}
