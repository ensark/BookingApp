namespace Booking.Core.Services.Constants
{
    public class Config
    {
        public const string JWT_SECRET_API_KEY = "JWT:APISecretKey";
        public const string JWT_ACCESS_TOKEN_EXPIRE = "JWT:AccessTokenExpiresInMinutes";
        public const string FILE_UPLOAD_SIZE = "FileUpload:MaxFileSize";       
        public const string VOUCHER_CODE_DISCOUNT = "PaymentDiscountsPercentages:VoucherCodeDiscount";
    }
}
