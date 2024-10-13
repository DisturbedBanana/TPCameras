using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AViewVolume : MonoBehaviour
{
    public int priority = 0;
    public AView view;
    public bool isCutOnSwitch;

    private int _uid;

    static int nextUid = 0;

    public int Uid
    {
        get => _uid;
        private set => _uid = value;
    }


    protected bool IsActive { get; private set; }

    public virtual float ComputeSelfWeight()
    {
        return 1.0f;
    }

    private void Awake()
    {
        Uid = nextUid;
        nextUid++;
    }

    protected void SetActive(bool isActive)
    {
        if(isActive)
        {
            ViewVolumeBlender.Instance.AddVolume(this);
        }
        else
        {
            ViewVolumeBlender.Instance.RemoveView(this);
        }
        IsActive = isActive;

        if (isCutOnSwitch)
        {
            ViewVolumeBlender.Instance.Update();
            CameraController.Instance.Cut();
        }
    }


}
