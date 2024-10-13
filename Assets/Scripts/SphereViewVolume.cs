using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereViewVolume : AViewVolume
{
    public AViewVolume target;
    public float outerRadius;
    public float innerRadius;

    private float _distance;
    private float _selfWeight;

    public float SelfWeight
    {
        get { return _selfWeight; }
        private set { _selfWeight = value; }
    }

    private void Update()
    {
        _distance = Vector3.Distance(target.transform.position, transform.position);

        if (_distance <= outerRadius && IsActive == false)
        {
            SetActive(true);
        }
        if (_distance > outerRadius && IsActive == true)
        {
            SetActive(false);
        }
    }

    public override float ComputeSelfWeight()
    {
        if (_distance <= innerRadius)
        {
            SelfWeight = 1;
        }
        else if (_distance >= outerRadius)
        {
            SelfWeight = 0;
        }
        else
        {
            SelfWeight = 1 - (_distance - innerRadius) / (outerRadius - innerRadius);
        }

        return SelfWeight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }
}

