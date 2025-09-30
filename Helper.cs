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


  /// <summary>
  /// Convierte una lista genérica de objetos en una cadena XML con una estructura simple,
  /// donde cada objeto se representa como un nodo "ROW" y cada propiedad como un subelemento.
  /// </summary>
  /// <typeparam name="T">
  /// Tipo de los objetos contenidos en la lista.
  /// </typeparam>
  /// <param name="lst">
  /// Lista de objetos que se desea convertir a formato XML.
  /// </param>
  /// <returns>
  /// Una cadena XML que representa la lista de objetos, con cada elemento envuelto en un nodo "ROW"
  /// dentro de un nodo raíz "ROOT".
  /// </returns>
  public static string ConvertListToXml<T>(List<T> lst)
  {
      var xmlDocument = new XElement("ROOT");
      var type = typeof(T);

      foreach (var item in lst)
      {
          var xmlRow = new XElement("ROW");

          foreach (var prop in type.GetProperties())
          {
              var value = prop.GetValue(item, null);
              if (value != null)
              {
                  var xmlElement = new XElement(prop.Name);
                  xmlElement.SetValue(value);
                  xmlRow.Add(xmlElement);
              }
          }

          xmlDocument.Add(xmlRow);
      }

      return xmlDocument.ToString();
  }

 /// <summary>
 /// Obtiene un diccionario con las propiedades del DTO que tienen valores asignados.
 /// Solo se incluyen las propiedades públicas de instancia cuyo valor no es nulo ni vacío (según el tipo).
 /// </summary>
 /// <param name="dto">Objeto de tipo DocumentalFlujoTransaccionBusqueda_DTO del cual se extraen las propiedades con valores.</param>
 /// <typeparam name="T">El tipo de datos contenidos en el conjunto de resultados.</typeparam>
 /// <returns>Un diccionario donde la clave es el nombre de la propiedad y el valor es el contenido de dicha propiedad.</returns>
 public static Dictionary<string, object> PropiedadesConValores<T>(T dto)
 {
     var propiedadesConValores = new Dictionary<string, object>();
     var tipo = dto.GetType();
     var propiedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

     foreach (var propiedad in propiedades)
     {
         try
         {
             var valor = propiedad.GetValue(dto, null);

             if (TieneValor(valor, propiedad.PropertyType))
             {
                 propiedadesConValores[propiedad.Name] = valor;
             }
         }
         catch (Exception)
         {
             // Si hay algún error al obtener el valor de la propiedad, se omite
             continue;
         }
     }

     return propiedadesConValores;
 }

 /// <summary>
 /// Determina si un valor es considerado como "con valor" basado en su tipo y contenido.
 /// </summary>
 /// <param name="valor">El valor a evaluar.</param>
 /// <param name="tipoPropiedad">El tipo de la propiedad.</param>
 /// <returns>True si el valor es considerado como "con valor", false en caso contrario.</returns>
 public static bool TieneValor(object valor, Type tipoPropiedad)
 {
     // Si el valor es null, no tiene valor
     if (valor == null)
     {
         return false;
     }

     // Diccionario de validadores por tipo
     var validadores = new Dictionary<Type, Func<object, bool>>
     {
         { typeof(string), v => !string.IsNullOrWhiteSpace(v.ToString()) },
         { typeof(int), v => (int)v != 0 },
         { typeof(decimal), v => (decimal)v != 0m },
         { typeof(long), v => (long)v != 0L },
         { typeof(DateTime), v => (DateTime)v != default(DateTime) },
     };

     // Verificar tipos nullable primero
     if (tipoPropiedad.IsGenericType && tipoPropiedad.GetGenericTypeDefinition() == typeof(Nullable<>))
     {
         return valor != null;
     }

     // Buscar validador específico por tipo
     if (validadores.TryGetValue(tipoPropiedad, out var validador))
     {
         return validador(valor);
     }

     // Para objetos complejos (tipos de referencia), verificar que no sean null
     if (!tipoPropiedad.IsValueType)
     {
         return valor != null;
     }

     // Para otros tipos de valor, se considera que tienen valor si no son null
     return true;
 }

 /// <summary>
  /// Extracts property information (name and data type) from the paged query results.
  /// This method analyzes each IQueryResult item to provide detailed information about
  /// the properties and their corresponding data types returned.
  /// </summary>
  /// <param name="results">The list of IQueryResult items to analyze.</param>
  /// <returns>A dictionary containing property names as keys and their data types as values.</returns>
  private Dictionary<string, string> ExtractPropertiesAndDataTypes(List<IQueryResult> results)
  {
      var propertyInfo = new Dictionary<string, string>();

      if (results == null || !results.Any())
      {
          return propertyInfo;
      }

      // Analyze the first result to get all available properties
      var firstResult = results.First();

      foreach (var property in firstResult.Properties)
      {
          string propertyName = property.LocalName ?? property.QueryName ?? property.DisplayName ?? "Unknown";
          string dataType = "Unknown";

          try
          {
              // Try to infer data type from the actual value
              if (property.FirstValue != null)
              {
                  dataType = property.FirstValue.GetType().Name;
              }
              else if (property.Values != null && property.Values.Count > 0)
              {
                  // Check if it's a multi-valued property
                  var firstNonNullValue = property.Values.FirstOrDefault(v => v != null);
                  if (firstNonNullValue != null)
                  {
                      dataType = firstNonNullValue.GetType().Name;
                      if (property.Values.Count > 1)
                      {
                          dataType += "[]"; // Indicate it's an array/collection
                      }
                  }
              }
          }
          catch (Exception)
          {
              // If we can't determine the type, keep "Unknown"
              dataType = "Unknown";
          }

          // Store the property information
          if (!propertyInfo.ContainsKey(propertyName))
          {
              propertyInfo.Add(propertyName, dataType);
          }
      }

      return propertyInfo;
  }
