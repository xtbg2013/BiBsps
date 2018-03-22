using System;
using BiBsps.BiData;
namespace BiBsps.BiProtocols.Parallel
{
    internal class ParallelProtocolQsfp28G:BaseParallelProtocol
    {
        private  static ParallelProtocolQsfp28G _inst;

        public static ParallelProtocolQsfp28G Inst(VbmsDriverAddr vbmsAddr, VbmsCmd cmd)
        {
            string msg;
            if (_inst != null) return _inst;
            _inst = new ParallelProtocolQsfp28G(vbmsAddr, cmd);
            if (!_inst.ConfigI2C(out msg))
                throw new Exception("Config I2C exception : " + msg);
            return _inst;  
        }

        protected ParallelProtocolQsfp28G(VbmsDriverAddr vbmsAddr, VbmsCmd cmd) :base(vbmsAddr, cmd)
        {
        }
    }
}
