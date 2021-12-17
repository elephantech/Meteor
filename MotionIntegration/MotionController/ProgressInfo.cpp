#include "ProgressInfo.h"
#include "json11.hpp"
#include <string>
#include <map>

using namespace json11;

TProgressInfo::TProgressInfo()
    : m_SwathPrinted(0)
    , m_SwathTotal(0) {
}

bool TProgressInfo::Initialise(char* aJsonString) {
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
    if (jsonMap.find("SwathPrinted") == jsonMap.end()
        || jsonMap.find("SwathTotal") == jsonMap.end()) {
        return false;
    }

    // Check if all items are of expected type
    if (!jsonMap.find("SwathPrinted")->second.is_number()
        || !jsonMap.find("SwathTotal")->second.is_number()) {
        return false;
    }

    // Apply found values
    m_SwathPrinted = jsonMap.find("SwathPrinted")->second.int_value();
    m_SwathTotal = jsonMap.find("SwathTotal")->second.int_value();

    return true;
}

int TProgressInfo::GetSwathPrinted() {
    return m_SwathPrinted;
}

int TProgressInfo::GetSwathTotal() {
    return m_SwathTotal;
}