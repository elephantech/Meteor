#include "json11.hpp"

// This class is implemented to exactly match settings sent
// Please do not change its behavior
class TTemperatureInfo {
public:
    TTemperatureInfo();
    bool Initialise(json11::Json aJson);
    double GetTargetTemperature();
    double GetActualTemperature();

private:
    double m_TargetTemperature;
    double m_ActualTemperature;
};