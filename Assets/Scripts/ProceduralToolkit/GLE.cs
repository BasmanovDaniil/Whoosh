using UnityEngine;

namespace ProceduralToolkit
{
    public static class GLE
    {
        public static readonly Material material = new Material("Shader \"Lines/Colored Blended\" {" +
                                                                "SubShader { Pass {" +
                                                                "   BindChannels { Bind \"Color\",color }" +
                                                                "   Blend SrcAlpha OneMinusSrcAlpha" +
                                                                "   ZWrite Off Cull Off Fog { Mode Off }" +
                                                                "} } }")
        {
            hideFlags = HideFlags.HideAndDontSave,
            shader = {hideFlags = HideFlags.HideAndDontSave}
        };

        /// <summary>
        /// GL.PushMatrix();
        /// material.SetPass(0);
        /// GL.LoadOrtho();
        /// </summary>
        public static void BeginOrtho()
        {
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadOrtho();
        }

        /// <summary>
        /// GL.PushMatrix();
        /// material.SetPass(0);
        /// GL.LoadPixelMatrix();
        /// </summary>
        public static void BeginPixel()
        {
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadPixelMatrix();
        }

        /// <summary>
        /// GL.PushMatrix();
        /// material.SetPass(0);
        /// GL.LoadPixelMatrix(left, right, bottom, top);
        /// </summary>
        public static void BeginPixel(float left, float right, float bottom, float top)
        {
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadPixelMatrix(left, right, bottom, top);
        }

        public static void End()
        {
            GL.PopMatrix();
        }
    }
}