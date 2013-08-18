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
	vec4 finalColor;
	if (u_textureHasAlphaOnly)
		finalColor = vec4(v_color.xyz, (v_color.a * texture2D(u_texture, v_texCoords).a));
	else
		finalColor = v_color * texture2D(u_texture, v_texCoords);
	gl_FragColor = finalColor;
}
