namespace API;

public class ApiException
{

    public int Statuscode { get; set; }

    public string Message { get; set; }

    public string Details { get; set; }


    public ApiException(int code, string message, string details)
    {
        Statuscode = code;
        Message = message;
        Details = details;
    }

}
