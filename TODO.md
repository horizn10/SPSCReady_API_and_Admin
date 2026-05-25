# OTP login fix - tasks

- [ ] Add server-side logging and richer error details for SendOtp endpoint (capture SMTP exception and configuration issues)
- [ ] Fix OTP code generation to avoid non-cryptographic Random (use RandomNumberGenerator)
- [ ] Improve input validation/ModelState handling for OTP endpoints
- [ ] Ensure Swagger response includes consistent JSON shape
- [ ] Rebuild and run dotnet to verify endpoints
- [ ] (Optional) Evaluate moving OTP cache to DB for multi-instance reliability

