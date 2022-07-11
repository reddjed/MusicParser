namespace BusinessLogic.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string? Message { get; }

        protected Result(bool isSuccess, T? data, string? message = null) =>
            (IsSuccess, Data, Message) = (isSuccess, data, message);

        public static Result<T> Success(T? data) =>
            new(true, data, null);
        public static Result<T> Error(string? message) =>
            new(false, default, message);
    }
}
