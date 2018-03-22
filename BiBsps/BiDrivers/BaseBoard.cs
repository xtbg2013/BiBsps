using System;
using System.Collections.Generic;
using BiBsps.BiGlobalFiies;
using BiInterface;

namespace BiBsps.BiDrivers
{
    internal abstract class BaseBoard: IBoard
    {
        protected TestPlanData DataMember;

        protected BaseBoard()
        {
            DataMember = new TestPlanData();
        }

        #region IBoard Implementation

        public virtual int SeatsCount { get; set; }

        public virtual string GetVersion()
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<int, string> GetSnSet()
        {
            throw new NotImplementedException();
        }

        public virtual void AddSeat(int seat, string sn)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveSeat(int seat)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, bool> CheckConnections(bool postBurnIn = false)
        {
            return !postBurnIn ? PreCheckConnections() : PostCheckConnections();
        }

        public virtual bool EnableBoard()
        {
            throw new NotImplementedException();
        }

        public virtual bool DisableBoard()
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<int, List<KeyValuePair<string, string>>> ReadDataSet(string type)
        {
            throw new NotImplementedException();
        }

        public double GetTargetOvenTemperature()
        {
            return DataMember.TargetOvenTemperature;
        }

        public virtual bool SetUpTemperature(double target)
        {
            throw new NotImplementedException();
        }

        public virtual bool TearDownTemperature()
        {
            throw new NotImplementedException();
        }

        public virtual void CatchException(int seat)
        {
            throw new NotImplementedException();
        }

        public bool IsMesCheck()
        {
            return DataMember.IsMesCheck;
        }

        public bool IsHold()
        {
            return DataMember.IsHold;
        }

        public string GetMesStepName()
        {
            return DataMember.MesStep;
        }

        public virtual bool IsCocCheck()
        {
            return false;
        }

        public virtual string GetCocTypeBySn(string sn, string[] cocInfo)
        {
            throw new NotImplementedException();
        }

        public virtual string GetCocTypeByPlan()
        {
            throw new NotImplementedException();
        }

        #endregion


        protected virtual Dictionary<int, bool> PostCheckConnections()
        {
            throw new NotImplementedException();
        }

        protected virtual Dictionary<int, bool> PreCheckConnections()
        {
            throw new NotImplementedException();
        }
    }
}
