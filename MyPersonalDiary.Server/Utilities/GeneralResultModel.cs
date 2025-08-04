namespace MyPersonalDiary.Server.Utilities
{
    public class GeneralResultModel
    {
        public List<string> Errors { get; set; } = new List<string>();
        public object? Result { get; set; }

        public bool HasErrors => Errors.Any();
    }
}
