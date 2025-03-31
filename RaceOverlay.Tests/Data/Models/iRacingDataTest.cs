using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceOverlay.Data.Models;

namespace RaceOverlay.Tests.Data.Models;

[TestClass]
    public class iRacingDataTest
    {
        [TestMethod]
        public void GetGapToPlayerMs_StandardSituation_ReturnsCorrectGap()
        {
            // Arrange
            var iRacingData = new iRacingData();
            
            // player
            var player = new DriverModel 
            { 
                Idx = 0, 
                BestLap = 90.0f, 
                LapDistance = 0.3f,
                EstCarClassNeededLapTime = 92.0f 
            };
            
            // opponent
            var opponent = new DriverModel 
            { 
                Idx = 1,
                LapDistance = 0.4f,
                EstCarClassNeededLapTime = 94.0f 
            };
            
            iRacingData.Drivers = new[] { player, opponent };
            iRacingData.PlayerIdx = 0;
            
            // Act
            int gap = iRacingData.GetGapToPlayerMs(1);
            
            // Assert
            // Expected time difference: (94.0f - 92.0f) * 1000 = 2000ms
            Assert.AreEqual(2000, gap);
        }

        [TestMethod]
        public void GetGapToPlayerMs_CrossingStartFinishLine_ReturnsCorrectGap()
        {
            // Arrange
            var iRacingData = new iRacingData();
            
            // player short time before crossing the start/finish line
            var player = new DriverModel 
            { 
                Idx = 0, 
                BestLap = 90.0f, 
                LapDistance = 0.9f,
                EstCarClassNeededLapTime = 92.0f 
            };
            
            // opponent short time after crossing the start/finish line
            var opponent = new DriverModel 
            { 
                Idx = 1,
                LapDistance = 0.1f,
                EstCarClassNeededLapTime = 94.0f 
            };
            
            iRacingData.Drivers = new[] { player, opponent };
            iRacingData.PlayerIdx = 0;
            
            // Act
            int gap = iRacingData.GetGapToPlayerMs(1);
            
            // Assert
            // At wrap and S <= C: (C - S) - bestForPlayer
            // (94.0f - 92.0f) - 90.0f = 2.0f - 90.0f = -88.0f
            // -88.0f * 1000 = -88000ms
            Assert.AreEqual(-88000, gap);
        }

        [TestMethod]
        public void GetGapToPlayerMs_NoBestLap_UsesEstimatedTime()
        {
            // Arrange
            var iRacingData = new iRacingData();
            
            // Player without best lap time
            var player = new DriverModel 
            { 
                Idx = 0, 
                BestLap = 0.0f, 
                LapDistance = 0.3f,
                EstCarClassNeededLapTime = 92.0f 
            };
            
            var opponent = new DriverModel 
            { 
                Idx = 1,
                LapDistance = 0.4f,
                EstCarClassNeededLapTime = 94.0f 
            };
            
            iRacingData.Drivers = new[] { player, opponent };
            iRacingData.PlayerIdx = 0;
            
            // Act
            int gap = iRacingData.GetGapToPlayerMs(1);
            
            // Assert
            Assert.AreEqual(2000, gap);
        }
    }