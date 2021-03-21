using System;
using GMath;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Renderer.Rendering
{
    /// <summary>
    /// Esta clase guarda la informacion de la imagen y contiene metodos<para/>
    /// para modificarla y guardarla en el sistema
    /// </summary>
    public class Texture2D
    {
        #region Variables

        /// <summary>
        /// El ancho de la imagen
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// La altura de la imagen
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// La informacion de la textura
        /// </summary>
        private float4[,] data;

        #endregion

        /// <summary>
        /// Inicializa una nueva textura
        /// </summary>
        /// <param name="width">El ancho de la textura</param>
        /// <param name="height">La altura de la textura</param>
        public Texture2D(int width, int height)
        {
            Width = Math.Max(width, 1);
            Height = Math.Max(height, 1);
            data = new float4[Height, Width];
        }

        #region Getters and Setters

        /// <summary>
        /// Gets or set the value in te texture in the specific position
        /// </summary>
        /// <value></value>
        public float4 this[int x, int y]
        {
            get
            {
                return data[y, x];
            }
            set
            {
                data[y, x] = value;
            }
        }

        /// <summary>
        /// Write a value in the texture at the specific position
        /// </summary>
        /// <param name="posX">X position</param>
        /// <param name="posY">Y position</param>
        /// <param name="value">Value for write</param>
        public void Write(int posX, int posY, float4 value)
        {
            this[posX,posY]=value;
        }

        /// <summary>
        /// Reads the exture value at the specific position
        /// </summary>
        /// <param name="posX">X position</param>
        /// <param name="posY">Y position</param>
        /// <returns></returns>
        public float4 Read(int posX, int posY) => this[posX,posY];

        #endregion

        /// <summary>
        /// Save the texture
        /// </summary>
        /// <param name="fileName">fileName of the image that is gonna be saved</param>
        public void Save(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            using(BinaryWriter writer =  new BinaryWriter(fileStream))
            {
                writer.Write(Width);
                writer.Write(Height);
                for (int py = 0; py < Height; py++)
                {
                    for (int px = 0; px < Width; px++)
                    {
                        var value = data[py, px];
                        writer.Write(value.x);
                        writer.Write(value.y);
                        writer.Write(value.z);
                        writer.Write(value.w);
                    }
                }
            }
        }

    }
}
