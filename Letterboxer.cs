// LetterBoxer.cs
//
// A Unity component that you attach to your cameras to allow letter (and pillar) boxing.
// You specify min and max aspect ratios for landscape and portrait and it works out the rest.
// Also attach to any canvas cameras you are using. Any code that needs the effective screen
// resolution or aspect ratio should call members of this component rather than Screen.width and Screen.height.
// You should put a 'background' orthographic camera in your scene with 'Solid Color' Clear Flags, Nothing
// in the Culling Mask, and a really low Depth (like -100 or something).
//
// Copyright (c) 2017 Tristan Grimmer.
// Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby
// granted, provided that the above copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
// INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN
// AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
// PERFORMANCE OF THIS SOFTWARE.
using UnityEngine;

public class Letterboxer : MonoBehaviour
{
	public Vector2 landscapeMinAspectRatio	= new Vector2(3.0f, 2.0f);
	public Vector2 landscapeMaxAspectRatio	= new Vector2(16.0f, 9.0f);
	public Vector2 portraitMinAspectRatio	= new Vector2(9.0f, 16.0f);
	public Vector2 portraitMaxAspectRatio	= new Vector2(2.0f, 3.0f);

	Camera cam = null;
	int screenWidth							= 0;		// 0 forces camAspect to be set correctly on Awake.
	int screenHeight						= 0;

	int effectiveScreenWidth				= 0;
	int effectiveScreenHeight				= 0;

	float camAspect		= 1.0f;

	void Awake()
	{
		cam = GetComponent<Camera>();
		Update();
	}

	void OnValidate()
	{
		// Just to avoid divide by zero and non-zero cam aspect ratios.
		if (landscapeMinAspectRatio.x < 0.01f)	landscapeMinAspectRatio.x = 0.01f;
		if (landscapeMinAspectRatio.y < 0.01f)	landscapeMinAspectRatio.y = 0.01f;
		if (landscapeMaxAspectRatio.x < 0.01f)	landscapeMaxAspectRatio.x = 0.01f;
		if (landscapeMaxAspectRatio.y < 0.01f)	landscapeMaxAspectRatio.y = 0.01f;

		if (portraitMinAspectRatio.x < 0.01f)	portraitMinAspectRatio.x = 0.01f;
		if (portraitMinAspectRatio.y < 0.01f)	portraitMinAspectRatio.y = 0.01f;
		if (portraitMaxAspectRatio.x < 0.01f)	portraitMaxAspectRatio.x = 0.01f;
		if (portraitMaxAspectRatio.y < 0.01f)	portraitMaxAspectRatio.y = 0.01f;
	}

	public float GetAspect()
	{
		return camAspect;
	}

	public float GetScreenWidth()
	{
		return effectiveScreenWidth;
	}

	public float GetScreenHeight()
	{
		return effectiveScreenHeight;
	}

	void Update()
	{
		if ((screenWidth != Screen.width) || (screenHeight != Screen.height))
		{
			screenWidth		= Screen.width;
			screenHeight	= Screen.height;
			if ((screenWidth > 0) && (screenHeight > 0))
				OnAspectChanged();
		}
	}

	void ComputeCameraAspect(float screenAspect)
	{
		bool landscape = screenAspect > 1.0f;
		if (landscape)
		{
			float landscapeMinAspect = landscapeMinAspectRatio.x / landscapeMinAspectRatio.y;
			float landscapeMaxAspect = landscapeMaxAspectRatio.x / landscapeMaxAspectRatio.y;
			camAspect = Mathf.Clamp(screenAspect, landscapeMinAspect, landscapeMaxAspect);
		}
		else
		{
			float portraitMinAspect = portraitMinAspectRatio.x / portraitMinAspectRatio.y;
			float portraitMaxAspect = portraitMaxAspectRatio.x / portraitMaxAspectRatio.y;
			camAspect = Mathf.Clamp(screenAspect, portraitMinAspect, portraitMaxAspect);
		}
	}

	void OnAspectChanged()
	{
		float screenAspect = (float)screenWidth / (float)screenHeight;
		ComputeCameraAspect(screenAspect);

		// If vertBars is false there will be unused horizontal space.
		bool vertBars			= screenAspect > camAspect;
		Rect rect				= cam.rect;
		float camWH				= vertBars ? camAspect/screenAspect : screenAspect/camAspect;
		rect.width				= vertBars ? camWH : 1.0f;
		rect.height				= vertBars ? 1.0f : camWH;
		rect.x					= vertBars ? (1.0f-camWH)*0.5f : 0.0f;
		rect.y					= vertBars ? 0.0f : (1.0f-camWH)*0.5f;
		effectiveScreenWidth	= (int)(rect.width * (float)screenWidth);
		effectiveScreenHeight	= (int)(rect.height * (float)screenHeight);
		cam.rect				= rect;
	}
}
