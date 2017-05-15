// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents data about an aerobic session.
    /// </summary>
    ///
    /// <remarks>
    /// Aerobic data can represent the data for an entire session, the data
    /// for a single lap, or the desired aerobic session for an aerobic goal.
    /// </remarks>
    ///
    public class AerobicBase : ItemBase
    {
        /// <summary>
        /// Populates the data for the length from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the length.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator modeNav =
                navigator.SelectSingleNode("mode");

            _mode.Clear();
            if (modeNav != null)
            {
                _mode.ParseXml(modeNav);
            }

            XPathNavigator distanceNav =
                navigator.SelectSingleNode("distance");

            if (distanceNav != null)
            {
                _distance = new Length();
                _distance.ParseXml(distanceNav);
            }

            _minutes =
                XPathHelper.GetOptNavValueAsDouble(navigator, "minutes");

            int? currentIntensity =
                XPathHelper.GetOptNavValueAsInt(navigator, "intensity");
            if (currentIntensity != null)
            {
                _intensity = (RelativeRating)(int)currentIntensity;
            }

            _peakHr =
                XPathHelper.GetOptNavValueAsInt(navigator, "peak-heartrate");

            _averageHr =
                XPathHelper.GetOptNavValueAsInt(navigator, "avg-heartrate");

            _minHr =
                XPathHelper.GetOptNavValueAsInt(navigator, "min-heartrate");

            _energy =
                XPathHelper.GetOptNavValueAsDouble(navigator, "energy");

            _energyFromFat =
                XPathHelper.GetOptNavValueAsDouble(navigator, "energy-from-fat");

            _peakSpeed =
                XPathHelper.GetOptNavValue<SpeedMeasurement>(
                    navigator,
                    "peak-speed");

            _averageSpeed =
                XPathHelper.GetOptNavValue<SpeedMeasurement>(
                    navigator,
                    "avg-speed");

            _minSpeed =
                XPathHelper.GetOptNavValue<SpeedMeasurement>(
                    navigator,
                    "min-speed");

            _peakPace =
                XPathHelper.GetOptNavValue<PaceMeasurement>(
                    navigator,
                    "peak-pace");

            _averagePace =
                XPathHelper.GetOptNavValue<PaceMeasurement>(
                    navigator,
                    "avg-pace");

            _minPace =
                XPathHelper.GetOptNavValue<PaceMeasurement>(
                    navigator,
                    "min-pace");

            _peakPower =
                XPathHelper.GetOptNavValue<PowerMeasurement>(
                    navigator,
                    "peak-power");

            _averagePower =
                XPathHelper.GetOptNavValue<PowerMeasurement>(
                    navigator,
                    "avg-power");

            _minPower =
                XPathHelper.GetOptNavValue<PowerMeasurement>(
                    navigator,
                    "min-power");

            _peakTorque =
                XPathHelper.GetOptNavValue<TorqueMeasurement>(
                    navigator,
                    "peak-torque");

            _averageTorque =
                XPathHelper.GetOptNavValue<TorqueMeasurement>(
                    navigator,
                    "avg-torque");

            _minTorque =
                XPathHelper.GetOptNavValue<TorqueMeasurement>(
                    navigator,
                    "min-torque");

            _leftRightBalance =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "left-right-balance");

            _peakCadence =
                XPathHelper.GetOptNavValueAsDouble(navigator, "peak-cadence");

            _averageCadence =
                XPathHelper.GetOptNavValueAsDouble(navigator, "avg-cadence");

            _minCadence =
                XPathHelper.GetOptNavValueAsDouble(navigator, "min-cadence");

            _peakTemperature =
                XPathHelper.GetOptNavValue<TemperatureMeasurement>(
                    navigator,
                    "peak-temperature");

            _averageTemperature =
                XPathHelper.GetOptNavValue<TemperatureMeasurement>(
                    navigator,
                    "avg-temperature");

            _minTemperature =
                XPathHelper.GetOptNavValue<TemperatureMeasurement>(
                    navigator,
                    "min-temperature");

            _peakAltitude =
                XPathHelper.GetOptNavValue<AltitudeMeasurement>(
                    navigator,
                    "peak-altitude");

            _averageAltitude =
                XPathHelper.GetOptNavValue<AltitudeMeasurement>(
                    navigator,
                    "avg-altitude");

            _minAltitude =
                XPathHelper.GetOptNavValue<AltitudeMeasurement>(
                    navigator,
                    "min-altitude");

            _elevationGain =
                XPathHelper.GetOptNavValue<Length>(
                    navigator,
                    "elevation-gain");

            _elevationLoss =
                XPathHelper.GetOptNavValue<Length>(
                    navigator,
                    "elevation-loss");

            _numberOfSteps =
                XPathHelper.GetOptNavValueAsInt(
                    navigator,
                    "number-of-steps");

            _numberOfAerobicSteps =
                XPathHelper.GetOptNavValueAsInt(
                    navigator,
                    "number-of-aerobic-steps");

            _aerobicStepMinutes =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "aerobic-step-minutes");
        }

        /// <summary>
        /// Writes the aerobic data to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the aerobic data.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the aerobic data to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");

            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

            writer.WriteStartElement(nodeName);

            if (_mode.Text != null)
            {
                // <mode>
                _mode.WriteXml("mode", writer);
            }

            // <distance>
            _distance?.WriteXml("distance", writer);

            if (_minutes != null)
            {
                // <minutes>
                writer.WriteElementString(
                    "minutes",
                    XmlConvert.ToString(_minutes.Value));
            }

            if (_intensity != RelativeRating.None)
            {
                // <intensity>
                writer.WriteElementString(
                    "intensity",
                    ((int)_intensity).ToString(CultureInfo.InvariantCulture));
            }

            if (_peakHr != null)
            {
                // <peak-heartrate>
                writer.WriteElementString(
                    "peak-heartrate",
                    _peakHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (_averageHr != null)
            {
                // <avg-heartrate>
                writer.WriteElementString(
                    "avg-heartrate",
                    _averageHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (_minHr != null)
            {
                // <min-heartrate>
                writer.WriteElementString(
                    "min-heartrate",
                    _minHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            XmlWriterHelper.WriteOptDouble(
                writer,
                "energy",
                _energy);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "energy-from-fat",
                _energyFromFat);

            _peakSpeed?.WriteXml("peak-speed", writer);

            _averageSpeed?.WriteXml("avg-speed", writer);

            _minSpeed?.WriteXml("min-speed", writer);

            _peakPace?.WriteXml("peak-pace", writer);

            _averagePace?.WriteXml("avg-pace", writer);

            _minPace?.WriteXml("min-pace", writer);

            _peakPower?.WriteXml("peak-power", writer);

            _averagePower?.WriteXml("avg-power", writer);

            _minPower?.WriteXml("min-power", writer);

            _peakTorque?.WriteXml("peak-torque", writer);

            _averageTorque?.WriteXml("avg-torque", writer);

            _minTorque?.WriteXml("min-torque", writer);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "left-right-balance",
                _leftRightBalance);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "peak-cadence",
                _peakCadence);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "avg-cadence",
                _averageCadence);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "min-cadence",
                _minCadence);

            _peakTemperature?.WriteXml("peak-temperature", writer);

            _averageTemperature?.WriteXml("avg-temperature", writer);

            _minTemperature?.WriteXml("min-temperature", writer);

            _peakAltitude?.WriteXml("peak-altitude", writer);

            _averageAltitude?.WriteXml("avg-altitude", writer);

            _minAltitude?.WriteXml("min-altitude", writer);

            XmlWriterHelper.WriteOpt(writer, "elevation-gain", _elevationGain);
            XmlWriterHelper.WriteOpt(writer, "elevation-loss", _elevationLoss);
            XmlWriterHelper.WriteOptInt(writer, "number-of-steps", _numberOfSteps);
            XmlWriterHelper.WriteOptInt(writer, "number-of-aerobic-steps", _numberOfAerobicSteps);
            XmlWriterHelper.WriteOptDouble(writer, "aerobic-step-minutes", _aerobicStepMinutes);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the mode by which the aerobic session occurred.
        /// </summary>
        ///
        /// <remarks>
        /// The mode of the aerobic session include actions such as bike, run, or
        /// swim and are defined by the HealthVault dictionary.
        /// </remarks>
        ///
        public CodableValue Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        private CodableValue _mode = new CodableValue();

        /// <summary>
        /// Gets or sets the length traversed by this aerobic session.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Length"/> value representing the distance.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no distance.
        /// </remarks>
        ///
        public Length Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        private Length _distance;

        private static void ThrowIfValueIsNegative(double? value, string message)
        {
            if (value != null && (double)value <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), message);
            }
        }

        private static void ThrowIfValueIsNegative(int? value, string message)
        {
            if (value != null && (int)value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), message);
            }
        }

        /// <summary>
        /// Gets or sets the duration of the session in minutes.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no duration.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? Duration
        {
            get { return _minutes; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionDurationNotPositive);
                _minutes = value;
            }
        }

        private double? _minutes;

        /// <summary>
        /// Gets the duration of the aerobic session as a
        /// <see cref="System.TimeSpan"/>.
        /// </summary>
        ///
        public TimeSpan DurationAsTimeSpan
        {
            get
            {
                TimeSpan result;
                if (_minutes != null)
                {
                    result =
                        new TimeSpan(
                            0,
                            (int)_minutes.Value,
                            (int)(60.0 * (_minutes - (int)_minutes)));
                }
                else
                {
                    result = new TimeSpan();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the relative intensity of the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <see cref="RelativeRating.None"/> if there is
        /// no intensity.
        /// </remarks>
        ///
        public RelativeRating Intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        private RelativeRating _intensity;

        /// <summary>
        /// Gets or sets the peak heart rate for the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no heart rate data for the session, the value should
        /// be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero on set.
        /// </exception>
        ///
        public int? PeakHeartRate
        {
            get { return _peakHr; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionPeakHrNotPositive);
                _peakHr = value;
            }
        }

        private int? _peakHr;

        /// <summary>
        /// Gets or sets the average heart rate for the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no heart rate data for the session, the value should
        /// be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero on set.
        /// </exception>
        ///
        public int? AverageHeartRate
        {
            get { return _averageHr; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionAvgHrNotPositive);
                _averageHr = value;
            }
        }

        private int? _averageHr;

        /// <summary>
        /// Gets or sets the minimum heart rate for the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no heart rate data for the session, the value should
        /// be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to
        /// zero when set.
        /// </exception>
        ///
        public int? MinHeartRate
        {
            get { return _minHr; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionMinHrNotPositive);
                _minHr = value;
            }
        }

        private int? _minHr;

        /// <summary>
        /// Gets or sets the food energy consumed during the session.
        /// </summary>
        ///
        /// <remarks>
        /// The energy consumed is measured in kilojoules.
        /// Set the value to <b>null</b> if there is no energy reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? Energy
        {
            get { return _energy; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionEnergyNotPositive);
                _energy = value;
            }
        }

        private double? _energy;

        /// <summary>
        /// Gets or sets the energy from fat consumed during the session.
        /// </summary>
        ///
        /// <remarks>
        /// The energy consumed is measured in kilojoules.
        /// Set the value to <b>null</b> if there is no energy from fat reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? EnergyFromFat
        {
            get { return _energyFromFat; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionEnergyFromFatNotPositive);
                _energyFromFat = value;
            }
        }

        private double? _energyFromFat;

        /// <summary>
        /// Gets or sets the peak speed, in meters per second, during the
        /// session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no speed reading.
        /// </remarks>
        ///
        public SpeedMeasurement PeakSpeed
        {
            get { return _peakSpeed; }
            set { _peakSpeed = value; }
        }

        private SpeedMeasurement _peakSpeed;

        /// <summary>
        /// Gets or sets the average speed, in meters per second, during the
        /// session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no speed reading.
        /// </remarks>
        ///
        public SpeedMeasurement AverageSpeed
        {
            get { return _averageSpeed; }
            set { _averageSpeed = value; }
        }

        private SpeedMeasurement _averageSpeed;

        /// <summary>
        /// Gets or sets the minimum speed, in meters per second, during the
        /// session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no speed reading.
        /// </remarks>
        ///
        public SpeedMeasurement MinSpeed
        {
            get { return _minSpeed; }
            set { _minSpeed = value; }
        }

        private SpeedMeasurement _minSpeed;

        /// <summary>
        /// Gets or sets the peak pace, in seconds per 100 meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no pace reading.
        /// </remarks>
        ///
        public PaceMeasurement PeakPace
        {
            get { return _peakPace; }
            set { _peakPace = value; }
        }

        private PaceMeasurement _peakPace;

        /// <summary>
        /// Gets or sets the average pace, in seconds per 100 meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no pace reading.
        /// </remarks>
        ///
        public PaceMeasurement AveragePace
        {
            get { return _averagePace; }

            set
            {
                _averagePace = value;
            }
        }

        private PaceMeasurement _averagePace;

        /// <summary>
        /// Gets or sets the minimum pace, in seconds per 100 meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no pace reading.
        /// </remarks>
        ///
        public PaceMeasurement MinPace
        {
            get { return _minPace; }

            set
            {
                _minPace = value;
            }
        }

        private PaceMeasurement _minPace;

        /// <summary>
        /// Gets or sets the peak power, in watts, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no power reading.
        /// </remarks>
        ///
        public PowerMeasurement PeakPower
        {
            get { return _peakPower; }

            set
            {
                _peakPower = value;
            }
        }

        private PowerMeasurement _peakPower;

        /// <summary>
        /// Gets or sets the average power, in watts, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no power reading.
        /// </remarks>
        ///
        public PowerMeasurement AveragePower
        {
            get { return _averagePower; }

            set
            {
                _averagePower = value;
            }
        }

        private PowerMeasurement _averagePower;

        /// <summary>
        /// Gets or sets the minimum power, in watts, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no power reading.
        /// </remarks>
        ///
        public PowerMeasurement MinPower
        {
            get { return _minPower; }

            set
            {
                _minPower = value;
            }
        }

        private PowerMeasurement _minPower;

        /// <summary>
        /// Gets or sets the peak torque, in newton meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no torque reading.
        /// </remarks>
        ///
        public TorqueMeasurement PeakTorque
        {
            get { return _peakTorque; }

            set
            {
                _peakTorque = value;
            }
        }

        private TorqueMeasurement _peakTorque;

        /// <summary>
        /// Gets or sets the average torque, in newton meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no torque reading.
        /// </remarks>
        ///
        public TorqueMeasurement AverageTorque
        {
            get { return _averageTorque; }

            set
            {
                _averageTorque = value;
            }
        }

        private TorqueMeasurement _averageTorque;

        /// <summary>
        /// Gets or sets the minimum torque, in newton meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no torque reading.
        /// </remarks>
        ///
        public TorqueMeasurement MinTorque
        {
            get { return _minTorque; }

            set
            {
                _minTorque = value;
            }
        }

        private TorqueMeasurement _minTorque;

        /// <summary>
        /// Gets or sets the balance between left and right strokes.
        /// </summary>
        ///
        /// <remarks>
        /// The value is a percentage where the indicated value is the
        /// percentage of the left stroke. The remaining percentage is
        /// the right stroke.
        /// Set the value to <b>null</b> if there is no balance reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or greater than one when set.
        /// </exception>
        ///
        public double? LeftRightBalance
        {
            get { return _leftRightBalance; }

            set
            {
                if (value != null && ((double)value < 0.0 || (double)value > 1.0))
                {
                    throw new ArgumentOutOfRangeException(nameof(LeftRightBalance), Resources.AerobicSessionLeftRightBalanceNotPercentage);
                }

                _leftRightBalance = value;
            }
        }

        private double? _leftRightBalance;

        /// <summary>
        /// Gets or sets the peak cadence, in revolutions per minute (rpm),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no cadence reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? PeakCadence
        {
            get { return _peakCadence; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionPeakCadenceNotPositive);
                _peakCadence = value;
            }
        }

        private double? _peakCadence;

        /// <summary>
        /// Gets or sets the average cadence, in revolutions per minute (rpm),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no cadence reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? AverageCadence
        {
            get { return _averageCadence; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionAvgCadenceNotPositive);
                _averageCadence = value;
            }
        }

        private double? _averageCadence;

        /// <summary>
        /// Gets or sets the minimum cadence, in revolutions per minute (rpm),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no cadence reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? MinCadence
        {
            get { return _minCadence; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionMinCadenceNotPositive);
                _minCadence = value;
            }
        }

        private double? _minCadence;

        /// <summary>
        /// Gets or sets the peak temperature, in degrees Celsius (C), during
        /// the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no temperature reading.
        /// </remarks>
        ///
        public TemperatureMeasurement PeakTemperature
        {
            get { return _peakTemperature; }

            set
            {
                _peakTemperature = value;
            }
        }

        private TemperatureMeasurement _peakTemperature;

        /// <summary>
        /// Gets or sets the average temperature, in degrees Celsius (C),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no temperature reading.
        /// </remarks>
        ///
        public TemperatureMeasurement AverageTemperature
        {
            get { return _averageTemperature; }

            set
            {
                _averageTemperature = value;
            }
        }

        private TemperatureMeasurement _averageTemperature;

        /// <summary>
        /// Gets or sets the minimum temperature, in degrees Celsius (C),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no temperature reading.
        /// </remarks>
        ///
        public TemperatureMeasurement MinTemperature
        {
            get { return _minTemperature; }
            set { _minTemperature = value; }
        }

        private TemperatureMeasurement _minTemperature;

        /// <summary>
        /// Gets or sets the peak altitude, in meters above sea level, during
        /// the session.
        /// </summary>
        ///
        /// <remarks>
        /// Negative values indicate the meters below sea level.
        /// Set the value to <b>null</b> if there is no altitude reading.
        /// </remarks>
        ///
        public AltitudeMeasurement PeakAltitude
        {
            get { return _peakAltitude; }
            set { _peakAltitude = value; }
        }

        private AltitudeMeasurement _peakAltitude;

        /// <summary>
        /// Gets or sets the average altitude, in meters above sea level,
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Negative values indicate the meters below sea level.
        /// Set the value to <b>null</b> if there is no altitude reading.
        /// </remarks>
        ///
        public AltitudeMeasurement AverageAltitude
        {
            get { return _averageAltitude; }
            set { _averageAltitude = value; }
        }

        private AltitudeMeasurement _averageAltitude;

        /// <summary>
        /// Gets or sets the minimum altitude, in meters above sea level,
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Negative values indicate the meters below sea level.
        /// Set the value to <b>null</b> if there is no altitude reading.
        /// </remarks>
        ///
        public AltitudeMeasurement MinAltitude
        {
            get { return _minAltitude; }
            set { _minAltitude = value; }
        }

        private AltitudeMeasurement _minAltitude;

        /// <summary>
        /// Gets or sets the elevation gained, in meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no elevation gain reading.
        /// </remarks>
        ///
        public Length ElevationGain
        {
            get { return _elevationGain; }
            set { _elevationGain = value; }
        }

        private Length _elevationGain;

        /// <summary>
        /// Gets or sets the elevation loss, in meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no elevation loss reading.
        /// </remarks>
        ///
        public Length ElevationLoss
        {
            get { return _elevationLoss; }
            set { _elevationLoss = value; }
        }

        private Length _elevationLoss;

        /// <summary>
        /// Gets or sets the number of steps taken during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no step readings.
        /// </remarks>
        ///
        public int? NumberOfSteps
        {
            get { return _numberOfSteps; }
            set { _numberOfSteps = value; }
        }

        private int? _numberOfSteps;

        /// <summary>
        /// Gets or sets the number of aerobic steps taken during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no aerobic step readings.
        /// </remarks>
        ///
        public int? NumberOfAerobicSteps
        {
            get { return _numberOfAerobicSteps; }
            set { _numberOfAerobicSteps = value; }
        }

        private int? _numberOfAerobicSteps;

        /// <summary>
        /// Gets or sets the number of aerobic minutes during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there are no readings indicating the number of
        /// aerobic minutes.
        /// </remarks>
        ///
        public double? AerobicStepMinutes
        {
            get { return _aerobicStepMinutes; }
            set { _aerobicStepMinutes = value; }
        }

        private double? _aerobicStepMinutes;

        /// <summary>
        /// Gets a summary of the aerobic data.
        /// </summary>
        ///
        /// <returns>
        /// A string summary of the aerobic data.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (Distance != null && Duration != null)
            {
                result = Resources.AerobicDataToStringFormatDistanceAndDuration.FormatResource(
                    Distance.ToString(),
                    Duration.Value.ToString(CultureInfo.CurrentCulture));
            }
            else if (Duration != null)
            {
                result = Resources.AerobicDataToStringFormatDuration.FormatResource(Duration.Value.ToString(CultureInfo.InvariantCulture));
            }
            else if (Distance != null)
            {
                result = Resources.AerobicDataToStringFormatDistance.FormatResource(Distance.ToString());
            }

            return result;
        }
    }
}
