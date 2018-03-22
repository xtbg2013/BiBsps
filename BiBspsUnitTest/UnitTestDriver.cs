using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BIModel;
using BiBsps.BiDrivers;
using BiBsps;
using BiBsps.Log;
using System.Collections.Generic;
using BILib;
using Moq;
namespace BiUnitTest
{
    [TestClass]
    public class UnitTestDriver
    {
        private ILog _logger = null;
        private BaseDriverCase _baseDriver= null;
        public UnitTestDriver()
        {
            _logger = new LogHelper();
            _baseDriver = new Test_CFP8TOSA(_logger);
           // _baseDriver = new Test_TOSA25G(_logger);
        }
        [TestMethod]
        public void Test_CreateDriver()
        {
            this._logger.Info("Begin test CreateDriver");
            this._baseDriver.CreateDriver();
            this._logger.Info("End test CreateDriver\n\n");
        }
        [TestMethod]
        public void Test_AddSeat()
        {
            this._logger.Info("________________Begin test  AddSeat");
            this._baseDriver.AddSeat();
           
        }
        [TestMethod]
        public void Test_SeatsCount()
        {
            this._logger.Info("________________Begin test SeatCount");
            this._baseDriver.SeatCount();
            
        }

        [TestMethod]
        public void Test_GetSnSet()
        {
            this._logger.Info("________________Begin test TestGetSnSet");
            this._baseDriver.GetSnSet();
           
        }
        [TestMethod]
        public void Test_RemoveSeat()
        {
            this._logger.Info("________________Begin test RemoveSeat");
            this._baseDriver.RemoveSeat();
        }
        [TestMethod]
        public void Test_CheckConnections()
        {
            this._logger.Info("________________Begin test CheckConnections");
            this._baseDriver.CheckConnections();
        }
        [TestMethod]
        public void Test_EnableBoard()
        {
            this._logger.Info("________________Begin test EnableBoard");
            this._baseDriver.EnableBoard();
        }
        [TestMethod]
        public void Test_DisableBoard()
        {
            this._logger.Info("________________Begin test DisableBoard");
            this._baseDriver.DisableBoard();
        }
        [TestMethod]
        public void Test_ReadDataSet()
        {
            this._logger.Info("________________Begin test ReadDataSet");
            this._baseDriver.ReadDataSet();
        }
        [TestMethod]
        public void Test_SetUpTemperature()
        {
            this._logger.Info("________________Begin test SetUpTemperature");
            this._baseDriver.SetUpTemperature();
        }
        [TestMethod]
        public void Test_TearDownTemperature()
        {
            this._logger.Info("________________Begin test TearDownTemperature");
            this._baseDriver.TearDownTemperature();
        }
        [TestMethod]
        public void Test_CatchException()
        {
            this._logger.Info("________________Begin test CatchException");
            this._baseDriver.CatchException();
        }
        [TestMethod]
        public void Test_GetMesStepName()
        {
            this._logger.Info("________________Begin test GetMesStepName");
            this._baseDriver.GetMesStepName();
        }
        [TestMethod]
        public void Test_IsCocCheck()
        {
            this._logger.Info("________________Begin test IsCocCheck");
            this._baseDriver.IsCocCheck();
        }
        [TestMethod]
        public void Test_GetTargetOvenTemperature()
        {
            this._logger.Info("________________Begin test GetTargetOvenTemperature");
            this._baseDriver.GetTargetOvenTemperature();
        }
        [TestMethod]
        public void Test_GetCocTypeBySn()
        {
            this._logger.Info("________________Begin test GetCocTypeBySn");
            this._baseDriver.GetCocTypeBySn();
        }
        [TestMethod]
        public void Test_GetCocTypeByPlan()
        {
            this._logger.Info("________________Begin test GetCocTypeByPlan");
            this._baseDriver.GetCocTypeByPlan();
        }


    }
}
