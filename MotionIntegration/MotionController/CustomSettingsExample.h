// This class is implemented as an example
// Please create any parameters you need such that they would much custom settings you set in the MetScan UI
class TCustomSettingsExample {
public:
    TCustomSettingsExample();
    bool Initialise(char* aJsonString);
    int GetCyclesNumber();
    double GetPressure();
    bool GetNeedsWiping();

private:
    int m_CyclesNumber;
    double m_Pressure;
    bool mb_NeedsWiping;
};