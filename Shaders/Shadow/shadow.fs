#version 330 core

in vec4 posInLight;
in vec2 TexCoords;
in vec3 Normal;
in vec3 posInWorld;

uniform vec3 lightPos;
uniform vec3 viewPos;

uniform sampler2D diffuseTexture;
uniform sampler2D shadowMap;

out vec4 FragColor;

float ShaowOrNot(vec4 posInLight){
	vec3 projCoords = posInLight.xyz / posInLight.w;
	projCoords = projCoords * 0.5 + 0.5;
	float closetDepth = texture(shadowMap, projCoords.xy).r;
	float currentDepth = projCoords.z;
	
	float shadow = currentDepth > closetDepth ? 1.0 : 0.5;
	return shadow;
}

void main(){
	vec3 color = texture(diffuseTexture, TexCoords).rgb;
	vec3 norm = normalize(Normal);
	vec3 lightColor = vec3(0.8);
	
	//ambient
	vec3 ambient = 0.3 * color;
	
	//diffuse
	vec3 lightDir = normalize(posInWorld - lightPos);
	float diff = max(0.0, dot(norm, lightDir));
	vec3 diffuse = diff * lightColor;
	
	//specular
	vec3 viewDir = normalize(viewPos - posInWorld);
	vec3 reflectDir = reflect(-lightDir, norm);
	vec3 halfwayDir = normalize(lightDir + viewDir);
	float spec = 0.0;
	spec = pow(max(0.0, dot(norm, halfwayDir)), 64.0);
	vec3 specular = spec  *lightColor;
	
	//shadow
	float shadow = ShaowOrNot(posInLight);
	shadow = 0;
	vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;
	
	FragColor = vec4(lighting, 1.0);
}