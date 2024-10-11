using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewVolumeBlender : MonoBehaviour
{
    public static ViewVolumeBlender Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private List<AViewVolume> activeViewVolumes = new List<AViewVolume>();
    private Dictionary<AView, List<AViewVolume>> volumePerViews;

    private void Update()
    {
        // Sort the active view volumes by priority and UID
        activeViewVolumes.Sort((x, y) =>
        {
            int priorityComparison = y.priority.CompareTo(x.priority);

            if (priorityComparison == 0)
            {
                return x.Uid.CompareTo(y.Uid);
            }
            return priorityComparison;
        });

        foreach (AViewVolume viewVolume in activeViewVolumes)
        {
            float weight = viewVolume.ComputeSelfWeight();
            weight = Mathf.Clamp01(weight);
            float remainingWeight = 1.0f - weight;

            viewVolume.view.weight = weight;
            viewVolume.view.weight *= remainingWeight;
        }
    }

    public void AddVolume(AViewVolume viewVolume)
    {
        activeViewVolumes.Add(viewVolume);
        if (!volumePerViews.ContainsKey(viewVolume.view))
        {
            viewVolume.view.SetActive(true);
        }
        volumePerViews[viewVolume.view].Add(viewVolume);
    }

    public void RemoveView(AViewVolume viewVolume)
    {
        activeViewVolumes.Remove(viewVolume);
        volumePerViews[viewVolume.view].Remove(viewVolume);
        if (volumePerViews[viewVolume.view].Count == 0)
        {
            viewVolume.view.SetActive(false);
        }
    }
}
