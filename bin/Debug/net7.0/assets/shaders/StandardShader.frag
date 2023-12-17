#version 440 core

in vec2 pass_uv;
in vec3 pass_normal;

uniform vec4 albedo;

uniform sampler2D texture0;

uniform int albedoTextureEnabled;

uniform float time;

out vec4 colour;

void main(){
    vec4 tex = vec4(1,1,1,1);
    if(albedoTextureEnabled == 1){
        tex = texture(texture0, pass_uv);
    }

    if(tex.a == 0){
        discard;
    }
    colour = vec4(tex.rgb * (albedo.rgb), 1.0);
}