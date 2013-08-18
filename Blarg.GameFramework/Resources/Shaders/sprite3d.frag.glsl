#ifdef GL_ES
	#define LOWP lowp
	precision mediump float;
#else
	#define LOWP
#endif
varying LOWP vec4 v_color;
varying vec2 v_texCoords;
uniform sampler2D u_texture;
uniform bool u_textureHasAlphaOnly;

void main()
{
	vec4 texColor = texture2D(u_texture, v_texCoords);
	if (texColor.a > 0.0)
	{
		vec4 finalColor;
		if (u_textureHasAlphaOnly)
			finalColor = vec4(v_color.xyz, (v_color.a * texColor.a));
		else
			finalColor = v_color * texColor;
		gl_FragColor = finalColor;
	}
	else
		discard;
}
