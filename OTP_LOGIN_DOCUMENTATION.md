# OTP Login Feature - API Documentation

## Overview
This document provides complete information about the OTP (One-Time Password) login feature implemented in the SPSCReady API backend.

## Feature Description
The OTP login feature allows users to log in using their email address instead of a password. The system sends a 6-digit OTP to the user's email, which they must verify to log in.

### Key Features:
- **Two-step authentication**: First, request OTP → Then, verify OTP
- **Email-based delivery**: OTP is sent to the registered email address
- **Time-limited validity**: OTP expires after 10 minutes
- **Security**: Single-use OTP tokens that cannot be reused
- **JWT Token Generation**: Returns JWT token for authenticated API requests

---

## Email Configuration

### Setup Instructions (Gmail Example)

#### Step 1: Enable 2-Factor Authentication on Gmail
1. Go to [myaccount.google.com/security](https://myaccount.google.com/security)
2. Enable 2-Step Verification if not already enabled

#### Step 2: Generate App Password
1. Go to [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords)
2. Select "Mail" and "Windows Computer"
3. Google will generate a 16-character password
4. Copy this password

#### Step 3: Update Configuration
Update `appsettings.json` with your credentials:

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": "587",
  "SenderEmail": "your-email@gmail.com",
  "SenderPassword": "your-app-password"
}
```

### For Other Email Providers:

**SendGrid:**
```json
"EmailSettings": {
  "SmtpServer": "smtp.sendgrid.net",
  "SmtpPort": "587",
  "SenderEmail": "apikey",
  "SenderPassword": "SG.YOUR_SENDGRID_API_KEY"
}
```

**Office 365/Outlook:**
```json
"EmailSettings": {
  "SmtpServer": "smtp.office365.com",
  "SmtpPort": "587",
  "SenderEmail": "your-email@company.onmicrosoft.com",
  "SenderPassword": "your-password"
}
```

---

## API Endpoints

### 1. Send OTP (Step 1)

**Endpoint:** `POST /api/account/send-otp`

**Description:** Request an OTP to be sent to the provided email address.

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "OTP sent successfully to your email."
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Email is required."
}
```

**Error Response (400 Bad Request - User not found):**
```json
{
  "success": false,
  "message": "If an account with this email exists, an OTP will be sent."
}
```

**Error Response (400 Bad Request - Email send failed):**
```json
{
  "success": false,
  "message": "Failed to send OTP. Please try again."
}
```

---

### 2. Verify OTP & Login (Step 2)

**Endpoint:** `POST /api/account/verify-otp-login`

**Description:** Verify the OTP sent to the user's email and return JWT authentication token.

**Request Body:**
```json
{
  "email": "user@example.com",
  "code": "123456"
}
```

**Success Response (200 OK):**
```json
{
  "isSuccessful": true,
  "message": "Login successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Response (401 Unauthorized - Invalid OTP):**
```json
{
  "message": "Invalid or expired OTP."
}
```

**Error Response (401 Unauthorized - User not found):**
```json
{
  "message": "User not found."
}
```

**Error Response (400 Bad Request - Missing fields):**
```json
{
  "message": "Email and OTP code are required."
}
```

---

## Security Considerations

1. **OTP Expiration**: OTPs expire after 10 minutes
2. **Single Use**: Each OTP can only be used once
3. **Automatic Cleanup**: Previous OTPs for the same email are deleted when a new OTP is requested
4. **JWT Token**: The returned JWT token is valid for 24 hours
5. **Email Validation**: The system checks if the email is registered before sending OTP
6. **HTTPS Only**: All API calls should be made over HTTPS in production

---

## Frontend Implementation Guide

### Flutter/Mobile Example

```dart
// Step 1: Request OTP
Future<void> requestOtp(String email) async {
  final response = await http.post(
    Uri.parse('https://yourapi.com/api/account/send-otp'),
    headers: {'Content-Type': 'application/json'},
    body: jsonEncode({'email': email}),
  );

  if (response.statusCode == 200) {
    final data = jsonDecode(response.body);
    print('OTP sent: ${data['message']}');
    // Navigate to OTP verification screen
  } else {
    print('Error: ${jsonDecode(response.body)['message']}');
  }
}

