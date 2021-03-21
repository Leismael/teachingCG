using System;
using System.Text;

namespace Renderer.Utils
{
    /// <summary>
    /// Esta clase es para servir como ayuda rapida para buscar el directorio parent<para/>
    /// No tiene como objetivo otra cosa... Y no maneja los errores con rutas invalidas<para/>
    /// Esto es codigo hecho a lo rapido y no pretende usars en mas ningun lugar
    /// </summary>
    public class PathHelper
    {
        string path;

        public PathHelper(string path)
        {
            if (CheckIfPathIsValid(path))
                throw new Exception("Ruta invalida");
            this.path = path;
        }

        private bool CheckIfPathIsValid(string path)
        {
            return string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path);
        }

        //Devuelve el directorio padre de una ruta
        public string Parent
        {
            get
            {
                var index = path.LastIndexOf("\\");
                if (index == -1)
                    return path;
                return path.Substring(0, index);
            }
        }

        /// <summary>
        /// Devuelve el directorio que se encuentra arriba del actual en 'amount' posiciones<para/>
        /// **Recuerden que aqui no manejo errores que puedan suceder... Esto es codigo a lo rapido**
        /// </summary>
        /// <param name="amount">La cantidad de directorios que se ascendera</param>
        /// <returns></returns>
        public string GetParent(int amount)
        {
            int index = path.Length;
            for (int i = path.Length - 1, count = 0; i >= 0 && count < amount; i--)
            {
                if (path[i] == '\\')
                {
                    count++;
                    index = i;
                }
            }
            return path.Substring(0, index);
        }

        /// <summary>
        /// Cambia la ruta interna de la clase
        /// </summary>
        /// <param name="newPath"></param>
        public void ChangeCurrentPath(string newPath)
        {
            if (CheckIfPathIsValid(newPath)) return;
            path = newPath;
        }

        /// <summary>
        /// Combina la ruta con las rutas de aqui<para/>
        /// **OJO... Aqui los path no pueden comenzar ni terminar con el simbolo '\'**
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Combine(params string[] paths)
        {
            if (paths == null)
            {
                throw new Exception("El array de rutas es null");
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(path);
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == null) continue;
                builder.Append($"\\{paths[i]}");
            }
            return path = builder.ToString();
        }

        public override string ToString()
        {
            return path;
        }
    }
    
}