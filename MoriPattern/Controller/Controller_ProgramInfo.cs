using MoriPattern.Data;
using MoriPattern.Model;
using MoriPattern.Res;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MoriPattern.Controller
{
    public static class ProgramInfoController
    {
        private static async Task<ProgramInfo> GetProgramInfoAsync(string serverUrl)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(serverUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ProgramInfo>(json);
            }

            return null;
        }

        private static async Task<string> GetServerInfoAsync()
        {
            try
            {
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync("https://raw.githubusercontent.com/MoriDanWork/MoriLogs/main/serverIp");
                if (response.IsSuccessStatusCode)
                {
                    string info = await response.Content.ReadAsStringAsync();
                    return info.Replace("\n", "");
                }
            }
            catch (Exception) { }

            return null;
        }

        public static async Task<(bool, ProgramInfo)> GetProgramInfo()
        {
            ProgramInfo programInfo = new();

            try
            {
                string programInfoUrl = $"{GlobalData.Instance.ProgramInfo.Server}/{GlobalData.Instance.ProgramInfo.MoriProgramID}/GetInfo";
                programInfo = await GetProgramInfoAsync(programInfoUrl);

                if (programInfo != null && programInfo.CurrentVersion == programInfo.LastVersion)
                {
                    return (true, programInfo);
                }
            }
            catch (Exception)
            {
                string serverInfo = await GetServerInfoAsync();
                if (!string.IsNullOrEmpty(serverInfo))
                {
                    programInfo.Server = serverInfo;

                    string programInfoUrl = $"{programInfo.Server}/{GlobalData.Instance.ProgramInfo.MoriProgramID}/GetInfo";
                    programInfo = await GetProgramInfoAsync(programInfoUrl);

                    if (programInfo != null && programInfo.CurrentVersion == programInfo.LastVersion)
                    {
                        return (true, programInfo);
                    }
                }
            }

            return (false, programInfo);
        }

        public static async Task IsFirstTime()
        {
            if (!GlobalData.Instance.IsFirstTime)
                return;

            using HttpClient httpClient = new();
            var token = JwtTokenGenerator.GenerateToken();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent("1", Encoding.UTF8, "application/json");

            string addUserCountUrl = $"{GlobalData.Instance.ProgramInfo.Server}/{GlobalData.Instance.ProgramInfo.MoriProgramID}/AddUsersCount";
            await httpClient.PostAsync(addUserCountUrl, content);
        }
    }

}
