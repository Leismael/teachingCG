﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GMath
{
    public static partial class Gfx
    {
        #region cross
        public static float3 cross(float3 pto1, float3 pto2)
        {
            return new float3(
                pto1.y * pto2.z - pto1.z * pto2.y,
                pto1.z * pto2.x - pto1.x * pto2.z,
                pto1.x * pto2.y - pto1.y * pto2.x);
        }
        #endregion

        #region determinant

        public static float determinant(float1x1 m)
        {
            return m._m00;
        }

        public static float determinant(float2x2 m)
        {
            return m._m00 * m._m11 - m._m01 * m._m10;
        }

        public static float determinant(float3x3 m)
        {
            /// 00 01 02
            /// 10 11 12
            /// 20 21 22
            float Min00 = m._m11 * m._m22 - m._m12 * m._m21;
            float Min01 = m._m10 * m._m22 - m._m12 * m._m20;
            float Min02 = m._m10 * m._m21 - m._m11 * m._m20;

            return Min00 * m._m00 - Min01 * m._m01 + Min02 * m._m02;
        }

        public static float determinant(float4x4 m)
        {
            float Min00 = m._m11 * m._m22 * m._m33 + m._m12 * m._m23 * m._m31 + m._m13 * m._m21 * m._m32 - m._m11 * m._m23 * m._m32 - m._m12 * m._m21 * m._m33 - m._m13 * m._m22 * m._m31;
            float Min01 = m._m10 * m._m22 * m._m33 + m._m12 * m._m23 * m._m30 + m._m13 * m._m20 * m._m32 - m._m10 * m._m23 * m._m32 - m._m12 * m._m20 * m._m33 - m._m13 * m._m22 * m._m30;
            float Min02 = m._m10 * m._m21 * m._m33 + m._m11 * m._m23 * m._m30 + m._m13 * m._m20 * m._m31 - m._m10 * m._m23 * m._m31 - m._m11 * m._m20 * m._m33 - m._m13 * m._m21 * m._m30;
            float Min03 = m._m10 * m._m21 * m._m32 + m._m11 * m._m22 * m._m30 + m._m12 * m._m20 * m._m31 - m._m10 * m._m22 * m._m31 - m._m11 * m._m20 * m._m32 - m._m12 * m._m21 * m._m30;

            return Min00 * m._m00 - Min01 * m._m01 + Min02 * m._m02 - Min03 * m._m03;
        }

        #endregion

        #region faceNormal
        public static float3 faceNormal(float3 normal, float3 direction)
        {
            return dot(normal, direction) > 0 ? normal : -normal;
        }
        #endregion

        #region lit
        public static float4 lit(float NdotL, float NdotH, float power)
        {
            return new float4(1, NdotL < 0 ? 0 : NdotL, NdotL < 0 || NdotH < 0 ? 0 : (float)Math.Pow(NdotH, power), 1);
        }
        #endregion

        #region reflect

        /// <summary>
        /// Performs the reflect function to the specified float3 objects.
        /// Gets the reflection vector. L is direction to Light, N is the surface normal
        /// </summary>
        public static float3 reflect(float3 L, float3 N)
        {

            return 2 * dot(L, N) * N - L;

        }

        #endregion

        #region refract

        /// <summary>
        /// Performs the refract function to the specified float3 objects.
        /// Gets the refraction vector.
        /// L is direction to Light, N is the normal, eta is the refraction index factor.
        /// </summary>
        public static float3 refract(float3 L, float3 N, float eta)
        {
            float3 I = -1 * L;

            float cosAngle = dot(I, N);
            float delta = 1.0f - eta * eta * (1.0f - cosAngle * cosAngle);

            if (delta < 0)
                return new float3(0, 0, 0);

            return normalize(eta * I - N * (eta * cosAngle + (float)Math.Sqrt(delta)));
        }

        #endregion

        #region ortho basis

        public static float copysign(float f, float t)
        {
            return (float)Math.CopySign(f, t);
        }

        /// <summary>
        /// Given a direction, creates two othonormal vectors to it.
        /// From the paper: Building an Orthonormal Basis, Revisited, Tom Duff, et.al.
        /// </summary>
        public static void createOrthoBasis(float3 N, out float3 T, out float3 B)
        {
            float sign = copysign(1.0f, N.z);
            float a = -1.0f / (sign + N.z);
            float b = N.x * N.y * a;
            T = float3(1.0f + sign * N.x * N.x * a, sign * b, -sign * N.x);
            B = float3(b, sign + N.y * N.y * a, -N.y);
        }

        #endregion

        #region Randoms

        static GRandom __random = new GRandom();

        /// <summary>
        /// Devuelve un numero entre 0 y 1
        /// </summary>
        /// <returns></returns>
        public static float random()
        {
            return __random.random();
        }

        public static float2 random2()
        {
            return __random.random2();
        }

        /// <summary>
        /// Devuelve puntos random dentro de un cubo
        /// Sus valores se encuentran entre -0.5 y 0.5
        /// </summary>
        /// <returns></returns>
        public static float3 randomInBox()
        {
            // 'u' y y 'v' estan entre -1 y 1
            // float u = random() * 2 - 1, v = random() * 2 - 1;
            float u = random() - .5f, v = random() -.5f;
            //Devuelve un numero entre 0 y 6
            switch ((int)(random() * 6)) 
            {
                case 0: return float3(u, v, -.5f); // negZ
                case 1: return float3(u, v, .5f); // posZ
                case 2: return float3(u, -.5f, v); // negY
                case 3: return float3(u, .5f, v); // posY
                case 4: return float3(-.5f, u, v); // negX
                case 5: return float3(.5f, u, v); // posX
            }
            return float3(0, 0, 0); // should never occur... but compiler doesn't know...
        }

        #endregion
    }
}
