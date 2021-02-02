using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoriAnonfilesChecker
{
    public static class ExistLogic
    {
        static string path = "base.mori";
        public static bool Check(string uid)
        {
            Chill:
            try
            {
                var ListOfBase = new List<string>();
                using StreamReader streamReader = new StreamReader(path);
                while (!streamReader.EndOfStream) ListOfBase.Add(streamReader.ReadLine());
                streamReader.Close();

                if (ListOfBase.Contains(uid))
                {
                    return false;
                }
            }
            catch(FileNotFoundException) {  }
            catch (IOException) { goto Chill; }
            Flex:
            try
            {
                using StreamWriter streamWriter = new StreamWriter(path, true);
                streamWriter.WriteLine(uid);
                streamWriter.Close();
                return true;
            }
            catch (Exception)
            {
                goto Flex;
            }
        }

    }
}
