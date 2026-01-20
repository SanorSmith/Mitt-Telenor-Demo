# Contributing to Mitt Telenor Demo

Thank you for your interest in contributing to this project!

## 📋 Code of Conduct

This project is a portfolio demonstration for the Telenor Front-end Developer position. While contributions are welcome, please note this is primarily a showcase project.

## 🚀 Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/yourusername/mitt-telenor-demo.git`
3. Create a feature branch: `git checkout -b feature/amazing-feature`
4. Make your changes
5. Commit using conventional commits: `git commit -m 'feat: add amazing feature'`
6. Push to your fork: `git push origin feature/amazing-feature`
7. Open a Pull Request

## 💻 Development Guidelines

### Code Style

**Frontend (Vue/TypeScript)**
- Use TypeScript strict mode (no `any` types)
- Follow Vue 3 Composition API best practices
- Use `<script setup>` syntax
- Format with Prettier (run `pnpm format`)
- Lint with ESLint (run `pnpm lint`)

**Backend (C#/.NET)**
- Follow C# coding conventions
- Use async/await for I/O operations
- Add XML documentation comments
- Write unit tests for new features

### Commit Messages

Follow [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `style:` - Code style changes (formatting)
- `refactor:` - Code refactoring
- `test:` - Adding or updating tests
- `chore:` - Maintenance tasks

Examples:
```
feat: add usage chart component
fix: resolve token refresh issue
docs: update setup guide
test: add auth service unit tests
```

### Testing

- Write unit tests for new features
- Ensure all tests pass before submitting PR
- Aim for 80%+ code coverage

**Frontend:**
```bash
pnpm test:unit
pnpm test:e2e
```

**Backend:**
```bash
dotnet test
```

### Pull Request Process

1. Update README.md with details of changes if needed
2. Update PROJECT_STATUS.md to reflect completion
3. Ensure all tests pass
4. Request review from maintainers
5. Address any feedback
6. Squash commits if requested

## 🐛 Bug Reports

When filing a bug report, please include:

- Clear description of the issue
- Steps to reproduce
- Expected behavior
- Actual behavior
- Screenshots (if applicable)
- Environment details (OS, browser, Node version, etc.)

## 💡 Feature Requests

Feature requests are welcome! Please:

- Check if the feature already exists
- Clearly describe the feature and its benefits
- Provide use cases
- Consider implementation complexity

## 📝 Documentation

- Update documentation for any user-facing changes
- Add JSDoc/XML comments for new functions
- Update API documentation if endpoints change

## ✅ Checklist Before Submitting PR

- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] Tests added/updated
- [ ] All tests passing
- [ ] No console errors or warnings
- [ ] Accessibility tested (WCAG 2.1 AA)
- [ ] Mobile responsive (if UI changes)

## 🙏 Thank You!

Your contributions help make this project better. Thank you for taking the time to contribute!

## 📧 Contact

For questions or discussions:
- Open an issue
- Email: your.email@example.com

---

**Note:** This is a portfolio project. Contributions are appreciated but may be evaluated based on alignment with the project's demonstration goals.
