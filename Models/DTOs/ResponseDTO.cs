namespace TecnoCredito.Models.DTOs;

public class ResponseDTO<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public string Error { get; set; } = null!;
    public T? Result { get; set; }

    public ResponseDTO()
    {
        IsSuccess = false;
    }

    private ResponseDTO(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Result = value;
        Error = error;
    }

    private ResponseDTO(bool isSuccess, T value, string error, string message)
    {
        IsSuccess = isSuccess;
        Result = value;
        Error = error;
        Message = message;
    }

    public static ResponseDTO<T> Success(T value) => new(true, value, null!);

    public static ResponseDTO<T> Success(T value, string message) =>
        new(true, value, null!, message);

    public static ResponseDTO<T> Failure(string error) => new(false, default!, error);

    public static ResponseDTO<T> Fail(string error) => new(false, default!, error);

    public static ResponseDTO<T> Failure(string error, string message) =>
        new(false, default!, error, message);

    public static ResponseDTO<T> Fail(string error, string message) =>
        new(false, default!, error, message);
}
