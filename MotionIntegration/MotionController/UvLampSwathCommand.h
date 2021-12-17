// This class is implemented to exactly match settings sent
// Please do not change its behavior
class TUvLampSwathCommand {
public:
    TUvLampSwathCommand();
    bool Initialise(char* aJsonString);
    double GetPrintStart();
    double GetPrintEnd();
    bool GetIsReverse();

private:
    double m_PrintStart;
    double m_PrintEnd;
    bool mb_IsReverse;
};