using UnityEngine;

namespace ProjecterLib
{
    public static class CPUtility
    {

        static public void SetProjecterKeyword (Material mat, ProjecterMode mode)
        {
            var m = (int)mode;
            var keys = _ProjecterModeShaderKeywords;

            for (int i = 0; i < keys.Length; i++)
            {
                if (m == i)
                    mat.EnableKeyword(keys[i]);
                else
                    mat.DisableKeyword(keys[i]);
            } 
        }

        static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        static readonly int _FaceMatrix = Shader.PropertyToID("_FaceMatrix");
        static readonly int _FovRad = Shader.PropertyToID("_FovRad");
        static readonly int _FovExpansion = Shader.PropertyToID("_FovExpansion");
        static readonly int _Rect = Shader.PropertyToID("_Rect");
        static readonly int _EyeBlindSide = Shader.PropertyToID("_EyeBlindSide");

        static public void SetProjecterDirection (Material mat, Quaternion rotation)
        {
            mat.SetMatrix(_FaceMatrix, GetMatrix(rotation));
        }

        static public void SetProjectionFOV (Material mat, float degrees, float internalDegrees)
        {
            mat.SetFloat(_FovRad, degrees * Mathf.Deg2Rad * 0.25f);
            mat.SetFloat(_FovExpansion, Mathf.Tan(internalDegrees * 0.5f * Mathf.Deg2Rad));
        }

        static public void SetProjectionRect (Material mat, Rect r, float aspect)
        {
            mat.SetVector(_Rect, new Vector4(r.width * aspect, r.height, r.x, r.y));
        }

        /// <summary>
        /// 0 = don't mask, 1 = left off, 2 = right off
        /// </summary>
        static public void SetEyeBlindSide(Material mat, int value)
        {
            mat.SetInt(_EyeBlindSide, value);
        }

        static public void SetEyeBlindSide(Material mat, Camera.MonoOrStereoscopicEye mask)
        {
            mat.SetInt(_EyeBlindSide, (2 - (int)mask));
        }

        static public void SetTexture (Material mat, Texture tex)
        {
            mat.SetTexture(_MainTex, tex);
        }

        static public void Destroy (Object obj, bool recordUndo = false)
        {
            if (!obj)
                return;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (recordUndo)
                    UnityEditor.Undo.DestroyObjectImmediate(obj);
                else
                    Object.DestroyImmediate(obj);
            }
            else
            #endif
                Object.Destroy(obj);
        }

        static public void DestroyRenderTexture (RenderTexture tex)
        {
            if (tex)
            {
                tex.DiscardContents();
                tex.Release();
                Destroy(tex);
            }
        }

        public static Shader blitShaderEffect
        {
            get
            {
                return Shader.Find("Hidden/WideCameraProjectorShaderEffect");
            }
        }


        public static Shader blitShaderCubic
        {
            get
            {
                return Shader.Find("Hidden/WideCameraProjectorShaderCubic");
            }
        }


		public static Shader blitShaderCompositor
        {
            get
            {
                return Shader.Find("Hidden/WideCameraProjectorShaderCompositor");
            }
        }

        public static Rect SetAspectRect (Rect rect, float aspect)
        {
            //var center = rect.center;
            var size = rect.size;
            size.x *= aspect * size.y;
            return new Rect(rect.position, size);
        }

        public static bool isNowEvenFrame () {
            return Time.frameCount % 2 == 0;
        }

        public static Matrix4x4 GetMatrix (Quaternion q)
        {
            var m = Matrix4x4.identity;

            m.SetRow(0, q * Vector3.right);
            m.SetRow(1, q * Vector3.up);
            m.SetRow(2, q * Vector3.forward);

            return m;
        }

        static public string[] _ProjecterModeShaderKeywords = {
            "MAP_STEREOGRAPHIC",
            "MAP_EQUIDISTANT",
            "MAP_EQUISOLID",
            "MAP_ORTHOGRAPHIC",
            "","","","","","",  // Keep margin in case new one developed later
            "MAP_GEOGRAPHIC",
            "MAP_MILLER",
            "MAP_CASSINI",
        };
    }

    public enum ProjecterMode
    {
        //Azimuthal
        Stereographic = 0,
        Equidistant = 1,
        Equisolid = 2,
        Orthographic = 3,
        //Cylindrical
        Geographic = 10,
        Miller = 11,
        Cassini = 12,
    }

    public enum ProjecterShape
    {
        Cube = 0,
        Tethahedron = 1,
    }

    public enum AntiAliasValue
    {
        Off = 1,
        x2 = 2,
        x4 = 4,
        x8 = 8,
    }

    public enum GrabberBehaviour
    {
        Normal = 0,
        HalfRes = 1,
        DoubleRes = 2,
        Discarded = 3,
    }
        
}
