using MoriPattern.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MoriPattern.Controller
{
    public static partial class SourceController
    {
        public static async Task LoadSourceAsync(string path)
        {
            try
            {
                string[] lines = await File.ReadAllLinesAsync(path);
                ObservableCollection<string> data = new(
                    lines.Where(IsValidLine)
                );

                GlobalData.Instance.Source = data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading source data: " + ex.Message);
            }
        }

        private static bool IsValidLine(string line)
        {
            return true;///
            return IsValidSource().IsMatch(line);
        }

        [GeneratedRegex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}:[a-zA-Z0-9]+$")]
        private static partial Regex IsValidSource();
    }
}
