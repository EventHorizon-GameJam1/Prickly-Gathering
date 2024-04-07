using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SFX_Manager : MonoBehaviour
{
    public static Action<Vector3, AudioClip> Request3DSFX = (Vector3 pos, AudioClip clip) => { };
    public static Action<Vector3, AudioClip> Request2DSFX = (Vector3 pos, AudioClip clip) => { };

    [Header("Settings")]
    [SerializeField][Range(0, 5)] private int Max3DSFX = 3;
    [SerializeField][Range(0, 5)] private int Max2DSFX = 5;
    [Header("If distance is higher the sound will not be played")]
    [SerializeField] private float MaxDistanceFromPlayer = 50f;
    [Header("Referces")]
    [SerializeField] private SFX_Effect Source3D = null;
    [SerializeField] private SFX_Effect Source2D = null;

    private List<SFX_Effect> Sources_3D = new List<SFX_Effect>();
    private List<SFX_Effect> Sources_2D = new List<SFX_Effect>();

    private Transform PlayerTransform => LevelManager.PlayerTransform;

    private void Awake()
    {
        Request3DSFX += Request3D_SFX;
        Request2DSFX += Request2D_SFX;
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
        if (clip == null)
            return;

        for (int i = 0;i < Max3DSFX; i++)
        {
            if (!Sources_3D[i].gameObject.activeInHierarchy)
            {
                Sources_3D[i].gameObject.SetActive(true);
                Sources_3D[i].PlaySound(position, clip);
                return;
            }
        }
    }

    public void Request2D_SFX(Vector3 position, AudioClip clip)
    {
        if (Vector3.Distance(PlayerTransform.position, position) > MaxDistanceFromPlayer)
            return;
        if (clip == null)
            return;

        for (int i = 0; i < Max2DSFX; i++)
        {
            if (!Sources_2D[i].gameObject.activeInHierarchy)
            {
                Sources_2D[i].gameObject.SetActive(true);
                Sources_2D[i].PlaySound(position, clip);
                return;
            }
        }
    }

    private void OnDestroy()
    {
        Request3DSFX -= Request3DSFX;
        Request2DSFX -= Request2DSFX;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, MaxDistanceFromPlayer);
    }
#endif
}
