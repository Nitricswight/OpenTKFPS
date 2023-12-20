#version 440 core

in vec2 pass_uv;

uniform sampler2D texture0;

out vec4 out_colour;

void main(){
    out_colour = texture(texture0,pass_uv);
}