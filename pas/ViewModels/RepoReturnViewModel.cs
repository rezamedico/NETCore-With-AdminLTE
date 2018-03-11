namespace pas.ViewModels
{
    public class RepoReturnViewModel<T>
    {
        public string Messages { get; set; }
        public T Payload { get; set; }
    }

    public class RepoReturnViewModel
    {
        public string Messages { get; set; }
        public object Payload { get; set; }
    }

}
