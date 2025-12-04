namespace Project.Emailing.Templates;

/// <summary>
/// Email template for two-factor authentication OTP codes.
/// </summary>
public static class TwoFactorEmailTemplate
{
    public static string GenerateOtpEmail(string otpCode, int expirationMinutes)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .container {{
            background-color: #f9f9f9;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .header h1 {{
            color: #2c3e50;
            margin: 0;
        }}
        .otp-box {{
            background-color: #fff;
            border: 2px solid #3498db;
            border-radius: 8px;
            padding: 20px;
            text-align: center;
            margin: 30px 0;
        }}
        .otp-code {{
            font-size: 48px;
            font-weight: bold;
            color: #3498db;
            letter-spacing: 10px;
            margin: 10px 0;
        }}
        .expiration {{
            color: #e74c3c;
            font-weight: bold;
            margin-top: 10px;
        }}
        .warning {{
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            margin-top: 30px;
            color: #7f8c8d;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🔐 Two-Factor Authentication</h1>
        </div>
        
        <p>Hello,</p>
        
        <p>You have requested to log in to your account. Please use the following verification code to complete your login:</p>
        
        <div class=""otp-box"">
            <div>Your verification code is:</div>
            <div class=""otp-code"">{otpCode}</div>
            <div class=""expiration"">⏰ This code will expire in {expirationMinutes} minutes</div>
        </div>
        
        <div class=""warning"">
            <strong>⚠️ Security Notice:</strong><br>
            If you did not request this code, please ignore this email and ensure your account is secure. 
            Someone may be trying to access your account.
        </div>
        
        <p>For your security:</p>
        <ul>
            <li>Never share this code with anyone</li>
            <li>Our team will never ask for this code</li>
            <li>If you didn't request this, change your password immediately</li>
        </ul>
        
        <div class=""footer"">
            <p>This is an automated message, please do not reply to this email.</p>
            <p>&copy; {DateTime.UtcNow.Year} Your Company. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    public static string GenerateBackupCodesEmail(List<string> backupCodes)
    {
        var codesHtml = string.Join("", backupCodes.Select(code => 
            $"<div style='background: #ecf0f1; padding: 10px; margin: 5px 0; border-radius: 4px; font-family: monospace; font-size: 16px;'>{code}</div>"));

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .container {{
            background-color: #f9f9f9;
            border-radius: 10px;
            padding: 30px;
        }}
        .warning {{
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>🔑 Your Backup Codes</h1>
        
        <p>You have enabled two-factor authentication. Here are your backup codes:</p>
        
        <div class=""warning"">
            <strong>⚠️ Important:</strong><br>
            Save these codes in a secure location. Each code can only be used once.
            You can use these codes if you lose access to your email.
        </div>
        
        <div style='margin: 20px 0;'>
            {codesHtml}
        </div>
        
        <p><strong>How to use backup codes:</strong></p>
        <ul>
            <li>When prompted for your verification code, enter one of these backup codes instead</li>
            <li>Each code can only be used once</li>
            <li>You can generate new codes anytime from your account settings</li>
        </ul>
    </div>
</body>
</html>";
    }
}
