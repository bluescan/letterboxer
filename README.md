# letterboxer
Letterboxing for Unity

A Unity component that you attach to your scene cameras to allow letter (and pillar) boxing.

Instructions:

1. Attach component to any cameras you have in your scenes. If you are using the Unity UI system, attach it also to any canvas cameras.
2. Specify min and max aspect ratios for landscape (width > height) and portrait (height > width) in the inspector.
3. Any code that needs the effective screen resolution or aspect ratio should call members of this component rather than Screen.width and Screen.height.
4. Put a 'background' orthographic camera in your scene with 'Solid Color' Clear Flags and Nothing in the Culling Mask. Set the 'depth' really low, like -100 or something.
