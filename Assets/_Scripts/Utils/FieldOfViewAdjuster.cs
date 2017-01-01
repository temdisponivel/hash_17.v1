using UnityEngine;
using System.Collections;

public class FieldOfViewAdjuster : MonoBehaviour
{
    public Camera Camera;

    public float DesiredAspectRatio = 16f / 9f;
    public float RealAspectRatio
    {
        get
        {
            return Screen.width / (Screen.height * 1f);
        }
    }
    public float AspectRatioToSet
    {
        get
        {
            return DesiredAspectRatio / RealAspectRatio;
        }
    }

    void Awake()
    {
        if (Camera == null)
            Camera = GetComponent<Camera>();
    }

    void Start()
    {
        if (Camera.orthographic)
        {
            Camera.orthographicSize = Camera.orthographicSize * AspectRatioToSet;
        }
        else
        {
            Camera.fieldOfView = Camera.fieldOfView * AspectRatioToSet;
        }
    }
}
