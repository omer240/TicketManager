# Ticket Manager Frontend

Angular 20 frontend application for the Ticket Manager system.

## Features Implemented (Phase 1)

✅ **Authentication System**
- Login page with form validation
- Register page with form validation
- JWT token management
- Token storage in localStorage
- Auth guard for protected routes
- JWT interceptor for automatic token attachment
- Token expiry checking
- Automatic logout on 401

## Project Structure

```
src/
├── app/
│   ├── core/
│   │   ├── guards/
│   │   │   └── auth.guard.ts           # Route protection
│   │   ├── interceptors/
│   │   │   └── jwt.interceptor.ts      # JWT token attachment
│   │   ├── models/
│   │   │   └── auth.models.ts          # Auth TypeScript interfaces
│   │   └── services/
│   │       └── auth.service.ts         # Authentication service
│   ├── features/
│   │   ├── auth/
│   │   │   ├── login/                  # Login component
│   │   │   └── register/               # Register component
│   │   └── tickets/                    # Tickets feature (placeholder)
│   ├── app.config.ts                   # App configuration
│   └── app.routes.ts                   # Route definitions
├── environments/
│   ├── environment.ts                  # Development environment
│   └── environment.prod.ts             # Production environment
└── proxy.conf.json                     # Dev proxy configuration
```

## Backend API Integration

The frontend connects to the backend API running on `http://localhost:5000`.

### Authentication Endpoints

**Login**: `POST /api/Auth/Login`
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Register**: `POST /api/Auth/Register`
```json
{
  "email": "user@example.com",
  "fullName": "John Doe",
  "password": "password123"
}
```

### Response Format
All API responses follow this structure:
```typescript
{
  success: boolean;
  data?: T;
  error?: {
    code: string;
    message: string;
  }
}
```

### Authentication
- JWT tokens are stored in localStorage
- Tokens expire after 120 minutes (2 hours)
- All protected endpoints require: `Authorization: Bearer <token>`
- 401 responses trigger automatic logout

## Development

### Prerequisites
- Node.js (v18+)
- Angular CLI 20
- Backend API running on port 5000

### Install Dependencies
```bash
npm install
```

### Start Development Server
```bash
npm start
# or
ng serve
```

The application will be available at `http://localhost:4200`

### Build for Production
```bash
npm run build
# or
ng build --configuration production
```

## Routes

| Path | Protected | Description |
|------|-----------|-------------|
| `/auth/login` | No | Login page |
| `/auth/register` | No | Registration page |
| `/tickets` | Yes | Tickets list (placeholder) |
| `/` | - | Redirects to `/tickets` |

## Configuration

### Environment Variables
Edit `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiBaseUrl: 'http://localhost:5000'  // Backend API URL
};
```

### Proxy Configuration
The dev server proxies `/api` requests to the backend. See `proxy.conf.json`:
```json
{
  "/api": {
    "target": "http://localhost:5000",
    "secure": false,
    "changeOrigin": true
  }
}
```

## Tech Stack

- **Angular 20** - Framework
- **TypeScript** - Language
- **RxJS** - Reactive programming
- **SCSS** - Styling
- **Standalone Components** - Modern Angular architecture
- **Reactive Forms** - Form handling
- **Signals** - State management

## Next Steps (Phase 2+)

The following features need to be implemented:
- [ ] Tickets list page with filtering/search
- [ ] Ticket creation form
- [ ] Ticket detail page
- [ ] Ticket editing
- [ ] Status update functionality
- [ ] Assignee management
- [ ] Comments feature
- [ ] Pagination
- [ ] Error handling improvements
- [ ] Loading states
- [ ] Toast notifications

## Notes

- Backend must be running on port 5000
- All backend endpoints except auth require authentication
- Tokens are automatically included in requests via interceptor
- Form validation matches backend requirements
