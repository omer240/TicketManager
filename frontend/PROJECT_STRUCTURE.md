# Frontend Project Structure

```
frontend/
│
├── 📄 angular.json                      # Angular workspace config (✏️ updated with proxy)
├── 📄 package.json                      # NPM dependencies
├── 📄 proxy.conf.json                   # 🆕 Dev server proxy config
├── 📄 FRONTEND_README.md                # 🆕 Complete documentation
├── 📄 QUICK_START.md                    # 🆕 Quick setup guide
│
├── src/
│   ├── 📄 index.html                    # Main HTML
│   ├── 📄 main.ts                       # Bootstrap
│   ├── 📄 styles.scss                   # Global styles
│   │
│   ├── environments/                    # 🆕 Environment configs
│   │   ├── 📄 environment.ts           # Dev config (apiBaseUrl)
│   │   └── 📄 environment.prod.ts      # Prod config
│   │
│   └── app/
│       ├── 📄 app.ts                    # Root component
│       ├── 📄 app.html                  # Root template
│       ├── 📄 app.scss                  # Root styles
│       ├── 📄 app.config.ts             # ✏️ Updated (HttpClient + interceptor)
│       ├── 📄 app.routes.ts             # ✏️ Updated (auth + tickets routes)
│       │
│       ├── core/                        # 🆕 Core services & infrastructure
│       │   │
│       │   ├── guards/                  # 🆕 Route guards
│       │   │   └── 📄 auth.guard.ts    # Protect /tickets routes
│       │   │
│       │   ├── interceptors/            # 🆕 HTTP interceptors
│       │   │   └── 📄 jwt.interceptor.ts # Auto-attach JWT token
│       │   │
│       │   ├── models/                  # 🆕 TypeScript interfaces
│       │   │   └── 📄 auth.models.ts   # Auth DTOs (LoginRequest, AuthResponse, etc.)
│       │   │
│       │   └── services/                # 🆕 Core services
│       │       └── 📄 auth.service.ts  # Authentication logic
│       │
│       └── features/                    # 🆕 Feature modules
│           │
│           ├── auth/                    # 🆕 Authentication feature
│           │   │
│           │   ├── login/               # 🆕 Login page
│           │   │   ├── 📄 login.component.ts
│           │   │   ├── 📄 login.component.html
│           │   │   └── 📄 login.component.scss
│           │   │
│           │   └── register/            # 🆕 Register page
│           │       ├── 📄 register.component.ts
│           │       ├── 📄 register.component.html
│           │       └── 📄 register.component.scss
│           │
│           └── tickets/                 # 🆕 Tickets feature (placeholder)
│               ├── 📄 tickets.component.ts
│               └── ticket-list/
│                   └── 📄 ticket-list.component.ts
```

## Legend
- 🆕 = New file created
- ✏️ = Existing file modified
- 📄 = File

## Key Directories

### `/core` - Application Core
Contains singleton services, guards, interceptors, and shared models.
- **Never feature-specific**
- **Used across entire application**

### `/features` - Feature Modules
Contains feature-specific components organized by domain.
- `/auth` - Authentication pages
- `/tickets` - Ticket management (ready for implementation)

### `/environments` - Configuration
Environment-specific settings (API URLs, feature flags, etc.)

## Import Patterns

### Core Services
```typescript
import { AuthService } from '@app/core/services/auth.service';
```

### Guards
```typescript
import { authGuard } from '@app/core/guards/auth.guard';
```

### Models
```typescript
import { LoginRequest, AuthResponse } from '@app/core/models/auth.models';
```

## Route Structure

```
/                          → Redirect to /tickets
├── /auth
│   ├── /login            → Login page (public)
│   └── /register         → Register page (public)
└── /tickets              → Protected by authGuard
    └── (empty path)      → Ticket list
```

## Data Flow

### Login Flow
```
LoginComponent
    ↓ (form submit)
AuthService.login()
    ↓ (HTTP POST /api/Auth/Login)
Backend API
    ↓ (JWT token response)
localStorage (store token)
    ↓
Router (redirect to /tickets)
```

### Protected Request Flow
```
User navigates to /tickets
    ↓
AuthGuard checks token
    ↓ (valid)
Load TicketsComponent
    ↓ (HTTP request)
JwtInterceptor adds Authorization header
    ↓
Backend API validates token
    ↓
Response received
```

### Logout Flow
```
AuthService.logout()
    ↓
localStorage.clear()
    ↓
Router (redirect to /auth/login)
```

## State Management

Currently using:
- **localStorage** for token persistence
- **BehaviorSubject** for current user state
- **Signals** for reactive UI updates

No external state management library needed at this stage.
