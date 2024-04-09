using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider Master_AudioSlider;
    [SerializeField] private Slider Music_AudioSlider;
    [SerializeField] private Slider SFX_AudioSlider;
    [SerializeField] private AudioMixer AudioMixer;

    public void ChangeMaster()
    {
        float volume = Master_AudioSlider.value;
        //Save volume value
        if (PlayerPrefs.HasKey("MasterVolume"))
            PlayerPrefs.SetFloat("MasterVolume", volume);
        //Apply volume value into audio mixer
        volume = 20 * Mathf.Log10(volume);
        if (volume < -80f)
            volume = -80f;
        AudioMixer.SetFloat("MasterVolume", volume);
    }

    public void ChangeMusic()
    {
        float volume = Music_AudioSlider.value;
        //Save volume value
        if (PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", volume);
        //Apply volume value into audio mixer
        volume = 20 * Mathf.Log10(volume);
        if (volume < -80f)
            volume = -80f;
        AudioMixer.SetFloat("MusicVolume", volume);
    }

    public void ChangeSFX()
    {
        float volume = SFX_AudioSlider.value;
        //Save volume value
        if (PlayerPrefs.HasKey("SFXVolume"))
            PlayerPrefs.SetFloat("SFXVolume", volume);
        //Apply volume value into audio mixer
        volume = 20 * Mathf.Log10(volume);
        if (volume < -80f)
            volume = -80f;
        AudioMixer.SetFloat("SFXVolume", volume);
    }

    private void OnEnable()
    {
        float volume;
        //Master
        if(PlayerPrefs.HasKey("MasterVolume"))
        {
            volume = PlayerPrefs.GetFloat("MasterVolume");
            Master_AudioSlider.value = volume;
        }
        //Music
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            volume = PlayerPrefs.GetFloat("MusicVolume");
            Music_AudioSlider.value = volume;
        }
        //SFX
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            volume = PlayerPrefs.GetFloat("SFXVolume");
            SFX_AudioSlider.value = volume;
        }
    }

}
