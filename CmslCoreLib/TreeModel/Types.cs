namespace CmslCore.TreeModel
{
    public interface ITMParent
    {
        void TaskRequest(object child, int code, object state);
    }

    public interface ITMChild
    {
        // 받기
        void SetParent(ITMParent parent);
    }
}
