// This class is implemented to exactly match settings sent
// Please do not change its behavior
class TProgressInfo {
public:
    TProgressInfo();
    bool Initialise(char* aJsonString);
    int GetSwathPrinted();
    int GetSwathTotal();

private:
    int m_SwathPrinted;
    int m_SwathTotal;
};