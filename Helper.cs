  /// <summary>
  /// Obtiene el valor de una propiedad específica de un objeto utilizando reflexión.
  /// </summary>
  /// <param name="result">El objeto del cual se desea obtener el valor de la propiedad.</param>
  /// <param name="property">El nombre de la propiedad que se desea obtener.</param>
  /// <returns>El valor de la propiedad especificada como una cadena.</returns>
  public static object GetValueResult(object result, string property)
  {
      Type resultType = result.GetType();
      return resultType.GetProperty(property).GetValue(result);
  }


   /// <summary>
   /// Calcula el tamaño en bytes de un archivo codificado en Base64, eliminando cualquier encabezado de metadatos si está presente.
   /// </summary>
   /// <param name="base64String">Cadena codificada en Base64 que representa el archivo, opcionalmente con encabezado (por ejemplo, "data:...;base64,").</param>
   /// <returns>Tamaño del archivo en bytes como un valor de tipo long.</returns>
   public static long GetBase64FileSize(string base64String)
   {
       // Calcula el tamaño real en bytes
       int padding = base64String.EndsWith("==") ? 2 : base64String.EndsWith("=") ? 1 : 0;
       return (base64String.Length * 3 / 4) - padding;
   }


 // var datos = JsonConvert.DeserializeObject<T>(Datos.ToString());
 // var lstPropiedades = new[] { "tipo", "comentario" };
 // var valores = ObtenerValoresPropiedades(datos, lstPropiedades);
 
        /// <summary>
        /// Obtiene los valores de propiedades específicas de un objeto genérico, siempre que dichas propiedades existan y no estén vacías.
        /// </summary>
        /// <typeparam name="T">Tipo del objeto del cual se extraerán los valores de las propiedades.</typeparam>
        /// <param name="datos">Instancia del objeto desde el cual se obtendrán los valores.</param>
        /// <param name="lstPropiedades">Lista de nombres de propiedades que se desean consultar.</param>
        /// <returns>Una lista de cadenas que contiene los valores no nulos y no vacíos de las propiedades especificadas.</returns>
        public static List<string> ObtenerValoresPropiedades<T>(T datos, IEnumerable<string> lstPropiedades)
        {
            var valores = new List<string>();
            var tipo = typeof(T);

            foreach (var prop in lstPropiedades)
            {
                var propiedad = tipo.GetProperty(prop);
                var valor = propiedad.GetValue(datos) as string;

                if (!string.IsNullOrEmpty(valor))
                {
                    valores.Add(valor);
                }
            }

            return valores;
        }