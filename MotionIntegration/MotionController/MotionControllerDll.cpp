#include <Windows.h>
#include "MotionControllerDll.h"
#include "UvLampSwathCommand.h"
#include "CustomSettingsExample.h"
#include "TemperatureInfos.h"
#include "ProgressInfo.h"
#include "JobGroupSizeCommand.h"
#include <mutex>

/***********************************************************************************/
static LogDelegate LogDel = NULL;
static std::mutex LogMutex;

void DoLog(const char * format, ...) {
    if (LogDel == NULL) {
        return;
    }
    std::lock_guard<std::mutex> lck(LogMutex);

    va_list args;
    va_start(args, format);
    size_t needed = vsnprintf(NULL, 0, format, args) + 1;
    char* buffer = (char*)malloc(needed);
    vsprintf_s(buffer, needed, format, args);
    SYSTEMTIME  sysTime;
    GetLocalTime(&sysTime);
    char* context;
    char* token = strtok_s(buffer, "\n", &context); //-- Split up each line into a separate message
    while (token != NULL) {
        size_t time = snprintf(NULL, 0, "%02d-%02d-%04d %02d:%02d:%02d.%03d %s", sysTime.wDay, sysTime.wMonth, sysTime.wYear, sysTime.wHour, sysTime.wMinute, sysTime.wSecond, sysTime.wMilliseconds, token) + 1;
        char* timeBuffer = (char*)malloc(time);
        sprintf_s(timeBuffer, time, "%02d-%02d-%04d %02d:%02d:%02d.%03d %s", sysTime.wDay, sysTime.wMonth, sysTime.wYear, sysTime.wHour, sysTime.wMinute, sysTime.wSecond, sysTime.wMilliseconds, token);
        LogDel(timeBuffer);
        free(timeBuffer);
        token = strtok_s(NULL, "\n", &context);
    }
    free(buffer);
    va_end(args);
}


