Distortion Shader Pack
by Martin Reintges
----------------------

The main goal of this package is to provide a simple and cheap package to use auto generated screen-space render-texture distortion. 


Showcase
--------
Assets/DistortionShaderPack/Scenes/Demo Setup.unity -> How to create a distortion plane from scratch
(coming soon) Assets/DistortionShaderPack/Scenes/Demo Presentation.unity -> A simple scene showing some use cases
Assets/DistortionShaderPack/Scenes/Demo Configuration.unity -> A couple of possible effect configurations
Assets/DistortionShaderPack/Scenes/Demo Code.unity -> How to manipulate the shader by code


Workflow distortion plane
-------------------------
1. Drag&Drop the DistortionPlane (Assets/DistortionShaderPack/Prefabs/DistortionPlane.prefab) into your scene.
2. Configure the render texture quality using the RenderTextureCamera script parameter
3. Configure the distortion using the attached Material parameter

Variants
--------
Please notice that there are different shader variants. The main difference is between a RenderTexture-shader or a GrabTexture-shader. 
The RenderTexture version should have a minimal performacne advantage. But the GrabTexture versions are more reliable.
HDR - if HDR is enabled in your project please make sure to use shader variants that end with HDR to make sure they are compatibel.

How it works
------------
The shader is working with the "Quad"-mesh and uses it as render target
For each camera in the scene that is rendering the target GameObject the RenderTextureCamera script creates a new camera.
This virtual(not saved) camera renders the same image using the defined parameter, but saves it into the render-texture.
The created texture can then be used as input for the distortion shader.
Depending on the alpha settings the render target can be partially transparent.
 ! Important: The script only create one virtual camera per real camera to minimize the rendering time.

 Nice to know
 ------------
 User feedback (pmcanneny):
 When using with MKGlow, make sure your camera's MKGlow has [Show Transparent] enabled.
 When using with custom or dynamic skyboxes such as StarNest, 
 put 'go.AddComponent<Skybox>(mainCamera.GetComponent<Skybox>());' in RenderTextureCamera.cs in the GetOrCreateCamera method.  
 When needing to debug, comment out 'go.hideFlags = HideFlags.HideAndDontSave;' in RenderTextureCamera.cs


Contact
-------
Mail: mailnightowl@gmail.com
Website: reintges.webs.com