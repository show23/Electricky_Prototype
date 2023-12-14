﻿/*
 The MIT License (MIT)

Copyright (c) 2013 yamamura tatsuhiko

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Rendering;

public class FadeImage : UnityEngine.UI.Graphic , IFade
{
	[SerializeField]
	private Texture maskTexture = null;

	[SerializeField, Range (0, 1)]
	private float cutoutRange;

    [SerializeField] private Color _color;

    public float Range {
		get {
			return cutoutRange;
		}
		set {
			cutoutRange = value;
			UpdateMaskCutout (cutoutRange);
		}
	}

	public Color MainColor
	{
		get { 
			return _color; 
		}
		set
		{
			_color = value;
            material.SetColor("_Color", _color);
        }
	}

	public void SetMaskTexture(string path)
	{
        maskTexture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture)) as Texture;
        material.SetTexture("_MaskTex", maskTexture);
    }

	protected override void Start ()
	{
		base.Start ();
		UpdateMaskTexture (maskTexture);
	}

	private void UpdateMaskCutout (float range)
	{
		enabled = true;
		material.SetFloat ("_Range", 1 - range);

		if (range <= 0) {
			this.enabled = false;
		}
	}

	public void UpdateMaskTexture (Texture texture)
	{
		material.SetTexture ("_MaskTex", texture);
		material.SetColor ("_Color", _color);
	}

	#if UNITY_EDITOR
	protected override void OnValidate ()
	{
		base.OnValidate ();
		UpdateMaskCutout (Range);
		UpdateMaskTexture (maskTexture);
	}
	#endif
}
