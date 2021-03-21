using System;
using GMath;
using Renderer.Rendering;
using static GMath.Gfx;
using System.Diagnostics;
using System.Reflection;
using Renderer.Utils;

namespace Renderer
{
    public struct BoxData
    {
        private float3 position;
        private float3 scale;

        private float3[] pointsInBox;
        public float3[] PointsInBox
        {
            get
            {
                var translateMat = Transform.Translate(position);
                var scaleMat = Transform.Scale(scale);
                // var matrix = translateMat*scaleMat;
                var matrix = mul(translateMat, scaleMat);
                return ApplyTransform(pointsInBox, matrix);
            }
            set
            {
                if (value == null) return;
                pointsInBox = value;
            }
        }

        public BoxData(float3 position, float3 scale, int amountOfPoints)
        {
            this.position = position;
            this.scale = scale;
            this.pointsInBox = RandomPositionsInBoxSurface(amountOfPoints);
            PointsInBox = this.pointsInBox;
        }


        public float3[] GetTransformedPoints(float4x4 parentTransformation, float4x4 matrix)
        {
            var translateMat = Transform.Translate(position);
            var scaleMat = Transform.Scale(scale);
            var mulMatrix = mul(translateMat, scaleMat);
            var points = ApplyTransform(pointsInBox, mulMatrix);
            points = ApplyTransform(points, parentTransformation);
            return ApplyTransform(points, matrix);
        }

        /// <summary>
        /// Devuelve los puntos tranformados por la matrix
        /// </summary>
        /// <param name="matrix">Matrix de transformacion</param>
        /// <returns></returns>
        public float3[] GetTransformedPoints(float4x4 matrix)
        {
            return ApplyTransform(PointsInBox, matrix);
        }

        /// <summary>
        /// Aplica una transformacion a todos los puntos
        /// </summary>
        /// <param name="points">El array de puntos</param>
        /// <param name="matrix">La matrix de la tranformacion</param>
        /// <returns></returns>
        public static float3[] ApplyTransform(float3[] points, float4x4 matrix)
        {
            float3[] result = new float3[points.Length];

            // Transform points with a matrix
            // Linear transform in homogeneous coordinates
            for (int i = 0; i < points.Length; i++)
            {
                float4 h = float4(points[i], 1);
                h = mul(h, matrix);
                result[i] = h.xyz / h.w;
            }

            return result;
        }

        /// <summary>
        /// Crea puntos randoms dentro de un cubo con valores entre -1 y 1
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static float3[] RandomPositionsInBoxSurface(int N)
        {
            float3[] points = new float3[N];

            for (int i = 0; i < N; i++)
                points[i] = randomInBox();

            return points;
        }

    }

    public struct Camera
    {
        public float3 position;
        private float3 viewVector;

        public float3 ViewVector
        {
            get => viewVector;
            set
            {
                viewVector = normalize(value);
            }
        }

        public Camera(float3 position, float3 viewVector)
        {
            this.position = position;
            this.viewVector = normalize(viewVector);
        }

