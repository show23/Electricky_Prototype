﻿using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIllustrator : MonoBehaviour
{
    private const float LineHeightOffset = 0.33f;
    LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void IllustratePath(Path1 path)
    {
        line.positionCount = path.tiles.Length;

        for (int i = 0; i < path.tiles.Length; i++)
        {
            Transform tileTransform = path.tiles[i].transform;
            line.SetPosition(i, tileTransform.position.With(y: tileTransform.position.y + LineHeightOffset));
        }
    }

    internal void IllustratePath1(Path1 path)
    {
        throw new NotImplementedException();
    }
}
