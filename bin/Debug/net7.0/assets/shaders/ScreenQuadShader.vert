#version 440 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aUV;

uniform vec2 scale;

out vec2 pass_uv;

void main(){
    gl_Position = vec4(aPos * vec3(scale.x,scale.y,1.0), 1.0);
    pass_uv = aUV;
}