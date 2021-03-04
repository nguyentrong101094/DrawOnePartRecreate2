using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//calling Show() Hide() on this will call all linked MgObject
public class MgAnimLink : MgAnimBase
{
    [SerializeField] MgObject[] linkedObjects;

    public override void Show(bool immediately = false)
    {
        base.Show(immediately);
        for (int i = 0; i < linkedObjects.Length; i++)
        {
            linkedObjects[i].Show(immediately);
        }
    }

    public override void Hide(bool immediately = false)
    {
        base.Hide(immediately);
        for (int i = 0; i < linkedObjects.Length; i++)
        {
            linkedObjects[i].Hide(immediately);
        }
    }
}
