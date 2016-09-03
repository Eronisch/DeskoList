namespace Core.Models
{
    public class ResultModel
    {
        public ResultModel()
        {
            
        }

        public ResultModel(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess
        {
            get { return string.IsNullOrEmpty(ErrorMessage); }
        }

        public string ErrorMessage { get; set; }
    }
}
