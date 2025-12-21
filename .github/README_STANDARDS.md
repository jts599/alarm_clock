# README Standards

This document defines the README standards for the Alarm Clock project. Use this as a reference when creating or updating README files.

## General Principles

- **Brevity**: Keep READMEs short and scannable - focus on the big picture
- **Hierarchy**: Different levels of the project need different levels of detail
- **Navigability**: Help readers quickly find what they need
- **Actionable**: Include practical information, not exhaustive documentation
- **Table of Contents**: Use `[toc]` for longer READMEs (>5 sections)

## README Hierarchy

### Root Project README

**Purpose**: Give newcomers the 30,000-foot view

**Must Include**:

- One-line project description
- Core technologies (frontend/backend stack)
- Key functionality/features (bullet points)
- How to run (docker/dev setup)
- Links to subdirectory READMEs

**Structure**:

````markdown
# Project Name

One sentence describing what this project does.

## Tech Stack

- **Frontend**: React/Vue/Angular + key libraries
- **Backend**: .NET/Node/Python + framework
- **Infrastructure**: Docker, databases, etc.

## Features

- Feature 1 (what it does, not how)
- Feature 2
- Feature 3

## Quick Start

```bash
# Development (uses :dev image tag)
IMAGE_TAG=dev docker-compose up

# Production (uses :latest image tag)
IMAGE_TAG=latest docker-compose up

# Or run dev on separate ports
docker-compose -f docker-compose.dev.yml up
```
````

## Project Structure

- `/frontend` - [Brief description](./frontend/README.md)
- `/backend` - [Brief description](./backend/README.md)

## Documentation

- [Setup Guide](./SETUP_GUIDE.md)
- [API Documentation](./API_DOCS.md)

````

### Service/Module Directory README

**Purpose**: Explain the service's role and public APIs

**Must Include**:
- What this service/module does (2-3 sentences)
- Public interfaces/APIs
- Implementation classes (one-line descriptions)
- Consumers (which controllers/services use this)
- Configuration (if applicable)

**Structure**:
```markdown
# Service Name

Brief description of what this service does and why it exists.

## Public API

### IServiceName (Interface)

```csharp
int GetSomething();
void SetSomething(int value);
````

See [IServiceName.cs](./IServiceName.cs) for full documentation.

## Implementations

- **ConcreteService** - Production implementation using [technology]
- **MockService** - Development/testing mock (enabled via `AppSettings.StubFeature`)

## Usage

Used by:

- `SomeController` - [Brief purpose]
- `OtherService` - [Brief purpose]

## Configuration

```json
{
  "RunConfiguration": {
    "StubFeature": true
  }
}
```

## Related

- [Related Service](../RelatedService/README.md)
- [Configuration Guide](../../docs/CONFIGURATION.md)

````

### Component/Feature Directory README

**Purpose**: Explain a specific feature or component

**Must Include**:
- Feature description (1-2 sentences)
- Key files and their purposes
- How it integrates with the rest of the system

**Structure**:
```markdown
# Feature Name

What this feature does and its role in the application.

## Key Files

- `MainComponent.tsx` - Primary component, handles [X]
- `helper.ts` - Utility functions for [Y]
- `types.ts` - Type definitions

## Integration

- Used in: `ParentComponent`
- Depends on: `SomeService`, `SomeContext`

## Example Usage

```typescript
<FeatureName config={...} onComplete={...} />
````

```

## What NOT to Include

❌ Exhaustive API documentation (use code comments for that)
❌ Implementation details (use code comments for that)
❌ Change history (use git/changelog for that)
❌ Troubleshooting guides (use separate docs for that)
❌ Step-by-step tutorials (use separate guides for that)

## Length Guidelines

- **Root README**: 100-200 lines max
- **Service/Module README**: 50-100 lines
- **Component README**: 30-50 lines

If your README exceeds these, consider:
1. Moving detailed guides to separate docs
2. Linking to external documentation
3. Using collapsible sections
4. Splitting into multiple focused READMEs

## AI Assistant Prompt Template

When asking an AI to create a README, use this template:

```

Create a README for [directory/service/module] following these standards:

**Context**: This is a [root/service/component] level README.

**Requirements**:

1. Keep it brief and scannable (target [X] lines based on level)
2. Focus on the big picture, not implementation details
3. Include:
   - [For service] What it does, public APIs, implementations, consumers
   - [For root] Tech stack, features, quick start, project structure
   - [For component] Feature description, key files, integration points
4. Use bullet points and short paragraphs
5. Link to detailed documentation rather than including it
6. Use code blocks for API signatures and configuration examples
7. Add `[toc]` if the README has more than 5 sections

**Tone**: Direct and factual. Start descriptions with verbs.

**Format**: Markdown with clear section headers.

````

## Examples by Type

### Good Root README Example
```markdown
# Alarm Clock

Smart alarm clock with sunrise simulation and weather integration for Raspberry Pi.

## Tech Stack

- **Frontend**: React + TypeScript + Vite
- **Backend**: .NET 8 + ASP.NET Core
- **Hardware**: Raspberry Pi, LIFX bulbs, touchscreen

## Features

- Configurable sunrise simulation with color transitions
- Weather-aware alarm adjustments
- Touch-friendly UI with screen brightness control
- Docker-based deployment

## Quick Start

```bash
docker-compose up
````

Access at `http://localhost:3000`

## Project Structure

- `/frontend` - React UI ([details](./frontend/README.md))
- `/backend` - .NET API ([details](./backend/README.md))

````

### Good Service README Example
```markdown
# Brightness Service

Controls Raspberry Pi screen backlight brightness via sysfs interface.

## Public API

```csharp
interface IBrightnessService {
    int GetBrightness();
    int GetMaxBrightness();
    void SetBrightness(int brightness);
}
````

Extensions provide percentage-based control (0-100).

## Implementations

- **RaspberryPiBrightnessService** - Reads/writes `/sys/class/backlight/*/brightness`
- **MockBrightnessService** - In-memory mock for development

Toggle via `RunConfiguration.StubBrightness` in appsettings.

## Usage

- `BrightnessController` - REST API endpoints
- Docker requires `/sys/class/backlight` mount

````

### Good Component README Example
```markdown
# Weather Component

Displays current conditions and forecast from National Weather Service API.

## Key Files

- `WeatherComponent.tsx` - Main display component
- `useWeatherForecast.ts` - Data fetching hook
- `WeatherClient.ts` - API client

## Integration

- Used in: Main activity view
- Context: None (self-contained)
- Updates: Every 30 minutes
````

## Maintenance Checklist

When creating/updating a README:

- [ ] Appropriate length for directory level
- [ ] Clear one-line description at top
- [ ] Big picture focus (no implementation details)
- [ ] Links to detailed docs instead of including them
- [ ] Code examples are minimal and illustrative
- [ ] No redundant information from code comments
- [ ] Structure matches the template for this level
- [ ] Uses bullet points and short paragraphs
- [ ] Includes `[toc]` if longer than 5 sections

## Quick Reference

| Level     | Focus               | Length        | Key Sections                            |
| --------- | ------------------- | ------------- | --------------------------------------- |
| Root      | Project overview    | 100-200 lines | Stack, Features, Quick Start, Structure |
| Service   | Public APIs & usage | 50-100 lines  | API, Implementations, Usage, Config     |
| Component | Integration points  | 30-50 lines   | Description, Key Files, Integration     |

## Related Standards

- [Documentation Standards](./DOCUMENTATION_STANDARDS.md) - For code comments
- [API Documentation](./API_STANDARDS.md) - For detailed API docs
- [Contributing Guide](./CONTRIBUTING.md) - For development guidelines
