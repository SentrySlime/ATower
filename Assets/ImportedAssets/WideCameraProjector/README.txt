Wide Camera Projector was known as Camera Projector before v3.0. 

I haven't updated the documentation but I believe most settings are still same.

Anyway, thanks for purchasing this asset! Contact me willnode@wellosoft.net or visit wellosoft.net if you have questions.

---

Release History:

V3.0 – March 24, 2021
- NEW: Use scroll wheel to adjust FOV in examples
- FIX: Updated to support latest version of unity
- FIX: No more need standard assets to run examples
- CHANGE: Minimum unity version increased to 2019.3
- CHANGE: Camera Projecter -> Wide Camera Projector

V2.0 – June 15, 2018
- NEW: VR Support (separate eye rendering)
- NEW: Selective rendering (results in performance improvements for small FOV)
- CHANGE: Removed separate six camera setup – now only use cubemap rendering
- CHANGE: Improved some projection offsets – some adjustment may needed after 
upgrade.
- CHANGE: Minimum unity version increased to 2018.1

V1.2 – June 5, 2017
- NEW: Undefined regions in some projection is explicitly masked out
- FIX: OnPreRender/Cull confusion. Significant improvement in performance expected.

V1.1 - May 23, 2017
- NEW: Cubemap rendering via Camera.RenderToCubemap()
- NEW: Tetrahedral camera setup which uses only 4 camera instead of 6!
- NEW: Support for rendering to RenderTexture
- NEW: Filter and Antialiasing choices for internal render textures
- NEW: Individual camera (aka. grabbers) now can be disabled or change the 
resolution separately 
- NEW: Grabbers can be adjusted manually in scene
- NEW: CP now can be statically updated instead of every frame
- NEW: Scene example improvements with trees and bushes
- FIX: Shader doesn't compile in OpenGL ES 2.0

V1.0 - May 11, 2017
- First Release
