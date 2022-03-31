namespace MyToDo.Api.Service
{
    public class ApiResponse
    {
        public ApiResponse(string message,bool status = false)
        {
            this.Message = message;
            this.Status = status;
        }
        public ApiResponse(bool stauts,object result)
        {
            this.Status = stauts;
            this.Result = result;
        }
        public string Message { get; set; }
        public bool Status { get; set; }
        public object Result { get; set; }
    }
}
