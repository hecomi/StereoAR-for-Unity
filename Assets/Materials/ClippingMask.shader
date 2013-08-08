// See: http://unitygeek.hatenablog.com/entry/2013/04/01/164040

Shader "Masked/Mask" {
	
	SubShader {
		// Render the mask after regular geometry, but before masked geometry and
		// transparent things.
		
		Tags {"Queue" = "Geometry+10" }
		
		// Don't draw in the RGBA channels; just the depth buffer
		
		ColorMask 0
		ZWrite On
		
		// Do nothing specific in the pass:
		
		Pass {}
	}
}