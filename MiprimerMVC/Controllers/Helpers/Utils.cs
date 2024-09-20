using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MiprimerMVC.Controllers.Helpers
{
    public class Utils
    {
        public static byte[] ExportarAExcel<T>(List<T> dataList)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Data");

                if (dataList != null && dataList.Any())
                {
                    // Crear cabeceras
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    for (int i = 0; i < properties.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = properties[i].Name;
                    }

                    // Llenar filas con los datos de la lista
                    int row = 2;
                    foreach (var item in dataList)
                    {
                        for (int col = 0; col < properties.Length; col++)
                        {
                            var value = properties[col].GetValue(item, null);
                            worksheet.Cells[row, col + 1].Value = value != null ? value.ToString() : "N/A";
                        }
                        row++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Guardar el archivo en memoria y devolverlo como un arreglo de bytes
                    return package.GetAsByteArray();
                }

                return null;
            }
        }
    }
}