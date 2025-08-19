# Next.js to .NET Core API Authentication Security Guide

## Overview
This guide outlines the security requirements and implementation standards for securing user authentication between a Next.js frontend and .NET Core API backend.

## ⚠️ Critical Security Requirements

### 1. HTTPS/TLS (MANDATORY)
- **Production**: All communication MUST use HTTPS
- **Development**: Use HTTPS for testing auth flows
- **Certificate**: Use valid SSL/TLS certificates (Let's Encrypt for free options)

### 2. Password Security (Client to Server)
- **HTTPS is MANDATORY**: Plain text passwords from client are only secure when transmitted over HTTPS/TLS
- **Never log passwords**: Ensure passwords are never logged in client or server logs
- **Immediate hashing**: Hash passwords immediately upon receipt at the API level
- **No client-side storage**: Never store plain text passwords in browser storage
- **Secure transmission**: Use HTTPS to encrypt credentials during transmission from Next.js to .NET Core API

### 3. Environment Configuration
- Never commit secrets to version control
- Use environment variables for all sensitive data
- Separate configurations for development, staging, and production
- Store JWT secrets, database connections, and API URLs as environment variables

---

## Frontend Implementation Requirements (Next.js)

### Authentication Flow
1. Create authentication utility functions for login, logout, and token management
2. Implement secure token storage (consider httpOnly cookies vs localStorage trade-offs)
3. Build login form with proper validation and error handling
4. Create API client with automatic token attachment and refresh logic
5. Implement route protection for authenticated pages

### Key Security Measures
- **Plain Text Password Handling**: Passwords sent from Next.js client will be in plain text - this is only secure over HTTPS
- **Input Validation**: Validate all user inputs on client side (with server-side validation as primary)
- **Token Management**: Implement token expiration checks and automatic refresh
- **Error Handling**: Don't expose sensitive information in error messages
- **CORS**: Configure requests to include credentials when necessary
- **Auto-logout**: Redirect to login on token expiration or unauthorized responses
- **No Password Storage**: Never store passwords in localStorage, sessionStorage, or any client-side storage

---

## Backend Implementation Requirements (.NET Core)

### Authentication Controller
1. Create login endpoint with rate limiting (5 attempts per minute)
2. Implement input validation using data annotations
3. Add proper logging (without sensitive data)
4. Generate JWT tokens with appropriate claims and expiration
5. Implement refresh token mechanism for extended sessions
6. Create logout endpoint to invalidate tokens

### Security Services
1. **Token Service**: Generate, validate, and manage JWT tokens
2. **User Service**: Handle user authentication and password verification
3. **Password Handling**: Receive plain text passwords from client and immediately hash using bcrypt or similar
4. **Rate Limiting**: Implement on authentication endpoints
5. **Logging**: Track login attempts and failures (without passwords)
6. **Input Sanitization**: Validate and sanitize all incoming authentication data

### Configuration Requirements
1. **JWT Configuration**: Strong secret keys (minimum 32 characters), proper issuer/audience
2. **CORS**: Restrictive policy allowing only specific origins
3. **Database**: Secure connection strings and parameterized queries
4. **Security Headers**: Implement security headers middleware
5. **HTTPS Redirection**: Force HTTPS in production

---

## Security Checklist

### ✅ Production Deployment Checklist

**HTTPS/TLS**
- [ ] Valid SSL certificate installed
- [ ] HTTPS redirect configured
- [ ] HSTS headers enabled
- [ ] Certificate auto-renewal setup

**Environment Variables**
- [ ] All secrets moved to environment variables
- [ ] No hardcoded credentials in code
- [ ] Production secrets differ from development
- [ ] Database connection strings secured

**API Security**
- [ ] CORS properly configured with specific origins
- [ ] Rate limiting implemented on auth endpoints
- [ ] Input validation on all endpoints
- [ ] Error handling doesn't expose sensitive info
- [ ] Logging configured (without sensitive data)

**Authentication**
- [ ] Strong JWT secret key (minimum 32 characters)
- [ ] Token expiration properly set
- [ ] Refresh token mechanism implemented
- [ ] Password hashing with strong algorithm (bcrypt)
- [ ] Account lockout after failed attempts

**Database**
- [ ] Passwords stored as hashes, never plain text
- [ ] Database connection encrypted
- [ ] Parameterized queries to prevent SQL injection
- [ ] Regular security updates applied

### 🚨 Common Security Mistakes to Avoid

1. **Never store passwords in plain text** (in database or logs)
2. **Don't expose sensitive data in client-side code**
3. **Don't use weak JWT secrets**
4. **Don't skip HTTPS in production** (critical for plain text password transmission)
5. **Don't trust client-side validation alone**
6. **Don't log sensitive information** (especially passwords)
7. **Don't use default/weak CORS policies**
8. **Don't store passwords in browser storage** (localStorage, sessionStorage)
9. **Don't send passwords over HTTP** (only HTTPS)

### 📋 Testing Security

**Manual Testing**
- [ ] Test login with valid credentials
- [ ] Test login with invalid credentials
- [ ] Test rate limiting by rapid login attempts
- [ ] Test token expiration
- [ ] Test CORS policies
- [ ] Test HTTPS redirect

**Automated Testing**
- [ ] Unit tests for authentication logic
- [ ] Integration tests for auth endpoints
- [ ] Security scanning tools
- [ ] Dependency vulnerability scanning

---

## Implementation Guidelines

### Development Standards
- Follow OWASP security guidelines
- Implement defense in depth strategy
- Use established authentication libraries where possible
- Regular security reviews and updates
- Document all security decisions and configurations

### Code Review Requirements
- All authentication-related code must be peer reviewed
- Security checklist must be completed before deployment
- Penetration testing recommended for production systems
- Regular dependency updates and vulnerability scanning

### Monitoring and Maintenance
- Monitor failed login attempts and unusual patterns
- Regular rotation of JWT secrets and certificates
- Keep security dependencies updated
- Implement alerting for security events

---

## Support and Troubleshooting

### Common Issues

**CORS Errors**
- Verify allowed origins in .NET Core configuration
- Check if credentials are included in fetch requests
- Ensure preflight OPTIONS requests are handled

**Token Issues**
- Verify JWT secret key matches between environments
- Check token expiration times
- Validate token format and claims

**HTTPS Issues**
- Ensure certificates are valid and not expired
- Check certificate chain completeness
- Verify HTTPS redirect is working

### Get more details (Resources)

- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [.NET Core Security Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [Next.js Security Headers](https://nextjs.org/docs/advanced-features/security-headers)

---

**Document Version:** 1.0  
**Last Updated:** 20 August 2025 