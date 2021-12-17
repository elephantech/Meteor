#include "JobGroupSizeCommand.h"
#include "json11.hpp"
#include <string>
#include <map>

using namespace json11;

TJobGroupSizeCommand::TJobGroupSizeCommand()
    : m_JobGroupSize(1) {
}

bool TJobGroupSizeCommand::Initialise(char* aJsonString) {
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
    if (jsonMap.find("JobGroupSize") == jsonMap.end()) {
        return false;
    }

    // Check if all items are of expected type
    if (!jsonMap.find("JobGroupSize")->second.is_number()) {
        return false;
    }

    // Apply found values
    m_JobGroupSize = jsonMap.find("JobGroupSize")->second.int_value();

    return true;
}

int TJobGroupSizeCommand::GetJobGroupSize() {
    return m_JobGroupSize;
}