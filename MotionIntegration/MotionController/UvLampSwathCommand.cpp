#include "UvLampSwathCommand.h"
#include "json11.hpp"
#include <string>
#include <map>

using namespace json11;

TUvLampSwathCommand::TUvLampSwathCommand()
    : m_PrintStart(0.0)
    , m_PrintEnd(0.0)
    , mb_IsReverse(false) {
}

bool TUvLampSwathCommand::Initialise(char* aJsonString) {
    if (aJsonString == nullptr) {
        return false;
    }

    std::string jsonStr (aJsonString), jsonErr;
    Json json = Json::parse(jsonStr, jsonErr);
    if (!json.is_object()) {
        return false;
    }

    std::map<std::string, Json> jsonMap = json.object_items();

    // Check if all expected items are present
    if (jsonMap.find("PrintStart") == jsonMap.end()
        || jsonMap.find("PrintEnd") == jsonMap.end()
        || jsonMap.find("IsReverse") == jsonMap.end()) {
        return false;
    }

    // Check if all items are of expected type
    if (!jsonMap.find("PrintStart")->second.is_number()
        || !jsonMap.find("PrintEnd")->second.is_number()
        || !jsonMap.find("IsReverse")->second.is_bool()) {
        return false;
    }

    // Apply found values
    m_PrintStart = jsonMap.find("PrintStart")->second.number_value();
    m_PrintEnd = jsonMap.find("PrintEnd")->second.number_value();
    mb_IsReverse = jsonMap.find("IsReverse")->second.bool_value();

    return true;
}

double TUvLampSwathCommand::GetPrintStart() {
    return m_PrintStart;
}

double TUvLampSwathCommand::GetPrintEnd() {
    return m_PrintEnd;
}

bool TUvLampSwathCommand::GetIsReverse() {
    return mb_IsReverse;
}