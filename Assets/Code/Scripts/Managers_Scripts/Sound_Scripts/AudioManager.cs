using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX_Manager))]
public class AudioManager : MonoBehaviour
{
    //Singeton
    [HideInInspector] public AudioManager Instance;

    [Header("Audio Settings")]
    [SerializeField] private List<AudioData> AudioData_List;

    private SFX_Manager SFXManager;

    private void Awake()
    {
        //Set Up singleton
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        SFXManager = GetComponent<SFX_Manager>();
    }

    private void Start()
    {
        //Audio is saved in decimal scale an then converted
        float volume = 0f;
        for (int i = 0; i < AudioData_List.Count; i++)
        {
            volume = AudioData_List[i].DefaultVolume;

            if (!PlayerPrefs.HasKey(AudioData_List[i].Type.ToString() + "Volume"))
                PlayerPrefs.SetFloat(AudioData_List[i].Type.ToString() + "Volume", AudioData_List[i].DefaultVolume);
            else
                volume = PlayerPrefs.GetFloat(AudioData_List[i].Type.ToString() + "Volume");
            
            //Convert audio volume
            volume = 20*Mathf.Log10(volume);
            if (volume < -80f)
                volume = -80f;
            
            AudioData_List[i].SubMixer.SetFloat(AudioData_List[i].Type.ToString() + "Volume", volume);
            Debug.Log(volume);
        }

        SFXManager.SetUp();
    }
}
