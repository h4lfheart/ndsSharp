#version 460 core
out vec4 FragColor;

in vec3 fPosition;
in vec2 fTexCoord;
in vec3 fNormal;

uniform sampler2D diffuse;
uniform float alpha;


void main()
{
    vec4 texture = texture(diffuse, fTexCoord);
    float effectiveAlpha = texture.a * alpha;
    
    if (effectiveAlpha < 0.01) discard;
    
    vec3 color = texture.rgb;
    color *= mix(dot(fNormal, vec3(0, 1, 0)), 1, 0.75);
    
    FragColor = vec4(color, effectiveAlpha);
}