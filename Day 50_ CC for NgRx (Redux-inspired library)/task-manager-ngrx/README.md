# NgRx Task Manager 📋

A modern task management application built with **Angular 18** and **NgRx** for efficient state management. This project demonstrates best practices for implementing Redux-style state management in Angular applications with local storage persistence.

![Angular](https://img.shields.io/badge/Angular-18.0-red?logo=angular)
![NgRx](https://img.shields.io/badge/NgRx-18.0-purple?logo=ngrx)
![TypeScript](https://img.shields.io/badge/TypeScript-5.4-blue?logo=typescript)
![SCSS](https://img.shields.io/badge/SCSS-Styles-pink?logo=sass)

## 🚀 Features

### Core Functionality
- ✅ **Add New Tasks**: Create tasks with title and description
- ✅ **View Task List**: Display all tasks with filtering options
- ✅ **Mark as Completed**: Toggle task completion status
- ✅ **Delete Tasks**: Remove unwanted tasks
- ✅ **Local Storage Persistence**: Tasks persist across browser sessions
- ✅ **Real-time State Updates**: Immediate UI updates through NgRx

### Advanced Features
- 🎯 **Task Filtering**: View all, pending, or completed tasks
- 📊 **Task Statistics**: Real-time count of total, pending, and completed tasks
- 🎨 **Modern UI**: Clean, responsive design with smooth animations
- 📱 **Mobile Responsive**: Works perfectly on all device sizes
- ♿ **Accessibility**: ARIA compliant with keyboard navigation
- 🎭 **Loading States**: Visual feedback during operations
- ❌ **Error Handling**: Graceful error management with user feedback

## 🏗️ Architecture Overview

This application follows NgRx best practices with a feature-based architecture:

### State Management Structure
```
src/app/state/tasks/
├── task.actions.ts     # Action definitions
├── task.reducer.ts     # State reducers
├── task.selectors.ts   # State selectors
└── task.effects.ts     # Side effects management
```

### Key NgRx Concepts Implemented
- **Store**: Centralized application state
- **Actions**: Dispatched to trigger state changes
- **Reducers**: Pure functions that update state
- **Selectors**: Derive data from the store
- **Effects**: Handle side effects (localStorage operations)

## 🛠️ Technology Stack

- **Frontend Framework**: Angular 18 (Standalone Components)
- **State Management**: NgRx Store, Effects, DevTools
- **Styling**: SCSS with modern CSS Grid/Flexbox
- **Type Safety**: TypeScript with strict mode
- **Build Tool**: Angular CLI
- **Local Storage**: Native Web Storage API

## 📦 Installation & Setup

### Prerequisites
- Node.js (v18 or higher)
- npm or yarn package manager
- Angular CLI (optional but recommended)

### Step-by-Step Installation

1. **Extract the project files**
   ```bash
   unzip task-manager-ngrx.zip
   cd task-manager-ngrx
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```
   Or with yarn:
   ```bash
   yarn install
   ```

3. **Start the development server**
   ```bash
   npm start
   ```
   Or:
   ```bash
   ng serve
   ```

4. **Open the application**
   Navigate to `http://localhost:4200` in your browser

### Build for Production

```bash
npm run build
```
The build artifacts will be stored in the `dist/` directory.

## 🎯 Usage Guide

### Adding a New Task
1. Fill in the **Task Title** (3-100 characters)
2. Enter a **Task Description** (5-500 characters)
3. Click **"Add Task"** button
4. The task appears immediately in the task list

### Managing Tasks
- **Mark as Completed**: Click the ✓ button next to any task
- **Undo Completion**: Click the ↺ button on completed tasks
- **Delete Task**: Click the 🗑️ button (with confirmation)
- **Filter Tasks**: Use the filter tabs (All/Pending/Completed)

### Data Persistence
- Tasks are automatically saved to localStorage
- Data persists across browser sessions and page refreshes
- Use "Clear All Tasks" to reset the application state

## 🏗️ Project Structure

```
task-manager-ngrx/
├── src/
│   ├── app/
│   │   ├── components/           # Reusable components
│   │   │   ├── task-form/       # Task creation form
│   │   │   └── task-list/       # Task display and management
│   │   ├── models/              # TypeScript interfaces
│   │   │   ├── task.model.ts    # Task and TaskState interfaces
│   │   │   └── app-state.model.ts # Application state interface
│   │   ├── services/            # Injectable services
│   │   │   └── local-storage.service.ts # localStorage operations
│   │   ├── state/               # NgRx state management
│   │   │   └── tasks/           # Task feature state
│   │   │       ├── task.actions.ts    # Action definitions
│   │   │       ├── task.reducer.ts    # State reducers
│   │   │       ├── task.selectors.ts  # State selectors
│   │   │       └── task.effects.ts    # Side effects
│   │   ├── app.component.*      # Root component
│   │   └── app.config.ts        # Application configuration
│   ├── styles/                  # Global styles
│   ├── assets/                  # Static assets
│   └── index.html              # Main HTML file
├── angular.json                 # Angular CLI configuration
├── package.json                # Dependencies and scripts
├── tsconfig.json               # TypeScript configuration
└── README.md                   # This file
```

## 🧪 Testing with Redux DevTools

1. **Install Redux DevTools Extension**
   - [Chrome Extension](https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd)
   - [Firefox Extension](https://addons.mozilla.org/en-US/firefox/addon/reduxdevtools/)

2. **Open DevTools**
   - Press F12 to open browser DevTools
   - Navigate to the "Redux" tab

3. **Monitor State Changes**
   - See all dispatched actions
   - Inspect state at any point in time
   - Time-travel debugging capabilities

## 🎨 Styling & Theming

The application uses modern SCSS with:
- **CSS Custom Properties** for easy theming
- **Flexbox & Grid** for responsive layouts
- **Smooth Animations** for better UX
- **Mobile-First** responsive design
- **Accessibility** features built-in

### Key Style Features
- Gradient background with backdrop blur effects
- Card-based design for tasks
- Smooth hover and focus animations
- Loading spinners and state indicators
- Error message styling
- Mobile-optimized touch targets

## 🔧 Development

### Available Scripts

- `npm start` - Start development server
- `npm run build` - Build for production
- `npm run watch` - Build with file watching
- `npm test` - Run unit tests (when configured)

### Code Quality
- **TypeScript Strict Mode** enabled
- **ESLint** rules for code consistency
- **Prettier** for code formatting
- **Angular Style Guide** compliance

## 📊 Performance Optimizations

- **OnPush Change Detection** where applicable
- **TrackBy Functions** for efficient list rendering
- **Lazy Loading** ready architecture
- **Tree Shaking** enabled for smaller bundles
- **Ahead-of-Time (AOT) Compilation**

## 🔒 Best Practices Implemented

### NgRx Best Practices
- ✅ Feature-based state organization
- ✅ Action creators with type safety
- ✅ Selectors for derived state
- ✅ Effects for side effect management
- ✅ Immutable state updates
- ✅ Error handling in effects

### Angular Best Practices
- ✅ Standalone components
- ✅ Reactive forms with validation
- ✅ OnPush change detection strategy
- ✅ Proper lifecycle management
- ✅ Separation of concerns
- ✅ Type-safe development

## 🐛 Known Limitations

1. **Single User**: No user authentication system
2. **Local Storage Only**: No server-side persistence
3. **Basic Validation**: Simple client-side validation only
4. **No Task Categories**: Tasks are not categorized
5. **No Due Dates**: No deadline management

## 🚀 Future Enhancements

- [ ] Task categories and tags
- [ ] Due dates with calendar integration
- [ ] Task priority levels
- [ ] Search and advanced filtering
- [ ] Drag-and-drop task reordering
- [ ] Task sharing and collaboration
- [ ] Dark mode toggle
- [ ] Export to PDF/CSV
- [ ] Offline support with Service Worker
- [ ] Server-side integration

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

If you encounter any issues or have questions:

1. Check the [troubleshooting section](#troubleshooting)
2. Search through existing GitHub issues
3. Create a new issue with detailed description

## 🙏 Acknowledgments

- Angular Team for the amazing framework
- NgRx Team for powerful state management
- Material Design for UI inspiration
- The open-source community for continuous support

---

**Built with ❤️ using Angular 18 & NgRx**

---

## Troubleshooting

### Common Issues

**1. Application won't start**
```bash
# Clear node modules and reinstall
rm -rf node_modules
npm install
npm start
```

**2. Redux DevTools not working**
- Ensure the browser extension is installed and enabled
- Check that the application is running in development mode

**3. Tasks not persisting**
- Check if localStorage is enabled in your browser
- Verify browser storage quota hasn't been exceeded

**4. TypeScript compilation errors**
```bash
# Clear TypeScript cache
rm -rf dist/
npm run build
```

For more specific issues, please check the browser console for detailed error messages.