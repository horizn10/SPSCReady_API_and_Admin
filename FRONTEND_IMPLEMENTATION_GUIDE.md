# OTP Login Feature - Quick Reference for Frontend Team

## Feature Overview
Users can now log in using **Email + OTP** instead of password. This is a 2-step process:

### Step 1: Request OTP
User enters email → Backend sends 6-digit OTP to email

### Step 2: Verify OTP  
User enters OTP received in email → Backend verifies and returns JWT token for login

---

## API Endpoints Summary

### Endpoint 1: Send OTP

```
POST /api/account/send-otp
Content-Type: application/json

{
  "email": "user@example.com"
}
```

**Success Response:**
```json
{
  "success": true,
  "message": "OTP sent successfully to your email."
}
```

**Error Response:**
```json
{
  "success": false,
  "message": "Email is required." | "If an account with this email exists, an OTP will be sent." | "Failed to send OTP. Please try again."
}
```

---

### Endpoint 2: Verify OTP & Login

```
POST /api/account/verify-otp-login
Content-Type: application/json

{
  "email": "user@example.com",
  "code": "123456"
}
```

**Success Response:**
```json
{
  "isSuccessful": true,
  "message": "Login successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Response:**
```json
{
  "message": "Invalid or expired OTP." | "User not found." | "Email and OTP code are required."
}
```

---

## Implementation Checklist

- [ ] Create Login Screen with "Login with OTP" option
- [ ] Create OTP Request Screen (email input)
- [ ] Create OTP Verification Screen (6-digit input)
- [ ] Call POST `/api/account/send-otp` with email
- [ ] Show OTP input field after successful response
- [ ] Call POST `/api/account/verify-otp-login` with email and OTP
- [ ] Store returned JWT token securely
- [ ] Navigate to dashboard/home on success
- [ ] Show error messages on failure
- [ ] Add "Resend OTP" button (can be clicked again after getting new OTP)
- [ ] Add timer showing OTP expiration (10 minutes)

---

## Key Points for Implementation

1. **OTP Validity**: OTP is valid for **10 minutes only**
2. **OTP Format**: 6-digit numeric code (e.g., "123456")
3. **User Experience**: Show countdown timer for OTP expiration
4. **Error Handling**: Display clear error messages from API
5. **Security**: 
   - Store JWT token securely (use secure storage libraries)
   - Use HTTPS for all API calls
   - Don't show full email in OTP screen (mask with ***, e.g., us***@example.com)

---

## Example Flow

```
User Interface               Backend API
─────────────────────────────────────────────────
[Login Screen]
     ↓
  "Login with OTP"
     ↓
[OTP Request Screen]
     ├─ POST /api/account/send-otp
     └─→ [Backend sends OTP to email]
          ↓ Returns success/error
[OTP Verification Screen]
     ├─ Show countdown timer (10 mins)
     ├─ POST /api/account/verify-otp-login
     └─→ [Backend verifies OTP]
          ↓ Returns JWT token
[Dashboard/Home]
```

---

## Sample Code Templates

### Flutter

```dart
// 1. Request OTP
var response = await http.post(
  Uri.parse('$baseUrl/api/account/send-otp'),
  headers: {'Content-Type': 'application/json'},
  body: jsonEncode({'email': emailController.text}),
);

// 2. Verify OTP
var response = await http.post(
  Uri.parse('$baseUrl/api/account/verify-otp-login'),
  headers: {'Content-Type': 'application/json'},
  body: jsonEncode({
    'email': emailController.text,
    'code': otpController.text
  }),
);
```

### React/Web

```javascript
// 1. Request OTP
const response = await fetch(`${baseUrl}/api/account/send-otp`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email })
});

// 2. Verify OTP
const response = await fetch(`${baseUrl}/api/account/verify-otp-login`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, code: otp })
});

const data = await response.json();
if (data.token) {
  localStorage.setItem('authToken', data.token);
}
```

---

## Testing Endpoints in Postman

1. **Import Collection** or create new requests:
   - POST: `http://localhost:5000/api/account/send-otp`
   - POST: `http://localhost:5000/api/account/verify-otp-login`

2. **Test Send OTP**:
   - Use registered email
   - Check inbox for OTP email (might be in spam)

3. **Test Verify OTP**:
   - Copy OTP from email
   - Paste in request and verify

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Not receiving OTP email | Check spam folder, verify email is registered |
| "User not found" error | Email must be registered in system first |
| OTP expired | Wait 10 minutes and request new OTP |
| Login fails after OTP | Ensure email and OTP match exactly |
| "Invalid or expired OTP" | OTP might have expired or is incorrect |

---

## Notes for Your Team

- **Existing Login** still works: `POST /api/account/login` (email + password)
- **No password required** for OTP login
- OTP is **automatically cleaned up** after being used
- Previous OTPs for same email are **deleted when new OTP is requested**
- JWT token from both login methods is **identical and valid for 24 hours**

---

For detailed documentation, see: `OTP_LOGIN_DOCUMENTATION.md`
