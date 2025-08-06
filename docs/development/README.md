# Development Guide

Welcome to the AutoPBI development documentation! This section provides comprehensive information for developers who want to understand, maintain, or extend the AutoPBI application.

## 📚 Development Documentation Structure

### Getting Started
- **[Tech Stack](tech-stack.md)** - Overview of technologies, frameworks, and architecture
- **[Project Structure](project-structure.md)** - Detailed breakdown of folders and files
- **[Development Setup](development-setup.md)** - How to set up the development environment

### Architecture & Patterns
- **[MVVM Architecture](mvvm-architecture.md)** - Understanding the Model-View-ViewModel pattern
- **[UI Components](ui-components.md)** - Custom controls, styles, and UI patterns
- **[Popup System](popup-system.md)** - How to add new popups and dialogs
- **[Services & Utilities](services-utilities.md)** - Backend services and utility classes

### Contributing Guidelines
- **[Adding New Features](adding-features.md)** - Step-by-step guide for adding new functionality
- **[Code Style & Conventions](code-style.md)** - Coding standards and best practices
- **[Testing Guidelines](testing-guidelines.md)** - How to test your changes

## 🚀 Quick Start for New Developers

1. **Read the Tech Stack** - Understand the technologies used
2. **Set up Development Environment** - Follow the setup guide
3. **Explore Project Structure** - Familiarize yourself with the codebase
4. **Study MVVM Architecture** - Understand the application patterns
5. **Review UI Components** - Learn about custom controls and styling

## 🔧 Development Workflow

### Adding New Features
1. Create new ViewModel in `ViewModels/Popups/` (if popup)
2. Create new View in `Views/Popups/` (if popup)
3. Add to `MainViewModel.cs` and `MainView.axaml`
4. Follow the patterns established in existing features

### Code Organization
- **Models** - Data structures and entities
- **ViewModels** - Business logic and state management
- **Views** - UI definitions and user interactions
- **Services** - Backend operations and utilities
- **Controls** - Reusable UI components

## 📖 Key Concepts

- **Avalonia UI** - Cross-platform UI framework
- **MVVM Pattern** - Separation of concerns
- **PowerShell Integration** - Power BI operations
- **Custom Controls** - Reusable UI components
- **Popup System** - Modal dialogs and overlays

---

**For questions or issues, refer to the specific documentation sections or create an issue in the repository.** 