        /// <summary>
        /// Devuelve la matriz de la vista de la camara
        /// </summary>
        /// <returns></returns>
        public float4x4 GetViewMatrix()
        {
            float3 upVector = float3(0, 1, 0);
            float3 xaxis = normalize(cross(upVector, viewVector));
            float3 yaxis = normalize(cross(viewVector, xaxis));

            return float4x4(
                xaxis.x, yaxis.x, viewVector.x, 0,
                xaxis.y, yaxis.y, viewVector.y, 0,
                xaxis.z, yaxis.z, viewVector.z, 0,
                -dot(xaxis, position), -dot(yaxis, position), -dot(viewVector, position), 1
            );
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Raster render = new Raster(736, 920);
            // FreeTransformTest(render);
            // SomeCubes(render);
            MyScene(render);
            render.RenderTarget.Save("test.rbm");
            Console.WriteLine("Done.");

            #region Esta parte del codigo es solo para que automaticamente me abra el codigo de python

            string currentPath = Assembly.GetEntryAssembly().Location;
            //La clase PathHelper es creada por mi y solo es para tener una ayuda con los directorios
            PathHelper helper = new PathHelper(currentPath);
            //Aqui cambia la ruta del directorio subiendo 6 directorios mas arriba
            helper.ChangeCurrentPath(helper.GetParent(5));
            //Aqui concateno la ruta padre con los directorios hijos
            //y termino formando la ruta completa hasta el archivo de Python
            helper.Combine(@"Viewer\Python\imageviewer.py");
            //Crea una instancia con la info del proceso
            ProcessStartInfo start = new ProcessStartInfo();
            //Esta linea siguiente solo funcionara si tienes el python en el Path
            start.FileName = @"Python.exe";
            //Esta linea solo pone la ruta del archivo de python que se quiere ejecutar
            start.Arguments = $"\"{helper}\"";
            //Esto es para que el proceso sea creado desde el archivo ejecutable
            start.UseShellExecute = false;
            //Instancia un nuevo proceso con esa info
            Process process = Process.Start(start);
            Console.ReadKey();
            // Libera la memoria de ese proceso
            process.Dispose();

            #endregion

            Console.ReadKey();
        }

        public static float3[] RandomPositionsInBoxSurface(int N)
        {
            float3[] points = new float3[N];

            for (int i = 0; i < N; i++)
                points[i] = randomInBox();

            return points;
        }

        public static void MyScene(Raster render)
        {
            render.ClearRT(float4(0, 0, 0.34f, 1)); // clear with color dark blue.
            // render.ClearRT(float4(17f/255,51f/255,90f/255,1));
            // render.ClearRT(1);
            Camera cam = new Camera(float3(-5.85f, 4.7f, -11.11f), /*float3(-4.96f, 1.09f, 0.2f)-float3(-5.85f, 3.7f, -11.11f)*/float3(0.4f, 0, 0.9f));
            var viewMatrix = cam.GetViewMatrix();
            var projectingMatrix = Transform.PerspectiveFovLH(pi / 3, render.RenderTarget.Height / (float)render.RenderTarget.Width, 0.01f, 1000);
            var matrix = mul(viewMatrix, projectingMatrix);

            #region Backstage

            var color0 = float4(94f/255,99f/255,92f/255,1);
            var closet = new BoxData(float3(-1.5f, .54f, 2.9f), float3(3.77f, 11.3f, 2.19f), 300000);
            var points = closet.GetTransformedPoints(matrix);
            render.DrawPoints(points,color0);

            var closet2 = new BoxData(float3(2.8f, .54f, 2.9f), float3(2.5f, 11.3f, 2.19f), 100000);
            points = closet2.GetTransformedPoints(matrix);
            render.DrawPoints(points,color0);

            var repisa = new BoxData(float3(.2f, 1.84f, 2.9f), float3(4.2f, 5f, 2.19f), 100000);
            points = repisa.GetTransformedPoints(matrix);
            render.DrawPoints(points,.9f);

            var meseta = new BoxData(float3(.1f, .64f, 2.9f), float3(9.2f, 3.1f, 2.19f), 80000);
            points = meseta.GetTransformedPoints(matrix);
            render.DrawPoints(points,color0);

            #endregion

            #region Mesa Central

            var traslacion = float3(-.9f, 1.63f, -2.9f);
            // matrix = mul(Transform.Translate(traslacion), matrix);
            var cube0 = new BoxData(float3(0.459f, 0.21f, 0.2f), float3(9.8f, 2.75f, 2.86f), 200000);
            var color = float4(204f/255,189f/255,172f/255,1);
            // var points = cube0.GetTransformedPoints(matrix);
            points = cube0.GetTransformedPoints(Transform.Translate(traslacion), matrix);
            render.DrawPoints(points, color);
            var cube1 = new BoxData(float3(-.7f, 1.45f, 0.2f), float3(2.1f, 1, 2.86f), 20000);
            points = cube1.GetTransformedPoints(Transform.Translate(traslacion), matrix);
            render.DrawPoints(points, color);
            var cube2 = new BoxData(float3(-7.3f, -.09f, -1.05f), float3(0.3f, 2.27f, 0.33f), 10000);
            points = cube2.GetTransformedPoints(Transform.Translate(traslacion), matrix);
            render.DrawPoints(points, color);
            var cube3 = new BoxData(float3(-7.3f, -.09f, 6.47f), float3(0.3f, 2.27f, 0.33f), 10000);
            points = cube3.GetTransformedPoints(Transform.Translate(traslacion), matrix);
            render.DrawPoints(points, color);
            var cube4 = new BoxData(float3(-1.16f, 3.5f, -0.35f), float3(1.68f, .2f, 2.3f), 20000);
            points = cube4.GetTransformedPoints(Transform.Translate(traslacion), matrix);
            render.DrawPoints(points, color);
            var cube5 = new BoxData(float3(-1.16f, -.2f, -0.35f), float3(1.68f, .3f, 2.3f), 20000);
            points = cube5.GetTransformedPoints(Transform.Translate(traslacion), matrix);
            render.DrawPoints(points, color);

            #endregion

        }

