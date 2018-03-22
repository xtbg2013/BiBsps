using log4net;

namespace BiBsps.BiLog
{
    public class DriverLog
    {
        private readonly string _productType;
        private readonly string _boardName;
        private readonly ILog _log;
        private readonly ILog _logBsp;
        public DriverLog(ILog log ,string productType, string boardName)
        {
            _productType = productType;
            _boardName = boardName;
            _log = log;
            _logBsp = LogManager.GetLogger("BiBspLocolLog");
        }
        public void LogInfo(string info)
        {
            _log?.Info(_boardName + " " + _productType + " " + info);
            _logBsp?.Info(_boardName + " " + _productType + " " + info);
        }
        public void LogError(string info)
        {
            _log?.Error(_boardName + " " + _productType + " " + info);
            _logBsp?.Info(_boardName + " " + _productType + " " + info);
        }
        public void LogWarn(string info)
        {
            _log?.Warn(_boardName + " " + _productType + " " + info);
            _logBsp?.Info(_boardName + " " + _productType + " " + info);
        }
        public void LogDebug(string info)
        {
            _log?.Debug(_boardName + " " + _productType + " " + info);
            _logBsp?.Info(_boardName + " " + _productType + " " + info);
        }
    }
}
