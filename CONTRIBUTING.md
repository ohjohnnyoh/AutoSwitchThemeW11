# Contributing to Auto Switch Theme W11

Thank you for your interest in contributing to Auto Switch Theme W11! This document provides guidelines and information for contributors.

## 🚀 Getting Started

### Prerequisites
- Windows 10/11 x64
- Visual Studio 2022 or later (Community edition is fine)
- .NET 8 SDK
- Git

### Development Setup
1. Fork the repository
2. Clone your fork locally
3. Open `AutoSwitchThemeW11.sln` in Visual Studio
4. Restore NuGet packages
5. Build the solution

## 🛠️ Development Guidelines

### Code Style
- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public methods
- Keep methods focused and concise
- Use proper exception handling

### Architecture Principles
- **Safety First**: Never compromise system stability for performance
- **Minimal Impact**: Keep system resource usage low
- **User-Friendly**: Provide clear error messages and logging
- **Maintainable**: Write clean, well-documented code

### Testing
- Test theme switching functionality thoroughly
- Verify multi-monitor support
- Test schedule functionality
- Ensure no Explorer.exe crashes occur
- Test on different Windows versions (10/11)

## 🐛 Bug Reports

When reporting bugs, please include:
- Windows version and build number
- Steps to reproduce the issue
- Expected vs actual behavior
- Log files from `%APPDATA%\AutoSwitchThemeW11\app.log`
- Monitor configuration details

## 💡 Feature Requests

When suggesting features:
- Describe the use case clearly
- Explain how it benefits users
- Consider implementation complexity
- Ensure it aligns with the app's purpose

## 🔧 Pull Request Process

1. **Create a feature branch** from `main`
2. **Make your changes** following the guidelines above
3. **Test thoroughly** on different configurations
4. **Update documentation** if needed
5. **Submit a pull request** with a clear description

### PR Guidelines
- Use descriptive commit messages
- Include screenshots for UI changes
- Update README.md if adding new features
- Ensure all tests pass
- Follow the existing code style

## 📋 Code Review Process

All contributions require review before merging:
- At least one maintainer must approve
- All CI checks must pass
- Code must follow project guidelines
- Security implications must be considered

## 🛡️ Security Considerations

Since this app modifies system settings:
- Never introduce code that could harm the system
- Always validate user inputs
- Use safe Windows API calls
- Avoid aggressive system modifications
- Test thoroughly before submitting

## 📝 Documentation

When contributing:
- Update README.md for new features
- Add inline code comments for complex logic
- Update CHANGELOG.md for significant changes
- Include usage examples where appropriate

## 🏷️ Versioning

We follow semantic versioning:
- **Major**: Breaking changes
- **Minor**: New features (backward compatible)
- **Patch**: Bug fixes and improvements

## 🤝 Community Guidelines

- Be respectful and constructive
- Help other contributors
- Follow the project's code of conduct
- Provide helpful feedback on issues and PRs

## 📞 Getting Help

If you need help:
- Check existing issues and discussions
- Create a new issue with clear details
- Join our community discussions
- Review the documentation

## 🎯 Areas for Contribution

We welcome contributions in these areas:
- **Bug fixes** - especially stability improvements
- **Performance optimizations** - while maintaining safety
- **UI/UX improvements** - tray menu enhancements
- **Documentation** - better guides and examples
- **Testing** - automated tests and manual testing
- **Localization** - additional language support

Thank you for contributing to Auto Switch Theme W11! 🎉
