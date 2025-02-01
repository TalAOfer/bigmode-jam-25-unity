using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;
    [SerializeField] private Slider masterSlider;

    public void Start()
    {
        //InitializeVolume();
    }

    private void InitializeVolume()
    {
        if (!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 100f);
        } 

        PlayerPrefs.Save();

        masterSlider.value = PlayerPrefs.GetFloat("Volume");
    }

    public void OnMasterSliderChanged(float sliderValue)
    {
        float value = sliderValue * 10;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Volume", value);
        PlayerPrefs.Save();
    }

    public void PlayOneShot(string soundName)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/" + soundName);
    }

    public void SetRunningParameter(bool value) 
    {
        int intValue = value ? 1 : 0;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Running", intValue);
    }

    public void SetMusicParameter(int musicIndexNumber)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Music", musicIndexNumber);
    }

    public void AlterPlayerStateSoundParameter(string label)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("Movement", label);
    }
} 
