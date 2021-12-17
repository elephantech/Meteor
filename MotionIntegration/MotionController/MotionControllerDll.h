/**
    Meteor Motion Controller API.

    @file MotionControllerDll.h
*/

#ifndef MotionControllerDllH
#define MotionControllerDllH

#include <stdint.h>
#include <stdio.h>
#include <stdarg.h>
#include "MotionResult.h"

#ifdef _MOTIONDLL           // Only defined in the DLL project itself
#define MOTION_IFTYPE __declspec(dllexport)
#else
#define MOTION_IFTYPE __declspec(dllimport)
#endif

#define MOTION_CALLCONV __cdecl


using namespace MeteorMotionIntegration;


#ifdef __cplusplus
extern "C" {
#endif

    typedef void(_stdcall *LogDelegate)(char*);

    MOTION_IFTYPE void MOTION_CALLCONV SetLogDelegate(LogDelegate apDelegate);

    MOTION_IFTYPE eMotionResult MOTION_CALLCONV GetInterfaceVersion(uint32_t* apVer, uint32_t* apFeaturesSupported);

    MOTION_IFTYPE eMotionResult MOTION_CALLCONV Initialise(const char* apCfgFilePath);
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV ShutDown();
    
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveHome(eMotionAxis aAxis);
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveTo(eMotionAxis aAxis, eMoveOption aMvOpt, double aDist_mm);
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveCancel(eMotionAxis aAxis, eCancelOption aOption);
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveStatus(eMotionAxis aAxis, TMoveStat* apMoveStat); 
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV MoveSetSpeed(eMotionAxis aAxis, eMSpeedOption aOpt, double aSpeed_mm_s); 

    MOTION_IFTYPE eMotionResult MOTION_CALLCONV ControlSetState(eDevice aDev, uint32_t aIndex, int32_t aState);
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV ControlStatus(eDevice aDev, uint32_t aIndex, int32_t* apState);
    
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV GenericControl(int32_t aCommand, int32_t aOption, int32_t aParamSize, uint32_t* apParam);
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV GenericControlStr(int32_t aCommand, int32_t aOption, int32_t aParamSize, char* apParam);

    // ==== Optional job motion queue API ===
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequenceStart(const TMotionQueueStartArgs* apParams, const char* apAdditionalParams);
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequenceStop(eJobSequenceStopOption aOption);
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequencePause();
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequenceResume();
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV JobSequenceQueueMove(const TMotionQueueMoveArgs* apParams);
    MOTION_IFTYPE eMotionResult MOTION_CALLCONV GetJobSequenceStatus(TMotionQueueStatus* apParams);

#ifdef __cplusplus
}
#endif

#endif
