#include <vector>
#include "TemperatureInfo.h"

// This class is implemented to exactly match settings sent
// Please do not change its behavior
class TTemperatureInfos {
public:
    TTemperatureInfos();
    bool Initialise(char* aJsonString);
    std::vector<TTemperatureInfo> GetVectorOfTemperatureInfos();

private:
    std::vector<TTemperatureInfo> m_VectorOfTemperatureInfos;
};
