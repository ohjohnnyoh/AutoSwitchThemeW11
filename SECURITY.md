# Security Policy

## Supported Versions

Use this section to tell people about which versions of your project are currently being supported with security updates.

| Version | Supported          |
| ------- | ------------------ |
| 1.5.x   | :white_check_mark: |
| 1.4.x   | :x:                |
| 1.3.x   | :x:                |
| < 1.3   | :x:                |

## Reporting a Vulnerability

We take security vulnerabilities seriously. If you discover a security vulnerability in Auto Switch Theme W11, please follow these steps:

### 🚨 How to Report

1. **DO NOT** create a public GitHub issue for security vulnerabilities
2. **DO** email us at: [INSERT SECURITY EMAIL]
3. **DO** provide detailed information about the vulnerability
4. **DO** include steps to reproduce the issue
5. **DO** specify the affected version(s)

### 📋 What to Include

When reporting a security vulnerability, please include:

- **Description**: Clear description of the vulnerability
- **Impact**: What could happen if exploited
- **Steps to Reproduce**: Detailed steps to reproduce the issue
- **Environment**: Windows version, .NET version, app version
- **Proof of Concept**: If possible, provide a safe PoC
- **Suggested Fix**: If you have ideas for fixing the issue

### ⏱️ Response Timeline

- **Initial Response**: Within 48 hours
- **Assessment**: Within 1 week
- **Fix Development**: Depends on complexity
- **Public Disclosure**: After fix is available

### 🔒 Security Considerations

This application modifies Windows system settings, so security is critical:

#### What the App Does (Safe Operations)
- Writes to user registry: `HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize`
- Sends safe system notifications: `WM_SETTINGCHANGE`, `WM_THEMECHANGED`, `WM_SYSCOLORCHANGE`
- Manages auto-start entries in user registry
- Creates log files in user AppData folder

#### What the App Does NOT Do (Security Boundaries)
- Never writes to system registry (HKLM)
- Never modifies system files
- Never runs with elevated privileges
- Never accesses user data outside of its own settings
- Never sends data over the network
- Never executes arbitrary code

### 🛡️ Security Best Practices

#### For Users
- Download only from official releases
- Verify file integrity using checksums
- Run with standard user privileges
- Keep Windows updated
- Use antivirus software

#### For Developers
- Never introduce code that requires elevated privileges
- Always validate user inputs
- Use safe Windows API calls only
- Avoid aggressive system modifications
- Test thoroughly before releasing

### 🔍 Security Audit

We regularly audit our code for:
- Input validation
- Registry access patterns
- Windows API usage
- Error handling
- Resource management

### 📝 Disclosure Policy

- Security vulnerabilities will be disclosed after fixes are available
- We will credit security researchers who responsibly report issues
- Public disclosure will include:
  - Description of the vulnerability
  - Impact assessment
  - Fix details
  - Timeline of events

### 🤝 Responsible Disclosure

We follow responsible disclosure practices:
- We will work with security researchers to understand and fix issues
- We will not take legal action against researchers who follow these guidelines
- We will publicly acknowledge security researchers who help improve our security

### 📞 Contact Information

For security-related issues:
- **Email**: [INSERT SECURITY EMAIL]
- **GitHub Security**: Use GitHub's security advisory feature
- **Response Time**: Within 48 hours

### 🏆 Security Hall of Fame

We acknowledge security researchers who help improve our application:

- [To be populated as issues are reported and fixed]

Thank you for helping keep Auto Switch Theme W11 secure! 🔒
