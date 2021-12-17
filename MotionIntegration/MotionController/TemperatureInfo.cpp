#include "TemperatureInfo.h"
#include <string>
#include <map>

using namespace json11;

TTemperatureInfo::TTemperatureInfo()
    : m_TargetTemperature(0.0)
    , m_ActualTemperature(0.0) {
}

bool TTemperatureInfo::Initialise(Json aJson) {
    if (aJson == nullptr) {
        return false;
    }

    if (!aJson.is_object()) {
        return false;
    }

    std::map<std::string, Json> jsonMap = aJson.object_items();

    // Check if all expected items are present
    if (jsonMap.find("TargetTemperature") == jsonMap.end()
        || jsonMap.find("ActualTemperature") == jsonMap.end()) {
        return false;
    }

    // Check if all items are of expected type
    if (!jsonMap.find("TargetTemperature")->second.is_number()
        || !jsonMap.find("ActualTemperature")->second.is_number()) {
        return false;
    }

    // Apply found values
    m_TargetTemperature = jsonMap.find("TargetTemperature")->second.number_value();
    m_ActualTemperature = jsonMap.find("ActualTemperature")->second.number_value();

    return true;
}

double TTemperatureInfo::GetTargetTemperature() {
    return m_TargetTemperature;
}

double TTemperatureInfo::GetActualTemperature() {
    return m_ActualTemperature;
}