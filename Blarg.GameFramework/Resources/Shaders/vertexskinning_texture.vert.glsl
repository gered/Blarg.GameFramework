#ifdef GL_ES
	precision mediump float;
#endif

const int MAX_BONES = 50;

attribute vec4 a_position;
attribute vec2 a_texcoord0;
attribute float a_jointIndex;

uniform mat4 u_modelViewMatrix;
uniform mat4 u_projectionMatrix;
uniform vec3 u_jointPositions[MAX_BONES];
uniform vec4 u_jointRotations[MAX_BONES];

varying vec2 v_texCoord;

vec3 qtransform(vec4 q, vec3 v)
{
	vec3 temp = cross(q.xyz, v) + q.w * v;
	return cross(temp, -q.xyz) + dot(q.xyz,v) * q.xyz + q.w * temp;
}

void main()
{
	int j = int(a_jointIndex);

	vec4 skinnedPosition = vec4(qtransform(u_jointRotations[j], a_position.xyz) + u_jointPositions[j], 1.0);
	
	v_texCoord = a_texcoord0;
	gl_Position = u_projectionMatrix * u_modelViewMatrix * skinnedPosition;
}
