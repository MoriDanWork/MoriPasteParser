using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriAnonfilesChecker
{
    public static class ExistLogic
    {
        static string Path = "base.mori";
        static List<string> ListOfBase = new List<string>();
        public static List<string> Check(List<string> uids)
        {
            try
            {

                List<string> uidsCleaned = new List<string>();
                foreach (string uid in uids)
                {
                    if (!ListOfBase.Contains(uid))
                    {
                        //TextWriter.Synchronized(WriteStream).WriteLine(uid);
                        uidsCleaned.Add(uid);
                    }
                }
                return uidsCleaned;
            }
            catch (System.IO.FileNotFoundException)
            {
                //File.WriteAllLines(Path, uids);
                return uids;
            }
        }

        public static void GetBase()
        {
            ListOfBase = File.ReadAllLines(Path).ToList();
        }
        
    }
}
