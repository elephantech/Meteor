/**
    (c) Meteor Inkjet. All rights reserved.

    @file MotionResult.h
    
    Definitions for unmanaged dlls which use the Meteor Motion Integration framework

    *** THIS IS AN AUTO-GENERATED FILE.  DO NOT MANUALLY EDIT ***
*/

#ifndef MOTIONRESULT_H
#define MOTIONRESULT_H

namespace MeteorMotionIntegration {

#define KVer_MMI_Interface 0x00000001   // Current version of the MMI interface.  An MMI plugin must report this in the upper 16 bits of the version number set by MMI_GetInterfaceVersion.
#define MMI_Feature_Motors 0x00000001   // Features bit set if the plugin supports motors
#define MMI_Feature_InkPumps 0x00000002   // Features bit set if the plugin supports ink supply pumps
#define MMI_Feature_PurgePump 0x00000004   // Features bit set if the plugin supports purge pump control
#define MMI_Feature_VacuumPump 0x00000008   // Features bit set if the plugin supports a meniscus pump
#define MMI_Feature_All_Pumps 0x0000000E   // Combination of ink, purge and vacuum pumps
#define MMI_Feature_Alarm 0x00000010   // Features bit set if the plugin supports an alarm device
#define MMI_Feature_MediaSense 0x00000020   // Features bit set if the plugin supports media position sensing using the DEV_MediaSense pseudo-device
#define MMI_Feature_UVLamp 0x00000040   // Features bit set if the plugin supports UV lamp control
#define MMI_Feature_PressureSensor 0x00000080   // Features bit set if the plugin supports pressure sensor
#define MMI_Feature_ContininuousMovement 0x00000100   // Features bit set if the plugin supports continuous movement on axis
#define MMI_Feature_PresenceDetector 0x00000200   // Features bit set if the plugin supports presence detector device such as light curtain
#define MMI_Feature_JobSequenceQueue 0x00000400   // Feature bit set if the plugin supports the job sequence queue methods (see IJobMotionQueue)
#define MMI_Feature_Keyboard 0x00000800   // Feature bit set if the plugin supports external buttons


// Definition of function return codes used by the Meteor Motion Integration Framework
// MRES_xxx codes should be used by the Motion Integration Plugins
// MIPC_xxx codes are reserved for the MMI Framework comms
typedef enum {
   MRES_OK = 0x00000000,   // Success
   MRES_BUSY = 0x00000001,   // Busy
   MRES_FAULT = 0x00000002,   // General fault
   MRES_INTERRUPT = 0x00000003,   // Interrupted
   MRES_NOTINIT = 0x00000004,   // Motion not initialised
   MRES_UNIMPL = 0x00000005,   // Unimplemented function
   MRES_BADPARAM = 0x00000006,   // Parameter is invalid - e.g. an out of range speed.  The value which is set will be snapped into the valid range.
   MRES_STRUCTSIZE_MISMATCH = 0x00000007,   // Mismatch in the structure size
   MRES_NOFIRMWARE = 0x00000008,   // Can't open the motion controller firmware file
   MRES_JOB_SEQ_ACTIVE = 0x00000009,   // The method can't be called while the MMI is in job sequence mode (e.g. the 'MoveTo' API, or to trap duplicate calls to JobSequenceStart)
   MRES_JOB_SEQ_INACTIVE = 0x0000000A,   // The method can't be used *unless* the MMI is in job sequence mode (i.e. after JobSequenceStart has been called)
   MRES_JOB_SEQ_QUEUE_FULL = 0x0000000B,   // MMI_JobSequenceQueueMove cannot queue the move on the PLC because the PLC's move queue is full.  MetScan will retry sending the command every 250ms until it succeeds.
   MRES_JOB_SEQ_QUEUE_NOT_PAUSED = 0x0000000C,   // Cannot pause the job sequence queue because it is already paused.
   MRES_JOB_SEQ_QUEUE_ALREADY_PAUSED = 0x0000000D,   // Cannot resume the job sequence queue because it is not paused
   MIPC_MethodNotFound = 0x00000100,   // Missing method: used by the IPC server/client only
   MIPC_ParamCountMismatch = 0x00000101,   // Parameter count mismatch: used by the IPC server/client only
   MIPC_ParamTypeInvalid = 0x00000102,   // Invalid parameter type: used by the IPC server/client only
   MIPC_MethodCallFailed = 0x00000103,   // Failed to invoke method: used by the IPC server/client only
   MIPC_IPCSendFailed = 0x00000104,   // Sending data over the IPC link failed: used by the IPC server/client only
   MIPC_IPCReceiveFailed = 0x00000105,   // Receiving data over the IPC link failed: used by the IPC server/client only
   MIPC_IPCLinkDown = 0x00000106,   // The Swath IPC named pipes aren't connected
   MIPC_InInitEvent = 0x00000107,   // An attempt has been made to send a command from within the client InitialisationComplete event
} eMotionResult;

// axis definition
typedef enum {
   AxisNone = 0x00000000,   // No axis specified.  Used in MoveCancel to cancel generic non-axis movement (if any)
   Axis1 = 0x00000001,   // Axis 1.  This is normally the printer's X axis (scanning carriage movement)
   Axis2 = 0x00000002,   // Axis 2.  This is normally the printer's Y axis (substrate movement)
   Axis3 = 0x00000003,   // Axis 3
   Axis4 = 0x00000004,   // Axis 4
} eMotionAxis;

// axis states
// (typo in enumeration name retained for backwards compatibility)
typedef enum {
   AxS_Invalid = 0x00000000,   // invalid state, motion system is not initialised
   AxS_Idle = 0x00000001,   // idle, can accept MoveXXX commands
   AxS_Busy = 0x00000002,   // busy, motor is moving
   AxS_Estop = 0x00000003,   // estop is activate
} eAxisSate;

// Motor movement cancelling options
typedef enum {
   CANCEL_NORMAL = 0x00000000,   // normal cancel, uses graceful motors stopping
   CANCEL_URGENT = 0x00000001,   // urgent cancel, crash stop, do it as quick as possible
} eCancelOption;

// Motor movement options
typedef enum {
   MV_DistAbs = 0x00000000,   // moving distance is considered as absolute
   MV_DistRel = 0x00000001,   // moving distance is considered as relative to the current position
   MV_DistAbsMediaSense = 0x00000002,   // special move type for sensing media position, e.g. using a sensor mounted on a print carriage
                                        // the carriage should stop the search at or before the absolute position sent as part of the 
                                        // MoveTo command
   MV_DistAbsForwardPrintScan = 0x00000003,   // Moving absolute distance for forward printing scan
   MV_DistAbsReversePrintScan = 0x00000004,   // Moving absolute distance for reverse printing scan
} eMoveOption;

// MoveSetSpeed() API options
typedef enum {
   MVS_ResetToDefault = 0x00000000,   // reset all speed settings to the default
   MVS_SetMoveSpeed = 0x00000001,   // set motor move speed
} eMSpeedOption;

// MPC devices, see ToggleControl()
typedef enum {
   DEV_Board = 0x00000000,   // The board itself
   DEV_Motor = 0x00000001,   // motor device
   DEV_InkPump = 0x00000002,   // ink pump e.g. for refilling header tanks
   DEV_VacPump = 0x00000003,   // vacuum pump, for maintaining meniscus
   DEV_PurgePump = 0x00000004,   // purge pump
   DEV_Valve = 0x00000005,   // valve, e.g. to switch in the purge pump
   DEV_AuxPump = 0x00000006,   // Auxiliary pump
   DEV_Sensors = 0x00000007,   // A set of generic sensors that can be mapped as Home/Neg.Limit/Pos. motor limit switches or used for other purposes
   DEV_LED = 0x00000008,   // LED (not supported currently)
   DEV_GpInput = 0x00000009,   // General purpose input (not supported currently)
   DEV_GpOutput = 0x0000000A,   // General purpose output (not supported currently)
   DEV_Keypad = 0x0000000B,   // Keypad pseudo-device. Uses generic MPC sensors as keys, see TKeypadState
   DEV_Alarm = 0x0000000C,   // "Alarm" pseudo-device. represents 1 status bit. value '1' indicates that MPC board alarm triggered and it has shut down all peripherals
   DEV_MediaSense = 0x0000000D,   // Media sensor pseudo-device.  There are two device indexes; 1 for the start of the media and 2 for the end of the media.
                                  // - MMI_ControlStatus(DEV_MediaSense, 1, out apState) should return the distance in mm from the home position to the start of the media in apState
                                  // - MMI_ControlStatus(DEV_MediaSense, 2, out apState) should return the distance in mm from the home position to the end of the media in apState
   DEV_InkTankSensors = 0x0000000E,   // Ink tanks sensors state
   DEV_PressureSensor = 0x0000000F,   // Pressure sensor
   DEV_PresenceDetector = 0x00000010,   // Presence detector device such as a light curtain.
                                        // The 'MMI_Feature_PresenceDetector' features supported bit should be set in 'MMI_GetInterfaceVersion' if a presence detector is available.
                                        // When available, MetScan polls the status of the presence detector via 'MMI_ControlStatus' when motion is in progress (at the configured MMI polling interval).
                                        // The sensor status should be returned in the 'apState' parameter: a value greater than zero means that the sensor has triggered.
                                        // If the MetScan 'Halt On Presence Detector Signal' setting is on, all current MetScan motion will be halted if the triggered status is detected.
                                        // *** Note that the MetScan poll is for status / error reporting only. ***
                                        // All safety critical functions that are triggered by the presence detector MUST be implemented in hardware. ***
} eDevice;

// Definition of logical motor sensors bit flags. 
// Enum values correspond to a bit number in sensors status word. Bit set to '1' corresponds to an active sensor
typedef enum {
   MtrSens_Home = 0x00000000,   // Bit 0, state of the motor "home" sensor
   MtrSens_MinPosLimit = 0x00000001,   // Bit 1, state of the motor limit switch at min. position
   MtrSens_MaxPosLimit = 0x00000002,   // Bit 2, state of the motor limit switch at max. position
} eMotorSensor;

// GenericControl commands
typedef enum {
   ECtl_None = 0x00000000,   // Placeholder value which can be used to indicate "no command"
   ECtl_SetDevVoltage = 0x00000001,   // set some device voltage (just PWM value [0..100]). aOption: low word = device (see eDevice), apParam = PWM value
   ECtl_RunCmdScript = 0x00000002,   // Run MPC command script. aOption:0 -> (const char*)apParam points to the script itself;  aOption:1 -> (const char*)apParam points to the script file name;
   ECtl_RunCleaningProcedure = 0x00000003,   // Run cleaning procedure. aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_SetUvLampSettings = 0x00000004,   // Set up UV lamp configurations. aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_SetUvLampSwathCommand = 0x00000005,   // Set up UV lamp start and end positions. aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_RunStartSequence = 0x00000006,   // Custom action to be completed before every job (if enabled). aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_RunEndSequence = 0x00000007,   // Custom sequence to be completed at the end of every job (if enabled). aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_UpdateHeadTemperatureInfo = 0x00000008,   // Fixed list of heads target and actual temperatures (if enabled). aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_SendPrintProgress = 0x00000009,   // Last printed and total number of swath (if enabled). aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_RunGroupEndSequence = 0x0000000A,   // Custom sequence to be completed at the end of last job in a group (if enabled). aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_RunQueueEndSequence = 0x0000000B,   // Custom sequence to be completed at the end of last job in the queue (if enabled). aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_RunQueueStartSequence = 0x0000000D,   // Custom sequence to be completed at the start of the first job in the queue (if enabled). aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_SendJobGroupSize = 0x0000000C,   // Send information about job group size at the start of first job (if enabled). aOption:0 -> (const char *) String with listed parameters and corresponding values
   ECtl_RunInitialiseSequence = 0x0000000D,   // Optional sequence run when the MPCZ first connects
   ECtl_RunPurgeSequence = 0x0000000E,   // Run the (MPCZ) purge sequence
   ECtlTest_SetLogging = 0x00000100,   // set DLL logging options . aOption contains bitmask (combination of KLog_* constants). Useful for forcing logging on and off
   ECtlTest_UpdateMpcFw = 0x00000101,   // update MPC FW. aOption: board number [0..N], apParam points to the FW file name
   ECtlTest_GetMotor = 0x00000200,   // get a pointer to the TMotor instance. aOption: specifies axis, see eMotionAxis. apParam: const  TMotor**, out: const  TMotor*
   ECtlTest_SendRawCmd = 0x00000201,   // send raw MPC command and get response. apParam: TTestRawMpcCmd* If aOption==0 then TTestRawMpcCmd::cmd sent via command interface, if aOption==1, TTestRawMpcCmd::cmd sent directly to UART
} TGenCtlCmd;

// Bit fields reported by the 'apState' returned by <see cref="M:Ttp.Meteor.MotionIntegration.IMotion.MMI_ControlStatus(Ttp.Meteor.MotionIntegration.eDevice,System.UInt32,System.Int32@)" /> for the <see cref="F:Ttp.Meteor.MotionIntegration.eDevice.DEV_InkPump" /> device,
// used when the MMI is running in "generic pumps" mode.  The pump index sent to MMI_ControlStatus in the 'aIndex' parameter is from 1 to N.
typedef enum {
   GenericPumps_PumpControlEnabled = 0x00000001,   // Bit is set if pump control is currently enabled (i.e. the pump automatically turns on when the ink sensor reports not-full)
   GenericPumps_PumpActive = 0x00000002,   // Bit is set if the pump is currently active.
   GenericPumps_TankFullSensor = 0x00000004,   // Bit is set if the sensor is reporting tank full.
   GenericPumps_PumpTimeout = 0x00000008,   // Bit is set if the pump timed out due to running for too long without a tank full being seen.
   GenericPumps_EntityConfigured = 0x40000000,   // The pump+ink entity is connected and configured (for the MPC-Z, the configuration parameters are in the [MPC_Ink_InterfaceN] section in the configuration file).
} eGenericPumpBits;

typedef enum {
   GenericPumpCmd_Disable = 0x00000000,   // Disable the ink pump
   GenericPumpCmd_Enable = 0x00000001,   // Enable the ink pump, it automatically switches on when the sensor reports not-full
   GenericPumpCmd_ClearTimeoutAndDisable = 0x00000002,   // Clear an ink pump timeout error, but leave the pumps in a disabled state.  
                                                         // A further GenericPumpCmd_Enable command is required to re-enable pump control.
} eGenericPumpCmd;

// Structure for retrieving move status. See MoveStatus() API.
typedef struct {
   uint32_t   structSize;   // IN: must contain this structure size
   eAxisSate   axisState;   // OUT: axis state, see eAxisSate enum
   double   dist_mm;   // OUT: current absolute distance if applicable, mm
   uint32_t   axisHomed;   // OUT: true (1) when axis was homed and motor absolute position is known; false (0) otherwise
   uint32_t   activeSensorsBmp;   // OUT: Bitmap with a combination of bits describing activated motor sensors. Bit set to '1' indicates an active sensor. For bits definition see <see cref="T:Ttp.Meteor.MotionIntegration.eMotorSensor" />.
   uint32_t   assignedSensorsBmp;   // OUT: Bitmap describing the sensors assigned to the motor. Bit set to '1' at some position indicates that the motor uses corresponding sensor. For bits definition see <see cref="T:Ttp.Meteor.MotionIntegration.eMotorSensor" />.
} TMoveStat;

// Structure that describes keypad state. See documentation on how to set up and use keypad
typedef struct {
   uint32_t   KeysStateCurr;   // Current keypad state bitmap
   uint32_t   KeysStateLatched;   // Latched keypad state bitmap
} TKeypadState;

// For future use.
// [ Mandatory parameter which are sent to MMI_JobSequenceStart to set up the sequence for a print job.
// Additional application specific pass-through parameters can be sent as a separate JSON string if required. ]
typedef struct {
   uint32_t   StructureSizeBytes;   // Must contain the size of this structure in bytes
   uint32_t   Reserved;   // Fields will be added here in the future as required
} TMotionQueueStartArgs;

// Options for stopping the job sequence queue (MMI_JobSequenceStop)
typedef enum {
   JobSeqStop_CompleteMove = 0x00000000,   // stop the job sequence queue after the current move has completed
   JobSeqStop_CancelNormal = 0x00000001,   // stop the move immediately with a normal cancel, uses graceful motors stopping
   JobSeqStop_CancelUrgent = 0x00000002,   // stop the move with an urgent cancel, crash stop, do it as quick as possible
} eJobSequenceStopOption;

#define JobMotionQueueCmd_XAxisMove 0x00000001   // Bit set in <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.ControlFlags" /> if this move includes the X axis.  All X axis parameters should be ignored if this bit is clear.
#define JobMotionQueueCmd_IsPrintingMove 0x00000002   // Bit set in <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.ControlFlags" /> for a printing X move (otherwise the X movement does not print - e.g. the return pass for a 
                                                      // uni-directional print job)
#define JobMotionQueueCmd_YAxisMove 0x00000004   // Bit set in <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.ControlFlags" /> if this move includes the Y axis.  All Y axis parameters should be ignored if this bit is clear.
#define JobMotionQueueCmd_IsYAbsolute 0x00000008   // Bit set in <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.ControlFlags" /> for an absolute Y movement (otherwise it's a relative Y movement)
#define JobMotionQueueCmd_PauseAfterMove 0x00000010   // Bit set in <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.ControlFlags" /> to say that the MMI should enter job sequence pause immediately after this move has completed
                                                      // This bit is set if the application needs to temporarily take over control of the X and Y axes during a queued job, e.g. for
                                                      // spitting or a cleaning sequence
#define JobMotionQueueCmd_StopAfterMove 0x00000020   // Bit set in <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.ControlFlags" /> to say that the MMI should stop job sequence mode immediately after this move has completed
                                                     // This bit can be set for the last movement in a queued print job
                                                     // Note that it is also possible in some situations for this to be the only ControlFlag bit that is set, meaning that this is a
                                                     // "no operation" move terminating the job (e.g. if there is an auto-clean sequence after the final swath in a job)
#define JobMotionQueueCmd_HasPreviousXPos 0x00000040   // Indicates that the details of the **previous** X move are valid (<see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.PreviousXPositionMm" /> and <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.PreviousXSpeedMm_s" />).
                                                       // This will normally be the case for every move in a print job aside from the first one.
                                                       // These details can be used to make sure that the printer carriage is in the correct position prior to resuming job queue moves
                                                       // after a ResumeJobSequence call.  This is because manual moves during the pause period may have moved the carriage to a different position.  
                                                       // Alternatively, the MMI queue implementation can simply remember its previous move details internally, so it is free to ignore these details.
#define JobMotionQueueCmd_HasPreviousAbsYPos 0x00000080   // As with <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.JobMotionQueueCmd_HasPreviousXPos" /> but for absolute Y axis moves.
                                                          // Y relative moves are not tracked because it is assumed that a relative Y axis won't move during maintenance.
#define JobNotionQueueCmd_SkipPreviousWaitForY 0x00001000   // Bit set in <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.ControlFlags" /> if the X movement can start *before* the Y movement for the previous swath has completed.
                                                            // This relies on appropriate speeds and acceleration distances.
                                                            // At some points in the job sequence - such as if there is a "skipped" blank swath - this may not be possible, due to a larger than
                                                            // normal Y step.
#define JobNotionQueueCmd_MoveYAfterPrintComplete 0x00002000   // Bit set in <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.ControlFlags" /> if the Y axis move can start as soon as the carriage has passed XPrintEndMm.
                                                               // Otherwise, the Y axis move should not start until the X axis move has completed.

// Parameters sent to MMI_JobSequenceQueueMove, defining the X and Y movement for a swath.
// The carriage will have been placed in the correct position for starting the swath by the previous queued moved,
// or by a non-queued MMI_MoveTo command prior to the start of job sequence mode.
typedef struct {
   uint32_t   StructureSizeBytes;   // Must contain the size of this structure in bytes
   uint32_t   ControlFlags;   // See JobMotionQueueCmd_XXX
   double   XPrintStartMm;   // Absolute X position of the carriage where printing (with the leading head) starts.
                             // (Zero if JobMotionQueueCmd_IsPrintingMove is not set)
   double   XPrintEndMm;   // Absolute X position of the carriage where printing (with the trailing head) stops
                           // (Zero if JobMotionQueueCmd_IsPrintingMove is not set)
   double   XEndMm;   // Absolute X position of the carriage at the end of the swath
   double   XSpeedMm_s;   // If non-zero, override the current X axis speed for this move.
                          // If zero, the X axis speed set previously set by MMI_MoveSetSpeed (for <see cref="F:Ttp.Meteor.MotionIntegration.eMotionAxis.Axis1" />) should be used.
   double   PostSwathYDistMm;   // Y axis movement required after the swath has completed printing.
                                // Either absolute or relative, defined by the JobMotionQueueCmd_IsYAbsolute bit in <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueMoveArgs.ControlFlags" />.
                                // The Y movement can take place before the X movement has completed provided the X position is between XPrintEndDistMm and XEndDistMm.
   double   YSpeedMm_s;   // If non-zero, override the current Y axis speed for this move.
                          // If zero, the Y axis speed set previously set by MMI_MoveSetSpeed (for <see cref="F:Ttp.Meteor.MotionIntegration.eMotionAxis.Axis2" />) should be used.
   double   ZHeightMm;   // Z axis height which the printer should be at for this X/Y move.
                         // N.B. Included for tracking purposes only; Z axis moves are ** not ** currently queued as part of the job sequence queue.
   double   PreviousXPositionMm;   // If the JobMotionQueueCmd_HasPreviousXPos flag is set, this is the position the X axis should have arrived at
                                   // prior to starting this move.
   double   PreviousXSpeedMm_s;   // If the JobMotionQueueCmd_HasPreviousXPos flag is set, this is the speed of the previous X move.
                                  // Included primarily for debug checks.
   double   PreviousYAbsPositionMm;   // If the JobNotionQueueCmd_SkipPreviousWaitForY flag is set, this is the target Y position of the 
                                      // previous move.
                                      // ** N.B. If the JobNotionQueueCmd_SkipPreviousWaitForY flag is also set, this Y position may not yet be reached
                                      // when the X move starts.
                                      // This is OK providing the motion speeds are set appropriately, with the Y axis guaranteed to reach PreviousYAbsPositionMm
                                      // before the X axis reaches XPrintStartMm.
   double   PreviousYSpeedMm_s;   // If the JobMotionQueueCmd_HasPreviousAbsYPos flag is set, this is the speed of the previous Y move.
                                  // Included primarily for debug checks.
} TMotionQueueMoveArgs;

#define JobMotionQueueStatus_QueueIsStarted 0x00000001   // The MMI plugin must set this bit in the StatusFlags when the job sequence queue is started.  i.e.:
                                                         // - after [the JobSequenceStart call], when the job sequence queue is ready to accept TMotionQueueMoveArgs commands
                                                         // Once the bit is set, it must remain set until the queue stops (i.e. when the JobMotionQueueStatus_QueueIsStopped bit becomes set)
#define JobMotionQueueStatus_QueueIsStopped 0x00000004   // The MMI plugin must set this bit in the StatusFlags when an active job sequence queue stops.  i.e.:
                                                         // - after [the JobSequenceStop call] OR [a move with the JobMotionQueueCmd_StopAfterMove flag completes], after which the MMI plugin 
                                                         // is ready to accept single MoveTo commands.
                                                         // The flag must not be set until all associated job sequence motion has completed.
                                                         // Once MetScan has see this flag set, it will stop polling GetJobSequenceStatus.

// Structure for retrieving job queue sequence status.
typedef struct {
   uint32_t   StructureSizeBytes;   // IN: Must contain the size of this structure in bytes
   uint32_t   StatusFlags;   // OUT: Status flags
                             // - JobMotionQueueStatus_QueueIsStarted: Set if the MMI motion queue has fully started
                             // - JobMotionQueueStatus_QueueIsStopped: Set if the MMI motion queue mode has fully stopped and the MMI can accept standard commands
   uint32_t   TotalMovesQueued;   // OUT: The MMI plugin must report the total number of movements that have been queued via MMI_JobSequenceQueueMove since 
                                  // the queue was last enabled via MMI_JobSequenceStart.  Should be zero if job sequence mode is not currently enabled. 
                                  // **  N.B. This count includes all queued moves, not just the moves which result in printing  **
   uint32_t   TotalMovesCompleted;   // OUT: The MMI plugin must report the total number of queued movements that have fully completed since the queue was
                                     // last enabled via MMI_JobSequenceStart.  Should be zero if job sequence mode is not currently enabled.
                                     // **  N.B. All move commands sent to MMI_JobSequenceQueueMove must be included, not just the moves which result in printing **
   uint32_t   PrintingMovesCompleted;   // OUT: The MMI plugin must also report the total number of **printing** movements that have fully completed since the queue was
                                        // last enabled via MMI_JobSequenceStart.  Should be zero if job sequence mode is not currently enabled.
                                        // These are the moves which are queued with the JobMotionQueueCmd_IsPrintingMove flag set
   double   XAxisPosition;   // OUT: current X axis position in mm
   double   YAxisPosition;   // OUT: current Y axis position in mm
   double   QueueEnabledTimeMs;   // Placeholder for possible future use.  Currently unused.  [ The amount of time which has passed since JobSequenceStart ]
   double   QueuePausedTimeMs;   // Placeholder for possible future use.  Currently unused.  [ The amount of time the queue has spent in pause since JobSequenceStart ]
   double   QueuePrintingMoveTimeMs;   // Placeholder for possible future use.  Currently unused.  [ The amount of time the queue has spent running printing moves since JobSequenceStart ]
   uint32_t   PauseCount;   // The number of times that the job sequence queue has transitioned into the paused state since the JobSequenceStart call.
                            // A transition to pause can be either (a) after a JobSequencePause call, or (b) after a move with the JobMotionQueueCmd_PauseAfterMove flag completes.
                            // When PauseCount == (1 + ResumeCount), the job sequence queue is paused, and the MMI plugin is ready to accept single MoveTo commands.
                            // PauseCount should be reset to zero in the JobSequenceStart call.  Aside from this reset case, it should never decrease.
                            // See also <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueStatus.ResumeCount" />
   uint32_t   ResumeCount;   // The number of times that the job sequence queue has transitioned out of the paused state since the JobSequenceStart call.
                             // A transition out of pause happens after MetScan calls JobSequenceResume.
                             // When ResumeCount == PauseCount the job sequence queue is "not paused".
                             // ResumeCount should be reset to zero in the JobSequenceStart call.  Aside from this reset case, it should never decrease.
                             // See also <see cref="F:Ttp.Meteor.MotionIntegration.TMotionQueueStatus.PauseCount" />
} TMotionQueueStatus;

// Definitions of the MMI (Meteor Motion Integration) server error codes
// Can be returned from <see cref="M:Ttp.Meteor.MotionIntegration.SwathIPCServer.RunWithObject(Ttp.Meteor.MotionIntegration.IMotion)" />; <see cref="M:Ttp.Meteor.MotionIntegration.SwathIPCServer.RunWithUnmanaged(System.Int32,System.Boolean,System.Boolean)" />; 
// <see cref="M:Ttp.Meteor.MotionIntegration.SwathIPCServer.RunWithAssemblySearch(System.Boolean,System.Boolean,System.String)" />; and <see cref="M:Ttp.Meteor.MotionIntegration.SwathIPCServer.Stop" />
typedef enum {
   OK = 0x00000000,   // Server initialised OK
   IMOTION_NOT_FOUND = 0x00000001,   // Could not find an assembly containing an object which implements IMotion
   ALREADY_STARTED = 0x00000002,   // This instance of the server has already been started
   ALREADY_RUNNING = 0x00000003,   // The Meteor Motion Integration Server is already running in a process on this PC
   NOT_RUNNING = 0x00000004,   // The MMI server is not running
   IPC_PIPE_CREATE_FAILED = 0x00000005,   // Failed to create the IPC named pipes
   INVALID_UNMANAGED_INDEX = 0x00000006,   // The index passed to RunWithUnamanaged was outside the valid range
   NULL_IMOTION = 0x00000007,   // The value of IMotion passed to RunWithObject was null
   PATH_NOT_FOUND = 0x00000008,   // The path to the server executable is invalid
   PROCESS_START_FAILED = 0x00000009,   // Failed to start the child server process
   PROCESS_STOP_FAILED = 0x0000000A,   // Failed to stop the server process
   UNMANAGED_PLUGIN_DLL_NOT_FOUND = 0x0000000B,   // The unmanaged MMI plugin DLL (e.g. Meteor_MPC.dll or MotionController.dll) could not be found
   MOTIONINTEGRATIONCLS_NOT_FOUND = 0x0000000C,   // If the server app SwathIPCServer.exe is being used, MotionIntegrationCLS.dll must also be present in the same folder
   ALREADY_STOPPING = 0x0000000D,   // Overlapped calls to <see cref="M:Ttp.Meteor.MotionIntegration.SwathIPCServer.Stop" /> are not allowed
   BAD_PARAMS = 0x0000000E,   // Invalid command line parameters
} MMI_SERVER_ERROR;

#define MMI_SERVER_RUNNING_MUTEX "Global\SwathIPCServerMutex"   // Name of the mutex which is created when the motion integration server is running


}

#endif // MOTIONRESULT_H
