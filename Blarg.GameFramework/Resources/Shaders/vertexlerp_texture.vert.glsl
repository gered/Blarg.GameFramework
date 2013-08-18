attribute vec4 a_position1;
attribute vec4 a_position2;
attribute vec2 a_texcoord0;
uniform mat4 u_modelViewMatrix;
uniform mat4 u_projectionMatrix;
uniform float u_lerp;
varying vec4 v_color;
varying vec2 v_texCoords;

void main()
{
	v_texCoords = a_texcoord0;
	gl_Position =  u_projectionMatrix * u_modelViewMatrix * mix(a_position1, a_position2, u_lerp);
}
