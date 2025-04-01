using System.Collections.Generic;
                    using System.Reflection;
                    using JetBrains.Annotations;
                    using Microsoft.VisualStudio.TestTools.UnitTesting;
                    using RaceOverlay.Data.Models;
                    using RaceOverlay.Overlays.FuelCalculator;
                    
                    namespace RaceOverlay.Tests.Overlays.FuelCalculator;
                    
                    [STATestClass]
                    [TestSubject(typeof(RaceOverlay.Overlays.FuelCalculator.FuelCalculator))]
                    public class FuelCalculatorTest
                    {
                        [TestMethod]
                        [STATestMethod]
                        public void OnLapChangeTest()
                        {
                            // Arrange
                            var fuelCalculator = new RaceOverlay.Overlays.FuelCalculator.FuelCalculator(true);
                            var type = fuelCalculator.GetType();
                            fuelCalculator.Hide();

                            // Setup mock data
                            var mockData = new iRacingData();
                            mockData.PlayerIdx = 0;
                            mockData.SessionData = new SessionData();
                            mockData.LocalCarTelemetry = new LocalCarTelemetry();
                            var mockDriver = new DriverModel
                            {
                                LastLap = 80.5f,
                                Idx = 0
                            };

                            mockData.Drivers = new[] { mockDriver };
                            mockData.PlayerIdx = 0;

                            // Initialize all required fields
                            type.GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, mockData);
                            type.GetField("_currentFuel", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, 100.0f);
                            type.GetField("_fuelOnLastLap", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, 102.0f);
                            type.GetField("_lastLapTimes", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, new List<float>());
                            type.GetField("_lastLapFuel", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, new List<float>());
                            type.GetField("_marginLaps", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, 1.0f);
                            type.GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, true);

                            // Act
                            var onLapChangedMethod = type.GetMethod("OnLapChanged", BindingFlags.NonPublic | BindingFlags.Instance);
                            onLapChangedMethod?.Invoke(fuelCalculator, null);
                    
                            // Assert
                            var lastLapTimes = (List<float>)type.GetField("_lastLapTimes", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(fuelCalculator);
                            var lastLapFuel = (List<float>)type.GetField("_lastLapFuel", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(fuelCalculator);
                    
                            Assert.IsNotNull(lastLapTimes);
                            Assert.IsNotNull(lastLapFuel);
                            Assert.AreEqual(1, lastLapTimes.Count);
                            Assert.AreEqual(1, lastLapFuel.Count);
                            Assert.AreEqual(80.5f, lastLapTimes[0]);
                            Assert.AreEqual(2.0f, lastLapFuel[0]);
                            Assert.AreEqual(100.0f, (float)type.GetField("_fuelOnLastLap", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(fuelCalculator));
                    
                            // Test list size limitation
                            for (int i = 0; i < 15; i++)
                            {
                                mockDriver.LastLap = 80.5f + i;
                                type.GetField("_currentFuel", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, 100.0f - (i * 2));
                                type.GetField("_fuelOnLastLap", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, 102.0f - (i * 2));
                                onLapChangedMethod?.Invoke(fuelCalculator, null);
                            }
                    
                            lastLapTimes = (List<float>)type.GetField("_lastLapTimes", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(fuelCalculator);
                            lastLapFuel = (List<float>)type.GetField("_lastLapFuel", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(fuelCalculator);
                    
                            Assert.AreEqual(10, lastLapTimes.Count);
                            Assert.AreEqual(10, lastLapFuel.Count);
                        }
                        
                        [TestMethod]
                        [STATestMethod]
                        public void CalculateFuelToEndTest()
                        {
                            // Arrange
                            var fuelCalculator = new RaceOverlay.Overlays.FuelCalculator.FuelCalculator(true);
                            var type = fuelCalculator.GetType();
                            fuelCalculator.Hide();

                            // Setup mock data
                            var mockData = new iRacingData();
                            var mockDriver = new DriverModel { Idx = 0, LastLap = 80.0f };
                            mockData.Drivers = new[] { mockDriver };
                            mockData.PlayerIdx = 0;
                            mockData.SessionData.TimeLeft = 3600; // 1 hour left
                            mockData.SessionData.LapsTotal = 32767; // Time-based race
                            mockData.LocalCarTelemetry.Lap = 10;
                            mockData.LocalCarTelemetry.FuelCapacity = 100;

                            // Initialize lists and set values via reflection
                            var lastLapTimes = new List<float> { 80.0f, 81.0f, 82.0f }; // Average: 81 seconds
                            var lastLapFuel = new List<float> { 2.0f, 2.1f, 1.9f }; // Average: 2 fuel per lap
                            float marginLaps = 1.0f;

                            type.GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, mockData);
                            type.GetField("_lastLapTimes", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, lastLapTimes);
                            type.GetField("_lastLapFuel", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, lastLapFuel);
                            type.GetField("_marginLaps", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, marginLaps);

                            // Act
                            var calculateFuelToEndMethod = type.GetMethod("CalculateFuelToEnd", BindingFlags.NonPublic | BindingFlags.Instance);
                            var result = (float)calculateFuelToEndMethod?.Invoke(fuelCalculator, null);

                            // Assert
                            var expectedLapsToFinish = 3600f / 81f; // ~44.44 laps
                            var expectedFuelPerLap = 2.0f; // Average fuel consumption
                            var expectedFuelNeeded = (expectedLapsToFinish * expectedFuelPerLap) + (expectedFuelPerLap * marginLaps);

                            Assert.AreEqual(expectedFuelNeeded, result, 0.1f); // Using delta for float comparison

                            // Test with lap-based race
                            mockData.SessionData.LapsTotal = 50;
                            type.GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, mockData);

                            result = (float)calculateFuelToEndMethod?.Invoke(fuelCalculator, null);

                            expectedLapsToFinish = 40f; // 50 total - 10 current
                            expectedFuelNeeded = (expectedLapsToFinish * expectedFuelPerLap) + (expectedFuelPerLap * marginLaps);

                            Assert.AreEqual(expectedFuelNeeded, result, 0.1f);
                            
                            // Test for Fuel capacity test
                            mockData.LocalCarTelemetry.FuelCapacity = 50;
                            mockData.SessionData.LapsTotal = 300;
                            type.GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(fuelCalculator, mockData);
                            
                            result = (float)calculateFuelToEndMethod?.Invoke(fuelCalculator, null);

                            expectedLapsToFinish = 40f; // 50 total - 10 current
                            expectedFuelNeeded = (expectedLapsToFinish * expectedFuelPerLap) + (expectedFuelPerLap * marginLaps);

                            Assert.AreEqual(mockData.LocalCarTelemetry.FuelCapacity, result, 0.1f);
                        }
                    }