        public static float3[] ApplyTransform(float3[] points, float4x4 matrix)
        {
            float3[] result = new float3[points.Length];

            // Transform points with a matrix
            // Linear transform in homogeneous coordinates
            for (int i = 0; i < points.Length; i++)
            {
                float4 h = float4(points[i], 1);
                h = mul(h, matrix);
                result[i] = h.xyz / h.w;
            }

            return result;
        }

        public static float3[] ApplyTransform(float3[] points, Func<float3, float3> freeTransform)
        {
            float3[] result = new float3[points.Length];

            // Transform points with a function
            for (int i = 0; i < points.Length; i++)
                result[i] = freeTransform(points[i]);

            return result;
        }

        public static void SomeCubes(Raster render)
        {
            render.ClearRT(float4(0, 0, 0.2f, 1));
            int N = 50000;
            var cube = new BoxData(float3(-1, 0, 0), float3(2, 1, 1), N);
            var points = cube.PointsInBox;
            var camPos = float3(2, 2.6f, 4);
            var direction = -camPos;
            var camera = new Camera(camPos, direction);
            var camViewMatrix = camera.GetViewMatrix();
            // var viewMatrix = Transform.LookAtLH(float3(2f, 2.6f, 4), float3(0, 0, 0), float3(0, 1, 0));
            // points = ApplyTransform(points, Transform.LookAtLH(float3(2f, 2.6f, 4), float3(0, 0, 0), float3(0, 1, 0)));
            points = ApplyTransform(points, camera.GetViewMatrix());
            points = ApplyTransform(points, Transform.PerspectiveFovLH(pi_over_4, render.RenderTarget.Height / (float)render.RenderTarget.Width, 0.01f, 10));
            render.DrawPoints(points);
        }

        private static void FreeTransformTest(Raster render)
        {
            render.ClearRT(float4(0, 0, 0.2f, 1)); // clear with color dark blue.

            int N = 100000;
            // Create buffer with points to render
            float3[] points = RandomPositionsInBoxSurface(N);

            // Creating boxy...
            //Esta es una transformacion de escala
            points = ApplyTransform(points, float4x4(
                2f, 0, 0, 0,
                0, 1f, 0, 0,
                0, 0, 1f, 0,
                0, 0, 0, 1
                ));

            points = ApplyTransform(points, Transform.Translate(-1, 0, 0));

            // Apply a free transform
            // points = ApplyTransform(points, p => float3(p.x * cos(p.y) + p.z * sin(p.y), p.y, p.x * sin(p.y) - p.z * cos(p.y)));

            #region viewing and projecting

            points = ApplyTransform(points, Transform.LookAtLH(float3(5f, 2.6f, 4), float3(0, 0, 0), float3(0, 1, 0)));
            points = ApplyTransform(points, Transform.PerspectiveFovLH(pi_over_4, render.RenderTarget.Height / (float)render.RenderTarget.Width, 0.01f, 10));

            #endregion

            render.DrawPoints(points);
        }

    }
}
