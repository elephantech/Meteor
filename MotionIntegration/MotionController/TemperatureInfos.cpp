#include "TemperatureInfos.h"
#include "json11.hpp"
#include <string>
#include <map>

using namespace json11;

TTemperatureInfos::TTemperatureInfos() = default;

bool TTemperatureInfos::Initialise(char* aJsonString) {
    if (aJsonString == nullptr) {
        return false;
    }

    std::string jsonStr(aJsonString), jsonErr;
    Json json = Json::parse(jsonStr, jsonErr);
    if (!json.is_array()) {
        return false;
    }

    std::vector<Json> jsonVec = json.array_items();

    if (jsonVec.empty()) {
        return false;
    }

    for (std::vector<Json>::iterator it = jsonVec.begin(); it != jsonVec.end(); ++it) {
        TTemperatureInfo info;
        if (info.Initialise(*it)) {
            m_VectorOfTemperatureInfos.push_back(info);
        }
    }

    return true;
}

std::vector<TTemperatureInfo> TTemperatureInfos::GetVectorOfTemperatureInfos() {
    return m_VectorOfTemperatureInfos;
}