Beautify for Universal Rendering Pipeline!

Requirements:
- Unity 2020.3.16 or later
- Universal RP 10.5.1 or later (install it from Package Manager)

Please check the Documentation folder for detailed setup instructions and more details.
To use Beautify, add Beautify override to a Post Processing Volume and customize!
Setup video: https://youtu.be/6fpeiysj6KM


Change Log
----------

Current version
- Chromatic Aberration: added "Separate Pass" option
- Bloom & anamorphic exclude layer options now support transparent objects

Version 10.4
- Anamorphic Flares: added Exclusion Mask options
- Bloom & Anamorphic Flares: removed upper cap to threshold parameters
- ACES: added "Max Input Brightness" option to avoid artifacts due to NaN or out of range pixel values
- Added option to demo scene DoF to toggle the effect
- [Fix] Fixed Bloom customize option issue in VR when using multiple cameras

Version 10.3
- Added "Downscale Mode" option
- [Fix] Fixed LUT effect being stripped from build

Version 10.2
- Edge Antialias: added "Max Spread" option

Version 10.1
- Added "Motion Restore Speed". Improved accuracy of motion sensibility.
- Edge Antialias: added "Depth Attenuation". Reduces antialias effect on distance

Version 10.0
- Added "Edge Antialiasing" option
- Frame: added "Cinematic Bands" style
- Bloom: added "Bloom Spread" option
- Bloom: added "Quicker Blur" option
- Bloom: uncapped "Depth Attenuation" limit
- Anamorphic Flares: added "Quicker Blur" option
- Anamorphic Flares: uncapped "Depth Attenuation" limit
- Outline: added "Outline Depth Fade" option (requires "Outline Customize" to be enabled)
- Chromatic Aberration: added "Hue Shift" parameter
- Chromatic Aberration: added CHROMATIC_ABERRATION_ALT shader option (see documentation)
- Depth of field: improved foreground blur effect
- Depth of field: improved bokeh effect in Single Pass Instanced mode
- Added "Camera Layer Mask" to the render feature. This option let you specify which cameras can render Beautify effects
- Volume inspector GUI performance optimizations
- [Fix] Fixed bloom & anamorphic flares not showing in secondary camera on VR setups
- [Fix] Fixes for Unity 2022.2 beta

Version 9.0.1
- Direct Write to Camera option works again (requires Unity 2021.3.3 or later)

Version 9.0
- Added "Ignore Post Processing Option" in Beautify Render Feature so no need to enable Post Processing option in cameras
- Added "Flip Vertically" option to compensate vertical flip in 2D renderer with camera stacking
- [Fix] Sun flares now use the direction set by the assigned Sun transform and not the main directional light
- [Fix] Fixed flipped input image with 2D renderer and camera stacking

Version 8.9
- Added new options to compare mode
- Added LUT 3D texture support and option to import CUBE LUT format

Version 8.8.1
- Change: adjusted opacity of vignette mask plus vignette color alpha now controls overall opacity as well

Version 8.8
- Depth of field: added real camera settings

Version 8.7
- Depth of field: added "Transparent Alpha Test Support" options
- Added "Render Pass Event" option to the Beautify Render Feature inspector

Version 8.6.3
- Final Blur now applies after depth of field
- Added "Double Sided" option to transparent depth of field option
- [Fix] Fixed inspector issue which hides chromatic aberration section when lens dirt feature is stripped

Version 8.6.2
- [Fix] BeautifySettings gameobject is no longer created if Beautify is not being used in the scene when camera post processing is enabled

Version 8.6.1
- [Fix] Fixes to Sun Flares effect in VR

Version 8.6
- Added Frame Pack browser

Version 8.5.1
- LUT browser UI improvements

Version 8.5
- Depth of Field: added Composition option for bokeh
- Added Depth of Field demo scene

Version 8.4
- Added LUT Browser (access it from the Windows menu)

Version 8.3.1
- Beautify cached profiles get updated now automatically when loading new scenes

Version 8.3
- Added "Blur Mask" option to Final Blur effect
- [Fix] Fixed some issues with Unity 2021.2 beta

Version 8.2
- Added new Outline options
- Version number upped to 8.2 to sync with built-in version

Version 2.0
- Added Chromatic Aberration effect
- [Fix] Fixed blink method issue when changing scenes

Version 1.7.3 18/Mar/2021
- DoF: added blur spread option to foreground blur
- [Fix] Fixed depth of field CoC radius calculation issue when using multiple cameras

Version 1.7.2 25/Feb/2021
- [Fix] Fixed Single Pass Stereo/MultiView issues due to Blit bug on XR
- [Fix] Fixed transparent support for Depth of Field not rendering in Editor

Version 1.7.1 8/Feb/2021
- [Fix] Fixed depth of field issue on Android with Unity 2020.2

Version 1.7 24/Jan/2021
- Added support for orthographic camera

Version 1.6
- Added "Vignetting Blink Style" option
- Added "Vignetting Center" option
- Added "Bloom Near Attenuation" option
- Added "Anamorphic Flares Near Attenuation" option
- Added new debug layers to Debug View

Version 1.5
- Added Depth Of Field Transparent Support option

Version 1.4
- Added Sun Flares "Occlusion Layer Mask" option
- Added Sun Flares "Attenuation Speed" (works with Occlusion Layer Mask option)
- [Fix] Fixed an issue that could produce Beautify to use a disabled camera when computing Sun Flares effect

Version 1.3.1 15/NOV/2020
- Improved compatibility with URP 10.1
- [Fix] Fixed an issue that prevents correct shader keyword stripping (ie. cloud build)

Version 1.3 18/OCT/2020
- Added "Bloom Exclusion Mask" option
- Added new demo scene "LUT Blending"
- [Fix] Fixed regression which disabled sharpen in build

Version 1.2.2 23/SEP/2020
- Optimized scriptable render pass initialization

Version 1.2.1 31/AGO/2020
- Support for LUT textures of 256x8 size
- [Fix] Fixed DoF material memory leak

Version 1.2 24/JUL/2020
- Added bloom color tint option under "Customize Bloom" section
- [Fix] Inspector fixes

Version 1.1 28/MAY/2020
- Added "Downscaling" option to Optimizations section

Version 1.0.2 19/MAY/2020
- Added Depth of Field "Distance Shift" parameter

Version 1.0.1 1/MAY/2020
- [Fix] Fixed max clamp values for some sharpen parameters

Version 1.0 April/2020
- Tested on Windows, Mac, Android.
- Added VR Single Pass Stereo support (tested with Oculus Quest)
- Added Beautify and Unity Post Processing build optimization options
- Added Best Performance Mode
- Added Final Blur effect
- Added White Balance color grading option
- Added Night Vision effect
- Added "Direct Write To Camera" option in Performance section