// Step 2: Verify OTP and Login
Future<String?> verifyOtp(String email, String otp) async {
  final response = await http.post(
    Uri.parse('https://yourapi.com/api/account/verify-otp-login'),
    headers: {'Content-Type': 'application/json'},
    body: jsonEncode({'email': email, 'code': otp}),
  );

  if (response.statusCode == 200) {
    final data = jsonDecode(response.body);
    final token = data['token'];
    // Store token securely (e.g., in flutter_secure_storage)
    print('Login successful! Token: $token');
    return token;
  } else {
    print('Login failed: ${jsonDecode(response.body)['message']}');
    return null;
  }
}
```

### JavaScript/Web Example

```javascript
// Step 1: Request OTP
async function requestOtp(email) {
  try {
    const response = await fetch('https://yourapi.com/api/account/send-otp', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email }),
    });

    const data = await response.json();
    if (response.ok) {
      console.log('OTP sent:', data.message);
      // Navigate to OTP verification
      return true;
    } else {
      console.error('Error:', data.message);
      return false;
    }
  } catch (error) {
    console.error('Network error:', error);
    return false;
  }
}

// Step 2: Verify OTP and Login
async function verifyOtp(email, otp) {
  try {
    const response = await fetch('https://yourapi.com/api/account/verify-otp-login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, code: otp }),
    });

    const data = await response.json();
    if (response.ok) {
      console.log('Login successful!');
      localStorage.setItem('authToken', data.token);
      return data.token;
    } else {
      console.error('Login failed:', data.message);
      return null;
    }
  } catch (error) {
    console.error('Network error:', error);
    return null;
  }
}
```

---

## Database Schema

### OtpToken Table Structure

```sql
CREATE TABLE OtpTokens (
  Id INT PRIMARY KEY IDENTITY(1,1),
  Email NVARCHAR(256) NOT NULL,
  Code NVARCHAR(6) NOT NULL,
  ExpiresAt DATETIME NOT NULL,
  IsUsed BIT NOT NULL DEFAULT 0,
  CreatedAt DATETIME NOT NULL
);
```

---

## Testing with Postman/REST Client

### 1. Send OTP Request

**Method:** POST  
**URL:** `http://localhost:5000/api/account/send-otp`  
**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "email": "test@example.com"
}
```

### 2. Verify OTP Request

**Method:** POST  
**URL:** `http://localhost:5000/api/account/verify-otp-login`  
**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "email": "test@example.com",
  "code": "123456"
}
```

---

## Files Created/Modified

### New Files:
1. `SPSCReady.Domain/Entities/OtpToken.cs` - OTP entity model
2. `SPSCReady.Application/DTOs/OtpRequestDto.cs` - Request DTO for OTP
3. `SPSCReady.Application/DTOs/OtpVerifyDto.cs` - DTO for OTP verification
4. `SPSCReady.Application/Interfaces/IEmailService.cs` - Email service interface
5. `SPSCReady.Infrastructure/Services/EmailService.cs` - Email implementation

### Modified Files:
1. `SPSCReady.Application/Interfaces/IaccountServices.cs` - Added OTP methods
2. `SPSCReady.Infrastructure/Services/AccountServices.cs` - Implemented OTP logic
3. `SPSCReady.Infrastructure/Data/ApplicationDbContext.cs` - Added OtpTokens DbSet
4. `SPSCReady.API/Controllers/AccountController.cs` - Added new endpoints
5. `SPSCReady.API/Program.cs` - Registered EmailService
6. `SPSCReady.API/appsettings.json` - Added EmailSettings configuration
7. `SPSCReady.API/appsettings.Development.json` - Added EmailSettings for development

---

## Migration Steps

After implementing this feature:

1. **Add the OTP migration:**
   ```bash
   cd SPSCReady.API
   dotnet ef migrations add AddOtpToken
   dotnet ef database update
   ```

2. **Update Email Configuration** in `appsettings.json` with your email service credentials

3. **Test the endpoints** using Postman or your frontend

4. **Deploy** the updated code to production

---

## Error Handling & Best Practices

### Frontend Error Handling
```javascript
async function handleOtpLogin(email) {
  try {
    // Request OTP
    const otpResponse = await requestOtp(email);
    if (!otpResponse) {
      showError('Failed to send OTP. Please check your email.');
      return;
    }

    // Get OTP from user
    const otp = await getUserInput('Enter the 6-digit OTP');

    // Verify OTP
    const token = await verifyOtp(email, otp);
    if (!token) {
      showError('Invalid OTP. Please try again.');
      return;
    }

    // Success
    navigateToDashboard(token);
  } catch (error) {
    showError('An unexpected error occurred.');
  }
}
```

### Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| "SMTP connection failed" | Verify SMTP server, port, and credentials in appsettings.json |
| "Invalid or expired OTP" | OTP expires after 10 minutes. Request a new one. |
| "Failed to send OTP" | Check email configuration and ensure firewall allows SMTP |
| "User not found" | Email must be registered before requesting OTP |

---

## Future Enhancements

- Add SMS OTP support
- Implement resend OTP rate limiting
- Add OTP attempt counter to prevent brute force
- Send audit logs for OTP requests
- Support for backup codes
- Integration with multi-factor authentication (MFA)

---

## Support & Questions

For implementation support or questions about the OTP feature, please contact the development team.
