// This class is implemented to exactly match settings sent
// Please do not change its behavior
class TJobGroupSizeCommand {
public:
    TJobGroupSizeCommand();
    bool Initialise(char* aJsonString);
    int GetJobGroupSize();

private:
    int m_JobGroupSize;
};