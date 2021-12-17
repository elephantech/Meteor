using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Ttp.Meteor.MotionIntegration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MotionControllerCLS {
    /// <summary>
    /// <para>Example object implementing the IMotion interface</para>
    /// <para>See <see cref="IMotion"/> markup or MotionIntegrationHelp.chm for descriptions of what each method should do</para>
    /// <para>This example object simply outputs text to the console</para>
    /// <para>To support the 'job sequence queue', the object should also implement the <see cref="IJobMotionQueue"/> interface and set the 
    ///       <see cref="MMI_Consts.MMI_Feature_JobSequenceQueue"/> feature bit</para>
    /// </summary>
    public class MMI_Example : IMotion, ILoggable {
        private LogDelegate _logDelegate;
        private void Log(string msg, params object[] args) {
            _logDelegate?.Invoke(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff ") + "--- MMI_Example ---");
            _logDelegate?.Invoke(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff ") + string.Format(msg, args));
            _logDelegate?.Invoke(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff ") + "-------------------");
        }

        public void MMI_SetLoggingDelegate(LogDelegate logDelegate) {
            _logDelegate = logDelegate;
        }


        public eMotionResult MMI_GetInterfaceVersion(out uint aVersion, out uint aFeaturesSupported) {
            // -- Return the version number of this dll, and the version number -- //
            // -- of the IMotion interface                                      -- //
            UInt16 thisPluginVersion = 9999;
            aVersion = (UInt32)(MMI_Consts.KVer_MMI_Interface << 16) | thisPluginVersion;
            // -- Set the features supported by this plugin --
            // -- Most third party plugins just provide motor control --
            aFeaturesSupported = MMI_Consts.MMI_Feature_Motors;
            Log("MMI_GetInterfaceVersion 0x{0} 0x{1}", aVersion.ToString("x"), aFeaturesSupported.ToString("x"));
            return eMotionResult.MRES_OK;
        }

        public eMotionResult MMI_Initialise(string cfgFilePath) {
            // -- Initialise the Motion Controller connection -- //
            // -- cfgFilePath is for future use and will be   -- //
            // -- an empty string in the current version      -- //
            Log("MMI_Initialise {0}", cfgFilePath);
            return eMotionResult.MRES_UNIMPL;
        }

        public eMotionResult MMI_ShutDown() {
            // -- Shut down the Motion Controller connection -- //
            Log("MMI_ShutDown");
            return eMotionResult.MRES_UNIMPL;
        }

        public eMotionResult MMI_MoveHome(eMotionAxis aAxis) {
            // -- Carry out a homing sequence on the axis -- //
            Log("MMI_MoveHome {0}", aAxis);
            return eMotionResult.MRES_UNIMPL;
        }

        public eMotionResult MMI_MoveTo(eMotionAxis aAxis, eMoveOption aMvOpt, double aDist_mm) {
            // -- Make an absolute or relative move on the axis  -- //
            // -- If MMI_Feature_ContinuousMovement is enabled -- //
            // -- and aDist_mm equals to Double.MaxValue or      -- //
            // -- Double.MinValue the move should be continuous  -- //
            Log("MMI_MoveTo {0} {1} {2}", aAxis, aMvOpt, aDist_mm);
            return eMotionResult.MRES_UNIMPL;
        }

        public eMotionResult MMI_MoveCancel(eMotionAxis aAxis, eCancelOption aOption) {
            // -- Cancel any in-progress move on the axis -- //
            // -- Also called with aAxis == eMotionAxis.AxisNone to cancel generic non-axis movement (if any) -- //
            Log("MMI_MoveCancel {0} {1}", aAxis, aOption);
            return eMotionResult.MRES_UNIMPL;
        }

        private double[] _dummyPos = new double[4];

        public eMotionResult MMI_MoveStatus(eMotionAxis aAxis, ref TMoveStat aMvStat) {
            // -- Structure size must be checked to protected against version mismatch -- //
            if (aMvStat.structSize != Marshal.SizeOf(aMvStat)) {
                return eMotionResult.MRES_STRUCTSIZE_MISMATCH;
            }

            // -- Make up some dummy status so it looks like the axis is moving -- //
            _dummyPos[(int)aAxis] += 10.4;
            aMvStat.axisState = eAxisSate.AxS_Busy;
            aMvStat.dist_mm = _dummyPos[(int)aAxis];

            // -- Pretend that a different sensor is on for each axis -- //
            if (aAxis == eMotionAxis.Axis1) {
                aMvStat.activeSensorsBmp = 1 << (int)eMotorSensor.MtrSens_Home;
                aMvStat.axisHomed = 1;
            } else if (aAxis == eMotionAxis.Axis2) {
                aMvStat.activeSensorsBmp = 1 << (int)eMotorSensor.MtrSens_MaxPosLimit;
                aMvStat.axisHomed = 0;
            } else {
                aMvStat.activeSensorsBmp = 1 << (int)eMotorSensor.MtrSens_MinPosLimit;
                aMvStat.axisHomed = 0;
            }
            // -- Pretend that all 3 sensors are assigned for every axis -- //
            aMvStat.assignedSensorsBmp = (1 << (int)eMotorSensor.MtrSens_Home) |
                                         (1 << (int)eMotorSensor.MtrSens_MaxPosLimit) |
                                         (1 << (int)eMotorSensor.MtrSens_MinPosLimit);

            Log("MMI_MoveStatus {0} {1} {2} {3} {4} {5}", aAxis, aMvStat.axisState, aMvStat.dist_mm, aMvStat.axisHomed, aMvStat.activeSensorsBmp, aMvStat.assignedSensorsBmp);
            
            return eMotionResult.MRES_OK;
        }

        public eMotionResult MMI_MoveSetSpeed(eMotionAxis aAxis, eMSpeedOption aOpt, double aSpeed_mm_s) {
            // -- Set the speed of the axis -- //
            Log("MMI_MoveSetSpeed {0} {1} {2}", aAxis, aOpt, aSpeed_mm_s);
            return eMotionResult.MRES_UNIMPL;
        }

        public eMotionResult MMI_ControlSetState(eDevice aDev, uint aIndex, int aState) {
            // -- This method gives the ability for device specific commands to be defined -- //
            // -- It does not need to be implemented                                       -- //
            throw new NotImplementedException();
        }

        public eMotionResult MMI_ControlStatus(eDevice aDev, uint aIndex, out int apState) {
            // -- This method gives the ability for device specific status to be returned  -- //
            // -- It does not need to be implemented                                       -- //
            throw new NotImplementedException();
        }

        public eMotionResult MMI_GenericControl(int aCommand, int aOption, int aParamSize, uint[] aParam) {
            // -- This method gives the ability for plugin specific commands to be defined -- //
            // -- It does not need to be implemented                                       -- //
            throw new NotImplementedException();
        }

        public eMotionResult MMI_GenericControlStr(int aCommand, int aOption, int aParamSize, string aParam) {
            // -- This method gives the ability for plugin specific commands to be defined  -- //
            // -- It does not need to be implemented                                        -- //

            // -- This is an example for executing cleaning procedure. Before this method   -- //
            // -- is called carriage has already arrived to the rest position and it will   -- //
            // -- not move until the procedure will complete. The cleaning parameters       -- //
            // -- entered in the maintenance tab are passed using JSON string. JSON         -- //
            // -- deserializer is provided in Newtonsoft.Json                               -- //
            // -- Similar idea can be applied to ECtl_SetUvLampSettings,                    -- //
            // -- ECtl_RunStartSequence, ECtl_RunEndSequence, ECtl_RunGroupEndSequence and  -- //
            // -- ECtl_RunQueueEndSequence                                                  -- //
            if (aCommand == (int)TGenCtlCmd.ECtl_RunCleaningProcedure) {
                if (aParamSize != 0) {
                    JObject parametres = JObject.Parse(aParam);
                    ExampleCustomSettingClass customSettingClass = parametres.ToObject<ExampleCustomSettingClass>();
                    // -- If class ExampleCustomSettingClass member names are going to      -- //
                    // -- match up with parameters passed in the JSON string, their values  -- //
                    // -- are going to be applied, otherwise, default values are going to   -- //
                    // -- be used                                                           -- //
                }
            }

            if (aCommand == (int)TGenCtlCmd.ECtl_RunStartSequence || aCommand == (int)TGenCtlCmd.ECtl_RunEndSequence
                                                                  || aCommand == (int)TGenCtlCmd.ECtl_RunGroupEndSequence
                                                                  || aCommand == (int)TGenCtlCmd.ECtl_RunQueueEndSequence
                                                                  || aCommand == (int)TGenCtlCmd.ECtl_SetUvLampSettings) {
                if (aParamSize != 0) {
                    JObject parametres = JObject.Parse(aParam);
                    ExampleCustomSettingClass customSettingClass = parametres.ToObject<ExampleCustomSettingClass>();
                    // -- Start, end and group end command is parsed from JSON string       -- //
                    // -- parameter the same way as with ExampleCustomSettingClass.         -- //
                }
            }

            if (aCommand == (int)TGenCtlCmd.ECtl_SetUvLampSwathCommand) {
                if (aParamSize != 0) {
                    JObject parametres = JObject.Parse(aParam);
                    UvLampSwathCommand command = parametres.ToObject<UvLampSwathCommand>();
                    // -- UV lamp swath command is parsed from JSON string parameter the    -- //
                    // -- same way as with ExampleCustomSettingClass. Please do not change  -- //
                    // -- UvLampSwathCommand class as it exactly the same as the one that   -- //
                    // -- was used to generate the JSON string.                             -- //
                    // -- This parameter will be sent before every swath if enabled so in   -- //
                    // -- the settings. Those parameters can be used to trigger UV lamps    -- //
                    // -- depending on carriage position.                                   -- //
                }
            }

            if (aCommand == (int)TGenCtlCmd.ECtl_SendPrintProgress) {
                if (aParamSize != 0) {
                    JObject parametres = JObject.Parse(aParam);
                    ProgressInfo info = parametres.ToObject<ProgressInfo>();
                    // -- Progress info is parsed from JSON string parameter the same way   -- //
                    // -- as with ExampleCustomSettingClass. Please do not change           -- //
                    // -- ProgressInfo class as it exactly the same as the one that was     -- //
                    // -- used to generate the JSON string.                                 -- //
                    // -- This parameter will be sent before every swath if enabled so in   -- //
                    // -- the settings. Those parameters can be used to trigger UV lamps    -- //
                    // -- depending on carriage position.                                   -- //
                }
            }

            if (aCommand == (int)TGenCtlCmd.ECtl_UpdateHeadTemperatureInfo) {
                if (aParamSize != 0) {
                    List<TemperatureInfo> temperatureInfos = JsonConvert.DeserializeObject<List<TemperatureInfo>>(aParam);
                    // -- Update temperature info command is parsed from JSON string        -- //
                    // -- parameter the same way as with ExampleCustomSettingClass. Please  -- //
                    // -- do not change UvLampSwathCommand class as it exactly the same as  -- //
                    // -- the one that was used to generate the JSON string.                -- //
                }
            }

            if (aCommand == (int)TGenCtlCmd.ECtl_SendJobGroupSize) {
                if (aParamSize != 0) {
                    JObject parametres = JObject.Parse(aParam);
                    JobGroupSizeCommand size = parametres.ToObject<JobGroupSizeCommand>();
                    // -- Group size is parsed from JSON string parameter the same way as   -- //
                    // -- with ExampleCustomSettingClass. Please do not change              -- //
                    // -- JobGroupSizeCommand class as it exactly the same as the one that  -- //
                    // -- was used to generate the JSON string.                             -- //
                    // -- This command will be send at the start of each group.             -- //
                }
            }

            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// This class is fixed and is reassembling the class used to generate JSON string
    /// </summary>
    public class UvLampSwathCommand {
        public double PrintStart;
        public double PrintEnd;
        public bool IsReverse;
    }

    /// <summary>
    /// This class is fixed and is exactly reassembling the class used to generate JSON string
    /// </summary>
    public class ProgressInfo {
        public int SwathPrinted;
        public int SwathTotal;
    }

    /// <summary>
    /// This class is fixed and is reassembling the class used to generate JSON string
    /// </summary>
    public class TemperatureInfo {
        public double TargetTemperature;
        public double ActualTemperature;
    }

    /// <summary>
    /// This class holds cleaning parameters
    /// Same idea can be applied to UvLampSettings StartSequence and EndSequence
    /// !!! Example code !!!
    /// !!! Purely depends on what has been set in the settings in corresponding settings tile!!!
    /// </summary>
    public class ExampleCustomSettingClass {
        public int NumberCleaningCycles;
        public int CycleTimeSec;
        public double Pressure;
    }

    /// <summary>
    /// This class is fixed and is exactly reassembling the class used to generate JSON string
    /// </summary>
    public class JobGroupSizeCommand {
        public int JobGroupSize { get; set; }
    }
}
