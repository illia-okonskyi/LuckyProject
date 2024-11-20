namespace LuckyProject.Lib.Web.Models
{
    public class LpApiResponse
    {
        public bool Success { get; init; } = true;
        public AbstractLpApiError LpApiError { get; init; }

        public static LpApiResponse Create()
        {
            return new LpApiResponse { Success = true };
        }

        public static LpApiResponse<TPayload> Create<TPayload>(TPayload payload)
        {
            return new LpApiResponse<TPayload> { Success = true, LpApiPayload = payload };
        }

        public static LpApiResponse Create(AbstractLpApiError lpApiError)
        {
            return new LpApiResponse { Success = false, LpApiError = lpApiError };
        }
    }

    public class LpApiResponse<TPayload> : LpApiResponse
    {
        public TPayload LpApiPayload { get; init; }
    }
}
