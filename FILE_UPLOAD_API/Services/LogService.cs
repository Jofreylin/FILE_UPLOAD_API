using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Extensions.Caching.Memory;
using FILE_UPLOAD_API.Context;

namespace FILE_UPLOAD_API.Services
{
    public interface ILogRepository
    {
        void InsertLog(string MethodName, string SentParameters, string LogMessages, int? CreateUserId = null);
    }
    public class LogService : ILogRepository
    {
        private readonly DocApiContext _Context;
        private readonly IMemoryCache _memoryCache;
        private readonly IWebHostEnvironment _env;
        public LogService(DocApiContext Context, IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _Context = Context;
            _memoryCache = memoryCache;
            _env = env;
        }

        public void InsertLog(string MethodName, string SentParameters, string LogMessages, int? CreateUserId = null)
        {

            try
            {
                _memoryCache.Set("LastErrorLogMessage", LogMessages);

                var Identity = new SqlParameter("@Output", SqlDbType.Int) { Direction = ParameterDirection.Output };
                _Context.Database.ExecuteSqlInterpolated($"[XDMS].[SP_SetLogs] {MethodName},{SentParameters}, {LogMessages}, {CreateUserId}, {Identity} out");

                InsertLogInFile(MethodName, SentParameters, LogMessages, CreateUserId);
            }
            catch (System.Exception ex)
            {
                InsertLogInFile($"TRYING INSERT LOG FOR {MethodName}", SentParameters, ex.Message + " | " + ex.InnerException, CreateUserId);
            }
        }

        private void InsertLogInFile(string MethodName, string SentParameters, string LogMessages, int? CreateUserId = null)
        {
            try
            {
                string folder = $"{_env.WebRootPath}/DOC-LOGS/ERRORS";

                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string path = $"{folder}/APP_ERRORS_LOGS.txt";

                List<string> logLines = new List<string>
                {
                    "",
                    "---- Log Entry ----",
                    $"Server Time {DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}",
                    $"Dominican Republic Time {DateTime.UtcNow.AddHours(-4).ToLongTimeString()} {DateTime.UtcNow.AddHours(-4).ToLongDateString()}",
                    "-------------------",
                    $" Method : {MethodName}",
                    $" Parameters Sent : {SentParameters}",
                    $" User ID : {CreateUserId}",
                    $" Error : {LogMessages}",
                    "============================================="
                };

                File.AppendAllLines(path, logLines);
            }
            catch (Exception)
            {

            }
        }
    }
}
