using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BiBsps.BiGlobalFiies
{
    [Serializable]
    public class SeatInit
    {
        [XmlAttribute] public string Sn { set; get; }
        [XmlAttribute] public int Position { set; get; }
        [XmlAttribute] public int CurrentDac0 { set; get; }
        [XmlAttribute] public int CurrentDac1 { set; get; }
        [XmlAttribute] public int CurrentDac2 { set; get; }
        [XmlAttribute] public int CurrentDac3 { set; get; }
        [XmlAttribute] public double Icc0 { set; get; }
    }

    [Serializable]
    public class BoardCurrents
    {
        private readonly List<SeatInit> _seatInit;

        public BoardCurrents()
        {
            _seatInit = new List<SeatInit>();
        }

        [XmlAttribute] public string BoardName { set; get; }
        [XmlElement] public List<SeatInit> SlotInit => _seatInit;
    }

    public class TestPlanData
    {
        public TestPlanData()
        {
            CalBiasPoint = new List<int>();
        }

        public int FloorNum { set; get; }
        public int BoardNum { set; get; }
        public string BoardName { set; get; }
        public int SeatsCount { set; get; }
        public double ReadCurrentk { set; get; }
        public double ReadCurrentb { set; get; }
        public double ReadIteck { set; get; }
        public double ReadItecb { set; get; }
        public double DeltaIcck { set; get; }
        public double DeltaIccb { set; get; }
        public double SetTemperaturek { set; get; }
        public double SetTemperatureb { set; get; }
        public double TargetTecTemperature { set; get; }
        public int SetBiasStep { set; get; }
        public List<int> CalBiasPoint { get; }
        public int ConnectCheckBias { set; get; }
        public int ConnectIccRangeMin { set; get; }
        public int ConnectIccRangeMax { set; get; }
        public int ConnectTecTempRangeMin { set; get; }
        public int ConnectTecTempRangeMax { set; get; }
        public double TargetCurrent { set; get; }
        public double DacMa { set; get; }
        public int SetBiasRangeMin { set; get; }
        public int SetBiasRangeMax { set; get; }
        public double CheckTemperature { set; get; }
        public double TargetOvenTemperature { set; get; }
        public string MesStep { set; get; }
        public bool IsMesCheck { set; get; }
        public bool IsHold { set; get; }
        public int OvenTimeout { set; get; }
        public string OvenPort { set; get; }
        public string CtrlPort { set; get; }
        public bool OvenTempCheck { set; get; }
        public string CocType { set; get; }
        public bool IsCocCheck { set; get; }
    }

    public class VbmsPlanData : TestPlanData
    {
        public int TargetBias { set; get; }
    }
}
