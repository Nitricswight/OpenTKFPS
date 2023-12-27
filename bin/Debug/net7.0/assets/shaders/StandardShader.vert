#version 440 core

layout(location = 0) in vec3 vertex;
layout(location = 1) in vec2 uv;
layout(location = 2) in vec3 normal;

out vec2 pass_uv;
out vec3 pass_normal;
out vec3 fragPos;

uniform mat4 transformation;
uniform mat4 projection;
uniform mat4 view;

void main(){
    gl_Position = projection * view * transformation * vec4(vertex, 1.0);
    fragPos = vec3(transformation * vec4(vertex, 1.0));
    pass_uv = uv;
    pass_normal = mat3(transpose(inverse(transformation))) * normal;
}