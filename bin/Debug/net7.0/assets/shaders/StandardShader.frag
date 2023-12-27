#version 440 core

struct DIRECTIONAL_LIGHT{
    vec3 dir;
    float intensity;
    vec4 colour;
};

struct POINT_LIGHT{
    vec3 pos;
    vec4 colour;
    float intensity;
    float range;
    float falloff;
};

struct LIGHT_DATA{
    DIRECTIONAL_LIGHT directionalLight;
    POINT_LIGHT pointLights[64];
};

in vec2 pass_uv;
in vec3 pass_normal;
in vec3 fragPos;

uniform vec4 albedo;

uniform sampler2D texture0;

uniform int albedoTextureEnabled;

uniform float time;

uniform LIGHT_DATA lightData;

out vec4 colour;

vec3 CalculateDirectionalLight(DIRECTIONAL_LIGHT light, vec3 norm){
    float diff = max(dot(norm, light.dir), 0.0) * light.intensity;
    return light.colour.rgb * diff;
}

vec3 CalculatePointLight(POINT_LIGHT light, vec3 norm){
    vec3 lightDir = normalize(light.pos - fragPos);

    float diff = max(dot(norm, lightDir), 0.0);

    float dist = length(light.pos - fragPos);
    float s = dist / light.range;
    float attenuation;

    float ss = s * s;

    if(s >= 1.0){
        attenuation = 0.0;
    }
    else{
        attenuation = light.intensity * (((1.0 - ss) * (1.0 - ss)) / 1.0 + (light.falloff * ss));
    }

    return light.colour.rgb * diff * attenuation;
}

void main(){
    vec3 norm = normalize(pass_normal);
    vec4 tex = vec4(1,1,1,1);
    if(albedoTextureEnabled == 1){
        tex = texture(texture0, pass_uv);
    }

    if(tex.a == 0){
        discard;
    }

    //phase 1: directional lighting
    vec3 diffTotal = vec3(0.1,0.1,0.1) + CalculateDirectionalLight(lightData.directionalLight, norm);

    for(int i = 0; i < 64; i++){
        if(length(lightData.pointLights[i].colour) > 0.01){
            diffTotal += CalculatePointLight(lightData.pointLights[i], norm);
        }
    }

    colour = tex * albedo * vec4(diffTotal, 1.0);
}