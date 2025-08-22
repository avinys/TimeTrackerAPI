# TimeTracker API – Configuration & Secrets

## Configuration sources (in order)
1. `appsettings.json` – committed, **placeholders only** (no real secrets)
2. `appsettings.{Environment}.json` – local overrides (optional). `appsettings.Development.json` is **git-ignored**.
3. **User Secrets** (Development) – the recommended way to store local secrets
4. **Environment variables** – recommended for CI/CD and production
