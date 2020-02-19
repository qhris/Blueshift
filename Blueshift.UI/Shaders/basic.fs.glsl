#version 330 core
out vec4 FragColor;

in vec2 TexCoord0;

uniform vec3 foregroundColor = vec3(1);
uniform sampler2D textureSampler;

void main()
{
    vec3 diffuse = texture(textureSampler, TexCoord0).rgb;
    FragColor = vec4(diffuse * foregroundColor, 1);
}
