using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriPattern.Model
{
    public class ProgramInfo
    {
        public string CurrentVersion { get; set; }
        public string LastVersion { get; set; }
        public DateTime? LastVersionDate { get; set; }
        public string Server { get { return "https://localhost:7098"; } set { } }
        public string BackupServerId { get { return "https://raw.githubusercontent.com/MoriDanWork/MoriLogs/main/serverIp"; } }
        public string NewVersionUrl { get; set; }
        public string[] ChangeLog
        {
            get
            {
                if (ChangeLog == null) { return Array.Empty<string>(); }
                return ChangeLog;
            }
            set { }
        }
        public int MoriProgramID { get; set; }
    }
}
