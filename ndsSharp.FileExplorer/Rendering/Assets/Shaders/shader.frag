#version 460 core
out vec4 FragColor;

in vec3 fPosition;
in vec2 fTexCoord;
in vec3 fNormal;

uniform sampler2D diffuse;


void main()
{
    vec4 color = texture(diffuse, fTexCoord);
    if (color.a < 0.1) discard;
    
    FragColor = vec4(color.rgb, 1.0);
}