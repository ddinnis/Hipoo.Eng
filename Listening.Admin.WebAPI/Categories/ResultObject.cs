namespace Listening.Admin.WebAPI.Categories
{
    public class ResultObject
    {
        public int Code { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public bool Ok { get; set; }
        public int StatusCode { get; internal set; }
    }
}