MOTION_IFTYPE void MOTION_CALLCONV SetLogDelegate(LogDelegate apDelegate) {
    // When called:
    //  Called immediately after the dll has been loaded, when the SwathIPCServer.exe is run with
    //  the -ml command line option (the 'setMmiLogDelgate' flag in IPCServerLauncher.Start)
    //
    // Purpose:
    //  Provides a function pointer which the dll can use to send diagnostic messages back to
    //  the parent application (i.e. MetScan)
    //
    // Notes:
    //  This function is optional, if it's not found in the dll's export table this is not
    //  considered to be an error condition
    //
    LogDel = apDelegate;
    DoLog("---MotionDLL---\nSetLogDelegate\n---------------\n");
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV GetInterfaceVersion(uint32_t* apVer, uint32_t* apFeaturesSupported) {
    // When called:
    // Called at any time.  
    // Note that GetInterfaceVersion can be called **before** Initialise.
    //
    // The upper 16 bits of apVer must be set with the MMI interface version 
    // as defined in MotionResult.h.
    //
    // The lower 16 bits of apVer should uniquely identify this version of the
    // plugin software.
    //
    // apFeaturesSupported must be set with a bitmask indicating the high level features
    // which the plugin supports.  For most plugins this will just be MMI_Feature_Motors.
    //
    const uint16_t KVer_Dll       = 0x01;
    *apVer = ((uint32_t)KVer_MMI_Interface << 16) | KVer_Dll;
    *apFeaturesSupported = MMI_Feature_Motors;
    // 
    //
    // Returns:
    // MRES_OK
    DoLog("---MotionDLL---\nGetInterfaceVersion 0x%08x 0x%08x\n---------------\n", *apVer, *apFeaturesSupported);
    return MRES_OK;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV Initialise(const char* apCfgFilePath) {
    // When called:
    // Called when the application is first started.

    // Purpose:
    // Connect to hardware and perform any other initialisation of variables.
    // apCfgFilePath is for future use and will currently be an empty string.
    
    // Notes:
    // Can block the current thread if necessary.

    // Returns:
    // MRES_OK if successful. 
    // MRES_FAULT for an unrecoverable fault.
    DoLog("---MotionDLL---\nInitialise '%s'\n---------------\n", apCfgFilePath);
    return MRES_UNIMPL;
}


/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV ShutDown() {
    // When called:
    // Called at the end of the application. 
    // 
    // Purpose:
    // Any code needed to wind down and disconnect the hardware cleanly can be
    // added here.
    //
    // Notes:
    // This function should block the thread until the shutdown is complete.
    //
    // This function does not need to move the print carriage to home position,
    // as this will be done by the application.
    //
    // Returns:
    // MRES_OK only. The shutdown process can not prevent the application from 
    // stopping.
    DoLog("---MotionDLL---\nShutDown\n---------------\n");
    return 	MRES_UNIMPL;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveHome(eMotionAxis aAxis) {
    // When called:
    // - when the application is first started, after Initialise.
    // - occasionally during normal operation, to rehome on the home sensor.
    
    // Purpose:
    // Go through a homing routine to find the home sensor.
    
    // Notes:
    // Can block the current thread if necessary.
    
    // Returns:
    // MRES_OK if successful. 
    // MRES_BUSY if the print carriage is already moving.
    // MRES_FAULT for an unrecoverable fault.
    DoLog("---MotionDLL---\nMoveHome %d\n---------------\n", aAxis);
    return MRES_UNIMPL;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveTo(eMotionAxis aAxis, eMoveOption aMvOpt, double aDist_mm) {
    // When called:
    // Called whenever the application wants the printer carriage to move.
    //
    // Purpose:
    // This function will send the print carriage to the designated position in mm
    //
    // The move can be absolute or relative, depending on the value of aMvOpt
    //
    // MV_DistAbs selects an absolute move. For example, calling this function with aDist_mm=10 will
    // send the axis to 10mm from the home position. Calling this function with aDist_mm=10 a second time 
    // will not move the axis, as it is already at aDist_mm=10.
    //
    // MV_DistRel selects a relative move. For example, calling this function with aDist_mm=10 will 
    // move the axis 10mm from its current position. Calling this function with aDist_mm=10 a second time 
    // will move the axis another 10mm.
    //
    // Continuous movement: if MMI_Feature_ContininuousMovement is set to the apFeaturesSupported in GetInterfaceVersion
    // continuous movement controls will be enabled in MetScan UI. The continuous movement command uses +/- infinity
    // as the distance value. Continuous movement is stopped using MoveCancel method.
    // The following example code demonstrates continuous command detection:

    if (aDist_mm == std::numeric_limits<double>::infinity()) {
        // Positive continuous motion command
    }
    if (aDist_mm == -std::numeric_limits<double>::infinity()) {
        // Negative continuous motion command
    }

    //
    // Notes:
    // This function should start the motion sequence and then return. It should not block 
    // while the axis is moving.  The application will poll MoveStatus to find out when the
    // move has completed
    //
    // Returns:
    // MRES_OK if the motion sequence was started.
    // MRES_INTERRUPT if the motion was cancelled.
    // MOTION_BUSY if the print carriage is already moving.
    // MRES_FAULT for an unrecoverable fault.
    // MRES_NOTINIT if the motion controller is not initialised. 
    DoLog("---MotionDLL---\nMoveTo %d %d %.2f\n---------------\n", aAxis, aMvOpt, aDist_mm);    
    return 	MRES_UNIMPL;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveCancel(eMotionAxis aAxis, eCancelOption aOption) {
    // When called:
    // Can be called at any point. 
    //
    // Purpose:
    // Cancel an in-progress move on the axis.
    // Also used as a general cancel for any non-axis movement which the plugin may be controlling,
    // with aAxis set to AxisNone (0)
    //
    // Returns:
    // MRES_OK if the Cancel sequence was started, or if there's nothing to cancel
    // MRES_FAULT otherwise.
    DoLog("---MotionDLL---\nMoveCancel %d %d\n---------------\n", aAxis, aOption);    
    return MRES_UNIMPL;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveStatus(eMotionAxis aAxis, TMoveStat* apMoveStat) {
    // When called:
    // At any time.  For example, after sending a motion command (Home, MoveTo), the application
    // checks whether the command has finished or not, and the current position of the axis.
    //
    // Purpose:
    // This function is used to determine when it is safe to send more motion commands.
    // The returned data is also used for the application to monitor the health
    // of the motion system.
    //
    // Returns:
    // MRES_OK if successful and motion is operating normally. 
    // MRES_FAULT if a serious fault occurred (i.e. the motion has jammed).
    // MRES_STRUCTSIZE_MISMATCH if the size of TMoveStat is incorrect (to protect against
    // version incompatibility)

    if ( apMoveStat->structSize != sizeof(TMoveStat) ) {
        DoLog("---MotionDLL---\nTMoveStat size mismatch\n");
        return MRES_STRUCTSIZE_MISMATCH;
    }

    // -- Set some dummy data --
    switch ( aAxis ) {
    case Axis1:
        // Axis is idle
        apMoveStat->axisState = AxS_Idle;
        apMoveStat->dist_mm = 123;
        // Axis has previously been successfully homed
        apMoveStat->axisHomed = true;
        // Home, limit+ and limit- sensors fitted; limit- active
        apMoveStat->activeSensorsBmp = (1 << MtrSens_MinPosLimit);
        apMoveStat->assignedSensorsBmp = (1 << MtrSens_Home) | (1 << MtrSens_MinPosLimit) | (1 << MtrSens_MaxPosLimit);
        break;
    case Axis2:
        // Move is in progress on the axis
        apMoveStat->axisState = AxS_Busy;
        apMoveStat->dist_mm = 456;
        apMoveStat->axisHomed = false;
        // No sensors on the axis
        apMoveStat->activeSensorsBmp = 0;
        apMoveStat->assignedSensorsBmp = 0;
        break;
    case Axis3:
        // Axis is idle
        apMoveStat->axisState = AxS_Idle;
        apMoveStat->dist_mm = 0;
        apMoveStat->axisHomed = true;
        // Axis has home sensor only; home sensor is active
        apMoveStat->activeSensorsBmp = (1 << MtrSens_Home);
        apMoveStat->assignedSensorsBmp = (1 << MtrSens_Home);
        break;
    }

    return 	MRES_OK;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveSetSpeed(eMotionAxis aAxis, eMSpeedOption aOpt, double aSpeed_mm_s) {
    // When called:
    // At any time, normally when an axis is not in motion.
    //
    // Purpose:
    // This function is used to set the speed of the axis for subsequent moves.
    // If a move is currently in progress the speed should not take effect until the next move
    //
    // Notes:
    // If the requested speed is larger than the maximum allowed speed, the axis speed should be
    // set to the maximum, and MRES_BADPARAM should be returned
    // If the requested speed is smaller than the minimum allowed speed, the axis speed should be
    // set to its minimum, and MRES_BADPARAM should be returned
    //
    // Returns:
    // MRES_OK if the speed was set successfully
    // MRES_BADPARAM if the requested speed was out of range
    DoLog("---MotionDLL---\nMoveSetSpeed %d %d %.2f\n---------------\n", aAxis, aOpt, aSpeed_mm_s);
    return MRES_UNIMPL;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV ControlSetState(eDevice aDev, uint32_t aIndex, int32_t aState) {
    // Purpose:
    // This function can be used to enable implementation specific PLC functionality.
    // It does not need to be implemented for standard motion control.
    //
    DoLog("---MotionDLL---\nControlSetState %d %d %d\n---------------\n", aDev, aIndex, aState);
    return MRES_UNIMPL;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV ControlStatus(eDevice aDev, uint32_t aIndex, int32_t* apState) {
    // Purpose:
    // This function can be used to enable implementation specific PLC status.
    // It does not need to be implemented for standard motion control.
    //
    DoLog("---MotionDLL---\nControlStatus %d %d\n---------------\n", aDev, aIndex);
    return MRES_UNIMPL;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV GenericControl(int32_t aCommand, int32_t aOption, int32_t aParamSize, uint32_t* apParam) {
    // Purpose:
    // This function can be used to specify plugin specific commands.
    // It does not need to be implemented for standard motion control.
    //
    DoLog("---MotionDLL---\nGenericControl %d %d\n", aCommand, aOption);

    DoLog("%d Params: ", aParamSize);
    for ( int i = 0; i < aParamSize; i++ ) {
        DoLog(" %d ", apParam[i]);
    }
    DoLog("\n---------------\n");
    return MRES_UNIMPL;
}

/***********************************************************************************/

MOTION_IFTYPE eMotionResult MOTION_CALLCONV GenericControlStr(int32_t aCommand, int32_t aOption, int32_t aParamSize, char* apParam) {
    // Purpose:
    // This function can be used to specify plugin specific commands.
    // It does not need to be implemented for standard motion control.
    //
    DoLog("---MotionDLL---\nGenericControlStr %d %d\n", aCommand, aOption);

    // Example of parsing UV lamp swath command
    // Beware that example is based on fixed setting class and therefore best to be used as is
    if (aCommand == ECtl_SetUvLampSwathCommand) {
        TUvLampSwathCommand lsc;
        if(!lsc.Initialise(apParam)) {
            DoLog("Failed to parse JSON string for command %d\n", aCommand);
        } else {
            DoLog("Swath command parsed successfully Start: %lf End: %lf Reverse: %d\n", lsc.GetPrintStart(), lsc.GetPrintEnd(), lsc.GetIsReverse());
        }
    }

    // Example of parsing print progress command
    // Beware that example is based on fixed setting class and therefore best to be used as is
    if (aCommand == ECtl_SendPrintProgress) {
        TProgressInfo pi;
        if (!pi.Initialise(apParam)) {
            DoLog("Failed to parse JSON string for command %d\n", aCommand);
        }
        else {
            DoLog("Swath command parsed successfully Swath Printed: %d Swath Total: %d.\n", pi.GetSwathPrinted(), pi.GetSwathTotal());
        }
    }

    // This is example code that can be used for UV lamp settings, cleaning settings,
    // start and end sequences. Please modify it as needed. It is you responsibility to maintain
    // settings parser compatible with the settings you are setting in the MetScan UI
    if (aCommand == ECtl_SetUvLampSettings || aCommand == ECtl_RunCleaningProcedure || aCommand == ECtl_RunStartSequence || aCommand == ECtl_RunEndSequence || aCommand == ECtl_RunGroupEndSequence || aCommand == ECtl_RunQueueEndSequence) {
        TCustomSettingsExample settings;
        if (!settings.Initialise(apParam)) {
            DoLog("Failed to parse JSON string for command %d\n", aCommand);
        } else {
            DoLog("Swath command parsed successfully CyclesNumber: %d Pressure: %lf NeedsWiping: %d\n", settings.GetCyclesNumber(), settings.GetPressure(), settings.GetNeedsWiping());
        }
    }

    // Example of parsing update temperature info command
    // Beware that example is based on fixed setting class and therefore best to be used as is
    if (aCommand == ECtl_UpdateHeadTemperatureInfo) {
        TTemperatureInfos tInfos;
        if (!tInfos.Initialise(apParam)) {
            DoLog("Failed to parse JSON string for command %d\n", aCommand);
        } else {
            std::vector<TTemperatureInfo> resultVec = tInfos.GetVectorOfTemperatureInfos();
            for (std::vector<TTemperatureInfo>::iterator it = resultVec.begin(); it != resultVec.end(); ++it) {
                DoLog("Update temperature info command parsed successfully TargetTemperature: %lf ActualTemperature: %lf\n", it->GetTargetTemperature(), it->GetActualTemperature());
            }
        }
    }

    // Example of parsing print progress command
    // Beware that example is based on fixed setting class and therefore best to be used as is
    if (aCommand == ECtl_SendJobGroupSize) {
        TJobGroupSizeCommand jgs;
        if (!jgs.Initialise(apParam)) {
            DoLog("Failed to parse JSON string for command %d\n", aCommand);
        }
        else {
            DoLog("Swath command parsed successfully Job Group Size: %d.\n", jgs.GetJobGroupSize());
        }
    }

    DoLog("\n---------------\n");
    return MRES_UNIMPL;
}


/***********************************************************************************/
// Job Sequence Queue API
// ----------------------


//
// Dummy status fields.  In this sample code, we don't have a PLC to talk to, so we
// just simulate some of the high level status reporting
//

bool g_isJobSequenceModeStarted = false;
bool g_isJobSequenceStopped = false;
uint32_t g_jobSequencePauseCount = 0;   // -- The number of times the queue has entered pause, should always be g_jobSequenceResumeCount or g_jobSequenceResumeCount + 1
uint32_t g_jobSequenceResumeCount = 0;  // -- The number of times the queue has exited pause, should always be g_jobSequencePauseCount or g_jobSequencePauseCount - 1
uint32_t g_totalMoves = 0;
uint32_t g_printingMoves = 0;

/*
    The below methods are optional, for plugins which support the job sequence queue API.
    
    This allows multiple moves for a print job to be queued on the PLC itself, which can 
    help to reduce swath to swath turnaround.

    GetInterfaceVersion must set the MMI_Feature_JobSequenceQueue feature bit if the job
    sequence queue is supported.

    See MotionIntegrationHelp.chm for details.
*/
MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequenceStart(const TMotionQueueStartArgs* apParams, const char* apAdditionalParams) {
    if (apParams->StructureSizeBytes != sizeof(TMotionQueueStartArgs)) {
        return MRES_STRUCTSIZE_MISMATCH;
    }
    //
    // apParams and apAdditionalParams are for future used and currently contain no information
    // - In the future, additional fields may be added to TMotionQueueStartArgs as required
    // - apAdditionalParams is intended as a general purpose additional command string, e.g. we may
    //   use a JSON formatted string here to set application specific parameters in the future
    //

    //
    // Here the command to put the PLC into "job sequence queue" mode must be sent.
    // Once the PLC reports it's in job sequence mode, the application will not send any standard Move_To commands
    //

    // 
    // All we can do in the sample code is check we're not currently in job sequence mode (which is an error),
    // and then switch the mode on
    //
    if (g_isJobSequenceModeStarted) {
        return MRES_JOB_SEQ_ACTIVE;
    }
    DoLog("---MotionDLL---\nJobSequenceStart\n---------------\n");
    g_isJobSequenceModeStarted = true;      //-- This flag does not necessarily have to go true immediately, it may need a PLC handshake first
    g_isJobSequenceStopped = false;
    g_jobSequencePauseCount = 0;
    g_jobSequenceResumeCount = 0;
    g_totalMoves = 0;
    g_printingMoves = 0;
    return MRES_OK;
}

MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequenceStop(eJobSequenceStopOption aOption) {
    //
    // aOption tells us how the application wants to initiate the stop
    
    //
    // For the sample, just turn job sequence mode off
    // Note that it is *not* an error here if job sequence mode is currently inactive, the
    // application may call JobSequenceStop multiple times during an abort sequence
    // 
    DoLog("---MotionDLL---\nJobSequenceStop\n---------------\n");
    g_isJobSequenceStopped = true;
    g_isJobSequenceModeStarted = false;
    return MRES_OK;
}

MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequencePause() {
    if (!g_isJobSequenceModeStarted) {
        return MRES_JOB_SEQ_INACTIVE;
    }
    if (g_jobSequencePauseCount != g_jobSequenceResumeCount) {
        return MRES_JOB_SEQ_QUEUE_ALREADY_PAUSED;
    }

    // !!!
    // !!! It is possible for a pause command to get through here when the PLC is about to enter pause, if it
    // !!! is about to complete a queued 'JobMotionQueueCmd_PauseAfterMove' command.
    // !!! In this case, the pause count should still only increment once.
    // !!!

    DoLog("---MotionDLL---\nJobSequencePause\n---------------\n");
    //
    // At this point the PLC should be told to pause processing commands from its job queue, the pause taking effect **after any in-progress move has completed**
    // In a real system, g_jobSequencePauseCount must only increase once the PLC side is fully paused
    // g_jobSequencePauseCount can also increase after a previously queued move with the 'JobMotionQueueCmd_PauseAfterMove' flag completes on the PLC
    // If a pause command reaches the PLC when its job queue is already paused, g_jobSequencePauseCount must *not* increase multiple times
    // 
    g_jobSequencePauseCount++;
    return MRES_OK;
}

MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequenceResume() {
    if (!g_isJobSequenceModeStarted) {
        return MRES_JOB_SEQ_INACTIVE;
    }
    if (g_jobSequencePauseCount == g_jobSequenceResumeCount) {
        return MRES_JOB_SEQ_QUEUE_NOT_PAUSED;
    }

    DoLog("---MotionDLL---\nJobSequenceResume\n---------------\n");
    //
    // At this point the PLC should be told to resume processing commands from its job queue
    // 
    g_jobSequenceResumeCount++;
    return MRES_OK;
}

MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequenceQueueMove(const TMotionQueueMoveArgs* apParams) {
    if (apParams->StructureSizeBytes != sizeof(TMotionQueueMoveArgs)) {
        return MRES_STRUCTSIZE_MISMATCH;
    }
    if (!g_isJobSequenceModeStarted) {
        return MRES_JOB_SEQ_INACTIVE;
    }

    //
    // Here the details of the move must be sent to the PLC, which should be implementing a
    // queue of moves.  The new move is added to the end of the queue.
    //

    //
    // Bits are set in apParams->ControlFlags to indicate the type of move - e.g. is it X then Y,
    // X only, Y only, is it a "printing" or a "non-printing" move etc.
    //
    const bool isXMove = (apParams->ControlFlags & JobMotionQueueCmd_XAxisMove) != 0;
    const bool isPrintingMove = (apParams->ControlFlags & JobMotionQueueCmd_IsPrintingMove) != 0;
    const bool isYMove = (apParams->ControlFlags & JobMotionQueueCmd_YAxisMove) != 0;
    const bool isMoveYAfterPrintComplete = (apParams->ControlFlags & JobNotionQueueCmd_MoveYAfterPrintComplete) != 0;
    const bool isSkipWaitForY  = (apParams->ControlFlags & JobNotionQueueCmd_SkipPreviousWaitForY) != 0;
    const bool isPauseAfterMove = (apParams->ControlFlags & JobMotionQueueCmd_PauseAfterMove) != 0;
    const bool isStopAfterMove = (apParams->ControlFlags & JobMotionQueueCmd_StopAfterMove) != 0;

    DoLog("---MotionDLL---\nJobSequenceQueueMove IsX=%d IsPrintMove=%d IsY=%d isMoveYAfterPrintComplete=%d isSkipYWaitForY=%d isPause=%d isStop=%d\n---------------\n",
          isXMove, isPrintingMove, isYMove, isMoveYAfterPrintComplete, isSkipWaitForY, isPauseAfterMove, isStopAfterMove);

    //
    // For the sample, we just count the number of moves as if they had already completed
    //
    g_totalMoves++;
    if (isPrintingMove) {
        g_printingMoves++;
    }
    

    //
    // There are two important queue control flags which are part of the queued move structure.
    // These tell the job queue to (a) enter pause after the move has completed, or (b) exit job sequence mode after the move has completed.
    //
    // (a) is used for the situation where MetScan needs to carry out in-job maintenance actions (spitting, head cleaning) so must
    //     take over control again and use the standard Move_To commands
    // (b) is used at the end of a print job
    //
    // Here we just immediately move into the pause / stop state.
    // In a real implementation, this must not happen until the PLC has reached the corresponding command in its queue.
    //
    if (isPauseAfterMove) {
        g_jobSequencePauseCount++;
    }
    if (isStopAfterMove) {
        g_isJobSequenceModeStarted = false;
    }

    return MRES_OK;
}

MOTION_IFTYPE eMotionResult MOTION_CALLCONV GetJobSequenceStatus(TMotionQueueStatus* apParams) {
    if (apParams->StructureSizeBytes != sizeof(TMotionQueueStatus)) {
        return MRES_STRUCTSIZE_MISMATCH;
    }

    //
    // Report the current job queue status including the current positions of the X and Y axes.
    // Normally this information will come from the PLC.
    // Here we just fill in a few sample fields set in the example code above.
    // Note that it is valid for GetJobSequenceStatus to be called irrespective of whether job sequence mode
    // is currently active, for any plugin which supports the MMI_Feature_JobSequenceQueue feature.
    //
    apParams->StatusFlags = 0;
    if (g_isJobSequenceModeStarted) {
        apParams->StatusFlags |= JobMotionQueueStatus_QueueIsStarted;
    }
    if (g_isJobSequenceStopped) {
        apParams->StatusFlags |= JobMotionQueueStatus_QueueIsStopped;
    }
    apParams->PauseCount = g_jobSequencePauseCount;
    apParams->ResumeCount = g_jobSequenceResumeCount;
    apParams->TotalMovesQueued = g_totalMoves;
    apParams->TotalMovesCompleted = g_totalMoves;
    apParams->PrintingMovesCompleted = g_printingMoves;
    apParams->XAxisPosition = 123;
    apParams->YAxisPosition = 456;

    //
    // There are a few placeholder TMotionQueueStatus fields for possible future use if
    // we want to introduce job queue time tracking diagnostics in the PLC - e.g. to report 
    // how much time it spends in job queue mode, how much time it spends in actual movements etc.
    //
    // These fields are QueueEnabledTimeMs, QueuePausedTimeMs and QueuePrintingMoveTimeMs.
    // Currently they are ignored by MetScan so should not be used.
    // 

    return MRES_OK;
}

/***********************************************************************************/
