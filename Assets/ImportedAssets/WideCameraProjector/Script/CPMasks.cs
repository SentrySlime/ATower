using UnityEngine;

namespace ProjecterLib
{
    public class CPMasks
    {

        public const float OnePerThreeSQRT = 0.57735f;

        static public Vector3[] vectors;

        static public int[] masks;

        static CPMasks()
        {
            vectors = new Vector3[8];
            masks = new int[8];
            int i = 0;
            for (int x = 0; x <= 1; x++)
                for (int y = 0; y <= 1; y++)
                    for (int z = 0; z <= 1; z++)
                    {
                        // order should be..
                        // (1,1,1) = 1+4+16
                        // (1,1,-1) = 1+4+32
                        // (1,-1,1) = 1+8+16
                        // ...
                        vectors[i] = new Vector3(x * -2 + 1, y * -2 + 1, z * -2 + 1).normalized;
                        masks[i] = (1 << x) + (4 << y) + (16 << z);
                        i++;
                    }
        }

        /// <summary>
        /// Faster way to inverse rotation
        /// </summary>
        static Quaternion Transpose(Quaternion q) { return new Quaternion(-q.x, -q.y, -q.z, q.w); }

        static int GetSingleFwdMask(Vector3 v)
        {
            const float m = 0.57735f;
            if (v.x >= m)
                return 1;
            else if (v.x <= -m)
                return 2;
            else if (v.y >= m)
                return 4;
            else if (v.y <= -m)
                return 8;
            else if (v.z >= m)
                return 16;
            else if (v.z <= -m)
                return 32;
            else
                return 0;
        }

        public static int GetPossibleMasks(Quaternion view, ProjecterMode mode, float fovrad, float aspect)
        {
            int mask = 0;
            float wide = 0;
            Vector3 fwd = view * Vector3.forward;
            fovrad *= 0.5f;
            aspect = Mathf.Max(1, aspect); // always assume longer than height
            switch (mode)
            {
                case ProjecterMode.Equidistant:
                case ProjecterMode.Equisolid:
                case ProjecterMode.Stereographic:
                    if (fovrad * aspect > (Mathf.PI / 3)) return 63; // Fail Fast at above 120 degree

                    wide = Mathf.Cos(Mathf.Min(Mathf.PI, new Vector2(fovrad, fovrad * aspect).magnitude));
                    for (int i = 0; i < 8; i++)
                    {
                        if (Vector3.Dot(vectors[i], fwd) > wide)
                            mask |= masks[i];
                    }
                    // When mask == 0 means the camera didn't see any corner so
                    // we have to do edge detection. However I don't know how to
                    // do it efficiently so just mask side that behind camera.
                    // (we should can do much better than this)
                    return mask == 0 ? 63 & ~GetSingleFwdMask(-fwd) : mask;

                case ProjecterMode.Orthographic:
                    if (fovrad * aspect > (Mathf.PI / 3)) wide = 0; // Fail Fast at above 120 degree
                    else wide = Mathf.Cos(Mathf.Min(Mathf.PI / 2, new Vector2(fovrad, fovrad * aspect).magnitude));
                    for (int i = 0; i < 8; i++)
                    {
                        if (Vector3.Dot(vectors[i], fwd) > wide)
                            mask |= masks[i];
                    }
                    return mask == 0 ? 63 & ~GetSingleFwdMask(-fwd) : mask;

                case ProjecterMode.Geographic:
                case ProjecterMode.Miller:
                case ProjecterMode.Cassini:
                default:
                    // geographic projections: coming soon.
                    return 63;
            }
        }
    }
}
