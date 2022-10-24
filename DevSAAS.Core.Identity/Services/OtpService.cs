using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Stores;
using DevSAAS.Core.Notification.Services;
using DevSAAS.Core.Notification.Templates.VerificationCode;

namespace DevSAAS.Core.Identity.Services;

public class OtpService
{
    private readonly DatabaseFactory _databaseFactory;
    private readonly SmsService _smsService;
    private readonly MailService _mailService;

    public OtpService(DatabaseFactory databaseFactory, SmsService smsService, MailService mailService)
    {
        _databaseFactory = databaseFactory;
        _smsService = smsService;
        _mailService = mailService;
    }

    public async Task<bool> Verify(string userId, string code)
    {
        await using var conn = _databaseFactory.Instance();
        var otpStore = new OtpStore(conn);
        var otp = await otpStore.GetByUserIdCodeAsync(userId, code);

        if (otp == null || otp.Active == 0) return false;

        //Update OTP
        otp.Active = 0;
        otp.UpdatedAt = DateTime.UtcNow;

        var changes = await otpStore.UpdateAsync(otp, "Active", "UpdatedAt");
        if (changes <= 0) return false;

        //Update User
        var userStore = new UserStore(conn);
        var user = await userStore.GetAsync(userId);
        if (user == null) return false;

        user.Active = 1;
        user.PhoneVerifiedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        changes = await userStore.UpdateAsync(user, "Active", "PhoneVerifiedAt", "UpdatedAt");
        return changes > 0;
    }

    public async Task<string> Generate(string userId)
    {
        await using var conn = _databaseFactory.Instance();

        var otpStore = new OtpStore(conn);
        var otp = await otpStore.GenerateCode(userId);

        return otp;
    }

    public async Task<bool> Resend(string userId, string phone)
    {
        await using var conn = _databaseFactory.Instance();

        var userStore = new UserStore(conn);
        var user = await userStore.GetAsync(userId);
        if (user == null) return false;

        if (await userStore.PhoneExistsAsync(phone, userId))
        {
            throw new ApplicationException("PhoneAlreadyExists");
        }

        // Send OTP
        var otpStore = new OtpStore(conn);
        var otp = await otpStore.GenerateCode(user.Id);
        new VerificationCode(_smsService, null, otp, user.Phone, user.Email).Send();

        return true;
    }
}