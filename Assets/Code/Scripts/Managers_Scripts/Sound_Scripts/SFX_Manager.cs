using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SFX_Manager : MonoBehaviour
{
    public static Action<Vector3, AudioClip> Request3DSFX = (Vector3 pos, AudioClip clip) => { };
    public static Action<Vector3, AudioClip> Request2DSFX = (Vector3 pos, AudioClip clip) => { };
    public static Action<AudioClip> Stop3DAudioClip = (AudioClip clip) => { };
    public static Action<AudioClip> Stop2DAudioClip = (AudioClip clip) => { };

    [Header("Settings")]
    [SerializeField][Range(0, 5)] private int Max3DSFX = 3;
    [SerializeField][Range(0, 5)] private int Max2DSFX = 5;
    [Header("If distance is higher the sound will not be played")]
    [SerializeField] private float MaxDistanceFromPlayer = 50f;
    [Header("Referces")]
    [SerializeField] private AudioSource Source3D = null;
    [SerializeField] private AudioSource Source2D = null;

    private List<AudioSource> Sources_3D = new List<AudioSource>();
    private List<AudioSource> Sources_2D = new List<AudioSource>();

    private Transform PlayerTransform => LevelManager.PlayerTransform;

    private void Awake()
    {
        Request3DSFX += Request3D_SFX;
        Request2DSFX += Request2D_SFX;
        Stop3DAudioClip += Stop3DClip;
        Stop2DAudioClip += Stop2DClip;
    }

    public void SetUp()
    {
        for (int i = 0; i < Max3DSFX; i++)
        {
            Sources_3D.Add(Instantiate(Source3D, transform.position, Quaternion.identity));
            Sources_3D[i].gameObject.SetActive(false);
        }
        
        for (int i = 0; i < Max2DSFX; i++)
        {
            Sources_2D.Add(Instantiate(Source2D, transform.position, Quaternion.identity));
            Sources_2D[i].gameObject.SetActive(false);
        }
    }

    public void Request3D_SFX(Vector3 position, AudioClip clip)
    {
        if (Vector3.Distance(PlayerTransform.position, position) > MaxDistanceFromPlayer)
            return;

        for (int i = 0;i < Max3DSFX; i++)
        {
            if (!Sources_3D[i].isPlaying)
            {
                Sources_3D[i].transform.position = position;
                Sources_3D[i].clip = clip;
                Sources_3D[i].gameObject.SetActive(true);
                Sources_3D[i].Play();
            }
        }
    }

    public void Request2D_SFX(Vector3 position, AudioClip clip)
    {
        if (Vector3.Distance(PlayerTransform.position, position) > MaxDistanceFromPlayer)
            return;

        for (int i = 0; i < Max2DSFX; i++)
        {
            if (!Sources_2D[i].isPlaying)
            {
                Sources_2D[i].transform.position = position;
                Sources_2D[i].clip = clip;
                Sources_2D[i].gameObject.SetActive(true);
                Sources_2D[i].Play();
            }
        }
    }

    public void Stop3DClip(AudioClip clip)
    {
        for (int i=0; Sources_3D.Count<i; i++)
            if (Sources_3D[i].clip == clip)
                Sources_3D[i].Stop();
    }

    public void Stop2DClip(AudioClip clip)
    {
        for (int i = 0; Sources_2D.Count < i; i++)
            if (Sources_2D[i].clip == clip)
                Sources_2D[i].Stop();
    }

    private void OnDestroy()
    {
        Request3DSFX -= Request3DSFX;
        Request2DSFX -= Request2DSFX;
        Stop3DAudioClip -= Stop3DAudioClip;
        Stop2DAudioClip -= Stop2DAudioClip;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, MaxDistanceFromPlayer);
    }
#endif
}
