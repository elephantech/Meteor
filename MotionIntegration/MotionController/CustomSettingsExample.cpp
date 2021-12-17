#include "CustomSettingsExample.h"
#include "json11.hpp"
#include <string>
#include <map>

using namespace json11;

TCustomSettingsExample::TCustomSettingsExample()
    : m_CyclesNumber(0)
    , m_Pressure(0.0)
    , mb_NeedsWiping(false) {
}

bool TCustomSettingsExample::Initialise(char* aJsonString) {
    if (aJsonString == nullptr) {
        return false;
    }

    std::string jsonStr(aJsonString), jsonErr;
    Json json = Json::parse(jsonStr, jsonErr);
    if (!json.is_object()) {
        return false;
    }

    std::map<std::string, Json> jsonMap = json.object_items();

    // Check if all expected items are present
    if (jsonMap.find("CyclesNumber") == jsonMap.end()
        || jsonMap.find("Pressure") == jsonMap.end()
        || jsonMap.find("NeedsWiping") == jsonMap.end()) {
        return false;
    }

    // Check if all items are of expected type
    if (!jsonMap.find("CyclesNumber")->second.is_number()
        || !jsonMap.find("Pressure")->second.is_number()
        || !jsonMap.find("NeedsWiping")->second.is_bool()) {
        return false;
    }

    // Apply found values
    m_CyclesNumber = jsonMap.find("CyclesNumber")->second.int_value();
    m_Pressure = jsonMap.find("Pressure")->second.number_value();
    mb_NeedsWiping = jsonMap.find("NeedsWiping")->second.bool_value();

    return true;
}

int TCustomSettingsExample::GetCyclesNumber() {
    return m_CyclesNumber;
}

double TCustomSettingsExample::GetPressure() {
    return m_Pressure;
}

bool TCustomSettingsExample::GetNeedsWiping() {
    return mb_NeedsWiping;
}