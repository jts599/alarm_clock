# CI/CD Setup Guide

This guide explains how to set up Continuous Integration and Continuous Deployment (CI/CD) for the alarm clock project using GitHub Actions with separate `main` and `dev` branches.

## Table of Contents

1. [Overview](#overview)
2. [Branch Strategy](#branch-strategy)
3. [GitHub Secrets Setup](#github-secrets-setup)
4. [Workflow Descriptions](#workflow-descriptions)
5. [Development Workflow](#development-workflow)
6. [Raspberry Pi Setup for Multi-Environment](#raspberry-pi-setup-for-multi-environment)
7. [Testing the CI/CD Pipeline](#testing-the-cicd-pipeline)
8. [Monitoring and Troubleshooting](#monitoring-and-troubleshooting)

---

## Overview

The CI/CD pipeline automatically:

- ✅ Tests and builds code on every pull request
- ✅ Publishes Docker images to Docker Hub when code is pushed to `main` or `dev`
- ✅ Creates GitHub releases when version tags are pushed
- ✅ Builds multi-architecture images (AMD64 and ARM64 for Raspberry Pi)
- ✅ Enables automatic updates via Watchtower

### Workflow Files

- **`.github/workflows/ci.yml`** - Runs tests on PRs and pushes
- **`.github/workflows/deploy.yml`** - Builds and publishes Docker images
- **`.github/workflows/release.yml`** - Creates releases for version tags

---

## Branch Strategy

### Main Branch (`main`)

- **Purpose**: Production-ready code
- **Deployment**: Automatic on every push
- **Docker Tags**: `latest` and `main`
- **Pi Configuration**: Production environment (port 3000, 5000)
- **Use Case**: Stable releases running on your primary Raspberry Pi

### Dev Branch (`dev`)

- **Purpose**: Development and testing
- **Deployment**: Automatic on every push
- **Docker Tags**: `dev` and `dev-<commit-sha>`
- **Pi Configuration**: Development environment (port 3001, 5001)
- **Use Case**: Testing new features before promoting to main

### Feature Branches

- **Naming**: `feature/<feature-name>` or `fix/<bug-name>`
- **Deployment**: None (only CI tests run)
- **Merge Target**: `dev` branch
- **Use Case**: Active development work

---

## GitHub Secrets Setup

You need to configure secrets in your GitHub repository for the workflows to function.

### Step 1: Create Docker Hub Access Token

1. Go to https://hub.docker.com/settings/security
2. Click **New Access Token**
3. Name it: `github-actions-alarm-clock`
4. Permissions: **Read, Write, Delete**
5. Click **Generate**
6. **Copy the token immediately** (you won't see it again)

### Step 2: Add Secrets to GitHub

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**

Add these two secrets:

| Name              | Value                        | Description       |
| ----------------- | ---------------------------- | ----------------- |
| `DOCKER_USERNAME` | Your Docker Hub username     | e.g., `jsmith`    |
| `DOCKER_PASSWORD` | Your Docker Hub access token | Token from Step 1 |

### Verification

After adding secrets, they should appear in your repository settings. You won't be able to view the values, only edit or delete them.

---

## Workflow Descriptions

### 1. CI Workflow (`ci.yml`)

**Triggers:**

- Pull requests to `main` or `dev`
- Pushes to `main` or `dev`

**Jobs:**

1. **test-backend**: Builds and tests .NET backend
2. **test-frontend**: Lints, type-checks, and builds React frontend
3. **build-docker-images**: Builds Docker images without pushing (validation)

**Purpose:** Ensure code quality before merging or deploying.

### 2. Deploy Workflow (`deploy.yml`)

**Triggers:**

- Pushes to `main` or `dev` branches
- Manual workflow dispatch

**Jobs:**

1. **build-and-push**:
   - Builds multi-architecture images (AMD64 + ARM64)
   - Pushes to Docker Hub with appropriate tags
   - Generates deployment summary

**Image Tags Produced:**

For `main` branch:

```
your-username/alarm-clock-backend:latest
your-username/alarm-clock-backend:main
your-username/alarm-clock-backend:main-<sha>
```

For `dev` branch:

```
your-username/alarm-clock-backend:dev
your-username/alarm-clock-backend:dev-<sha>
```

### 3. Release Workflow (`release.yml`)

**Triggers:**

- Version tags pushed (e.g., `v1.0.0`, `v1.2.3`)

**Jobs:**

1. **release**:
   - Builds and pushes images with version tag
   - Creates GitHub release with changelog
   - Tags images as both `latest` and version number

**Creating a Release:**

```bash
# Tag your commit
git tag -a v1.0.0 -m "Release version 1.0.0"

# Push the tag
git push origin v1.0.0
```

This will trigger the release workflow and create a GitHub release page.

---

## Development Workflow

### Standard Development Flow

```
feature branch → dev branch → main branch → release tag
```

### Step-by-Step

#### 1. Create Feature Branch

```bash
# Start from dev
git checkout dev
git pull origin dev

# Create feature branch
git checkout -b feature/my-new-feature
```

#### 2. Develop and Test Locally

```bash
# Make changes
# Test locally with dev containers or Docker Compose

# Commit changes
git add .
git commit -m "feat: add new feature"
```

#### 3. Push and Create PR to Dev

```bash
# Push feature branch
git push origin feature/my-new-feature

# Create PR on GitHub targeting 'dev' branch
# CI workflow will run tests automatically
```

#### 4. Merge to Dev

```bash
# After PR approval, merge to dev
# Deploy workflow automatically publishes :dev images
# Watchtower updates dev Pi within 5 minutes
```

#### 5. Test on Dev Environment

- Access dev Pi at `http://<pi-ip>:3001` (frontend) and `http://<pi-ip>:5001` (backend)
- Verify functionality
- Test for stability

#### 6. Promote to Main

```bash
# Create PR from dev to main
# CI workflow runs tests

# After approval, merge to main
# Deploy workflow automatically publishes :latest images
# Watchtower updates production Pi within 5 minutes
```

#### 7. Create Release (Optional)

```bash
# Tag the main branch
git checkout main
git pull origin main
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin v1.0.0

# Release workflow creates GitHub release
```

---

## Raspberry Pi Setup for Multi-Environment

### Option 1: Single Pi with Both Environments

Run both production and dev environments on one Pi:

```bash
ssh pi@raspberrypi.local
cd /opt

# Production environment
sudo mkdir -p /opt/alarm-clock-prod
sudo chown $USER:$USER /opt/alarm-clock-prod
cd /opt/alarm-clock-prod
mkdir -p models shared logs

# Dev environment
sudo mkdir -p /opt/alarm-clock-dev
sudo chown $USER:$USER /opt/alarm-clock-dev
cd /opt/alarm-clock-dev
mkdir -p models shared logs
```

#### Production Setup

```bash
cd /opt/alarm-clock-prod

# Copy production compose file
# On dev machine:
scp docker-compose.prod.yml pi@raspberrypi.local:/opt/alarm-clock-prod/docker-compose.yml

# On Pi, create .env
cat > .env << 'EOF'
DOCKER_USERNAME=your-dockerhub-username
WEATHER_LONGITUDE=-74.0060
WEATHER_LATITUDE=40.7128
STUB_LIFX=false
FAST_TIMESCALE=false
EOF

# Create systemd service
sudo nano /etc/systemd/system/alarm-clock-prod.service
```

Add:

```ini
[Unit]
Description=Alarm Clock Production
Requires=docker.service
After=docker.service network-online.target
Wants=network-online.target

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/opt/alarm-clock-prod
ExecStart=/usr/bin/docker compose up -d
ExecStop=/usr/bin/docker compose down
TimeoutStartSec=0
Restart=on-failure
RestartSec=10s

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl daemon-reload
sudo systemctl enable alarm-clock-prod.service
sudo systemctl start alarm-clock-prod.service
```

#### Dev Setup

```bash
cd /opt/alarm-clock-dev

# Copy dev compose file
# On dev machine:
scp docker-compose.dev.yml pi@raspberrypi.local:/opt/alarm-clock-dev/docker-compose.yml

# On Pi, create .env
cat > .env << 'EOF'
DOCKER_USERNAME=your-dockerhub-username
WEATHER_LONGITUDE=-74.0060
WEATHER_LATITUDE=40.7128
STUB_LIFX=true
FAST_TIMESCALE=true
EOF

# Create systemd service
sudo nano /etc/systemd/system/alarm-clock-dev.service
```

Add:

```ini
[Unit]
Description=Alarm Clock Development
Requires=docker.service
After=docker.service network-online.target
Wants=network-online.target

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/opt/alarm-clock-dev
ExecStart=/usr/bin/docker compose up -d
ExecStop=/usr/bin/docker compose down
TimeoutStartSec=0
Restart=on-failure
RestartSec=10s

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl daemon-reload
sudo systemctl enable alarm-clock-dev.service
sudo systemctl start alarm-clock-dev.service
```

### Option 2: Separate Pis for Each Environment

Use two different Raspberry Pis:

- **Pi 1**: Production (uses `docker-compose.prod.yml`)
- **Pi 2**: Development (uses `docker-compose.dev.yml`)

Follow the same setup as Option 1, but only install one environment per Pi.

---

## Testing the CI/CD Pipeline

### Test 1: Verify CI on Feature Branch

```bash
# Create test feature branch
git checkout -b feature/ci-test
echo "# CI Test" >> README.md
git add README.md
git commit -m "test: CI pipeline"
git push origin feature/ci-test

# Create PR to dev on GitHub
# Watch Actions tab - CI workflow should run
```

### Test 2: Deploy to Dev

```bash
# Merge PR to dev or push directly
git checkout dev
git merge feature/ci-test
git push origin dev

# Watch GitHub Actions - deploy workflow should run
# Check Docker Hub for new :dev images
# Within 5 minutes, dev Pi should update
```

### Test 3: Deploy to Production

```bash
# Create PR from dev to main
# After approval, merge
git checkout main
git merge dev
git push origin main

# Watch GitHub Actions - deploy workflow should run
# Check Docker Hub for new :latest images
# Within 5 minutes, production Pi should update
```

### Test 4: Create Release

```bash
git checkout main
git pull origin main
git tag -a v1.0.0 -m "First release"
git push origin v1.0.0

# Watch GitHub Actions - release workflow should run
# Check GitHub Releases page
# Check Docker Hub for :v1.0.0 tags
```

---

## Monitoring and Troubleshooting

### GitHub Actions

**View Workflows:**

1. Go to your GitHub repository
2. Click **Actions** tab
3. Click on a workflow run to see details

**Common Issues:**

#### Docker Login Fails

```
Error: Could not authenticate to Docker Hub
```

**Solution:** Check that `DOCKER_USERNAME` and `DOCKER_PASSWORD` secrets are set correctly.

#### Build Fails

```
Error: Build failed for platform linux/arm64
```

**Solution:** Check Dockerfile syntax and dependencies. Test locally with:

```bash
docker buildx build --platform linux/arm64 -f backend/Dockerfile backend/
```

#### Submodules Not Initialized

```
Error: LifxNet source not found
```

**Solution:** Workflow includes `submodules: recursive`. Verify `.gitmodules` is correct.

### Raspberry Pi

**Check Container Status:**

```bash
# Production
docker ps | grep alarm-clock

# Dev
docker ps | grep alarm-clock-dev

# View logs
docker logs alarm-clock-backend -f
docker logs watchtower -f
```

**Verify Watchtower Updates:**

```bash
# Check Watchtower logs for update activity
docker logs watchtower --tail 100

# Should see messages like:
# "Found new image for alarm-clock-backend:latest"
# "Stopping alarm-clock-backend"
# "Starting alarm-clock-backend with new image"
```

**Manual Image Update:**

```bash
cd /opt/alarm-clock-prod
docker compose pull
docker compose up -d
```

**Check Systemd Services:**

```bash
# Status
sudo systemctl status alarm-clock-prod.service
sudo systemctl status alarm-clock-dev.service

# Logs
journalctl -u alarm-clock-prod.service -f
```

### Docker Hub

**Verify Images Published:**

1. Go to https://hub.docker.com
2. Navigate to your repositories
3. Check for:
   - `alarm-clock-backend:latest`
   - `alarm-clock-backend:dev`
   - `alarm-clock-frontend:latest`
   - `alarm-clock-frontend:dev`

**Check Image Architectures:**

- Click on an image
- Look for `linux/amd64` and `linux/arm64` in the tag details

---

## Best Practices

### 1. Commit Messages

Use conventional commit format:

```
feat: add new alarm sound
fix: resolve LIFX connection timeout
docs: update deployment guide
chore: update dependencies
```

### 2. Pull Requests

- Always create PRs for code review
- Target `dev` branch first, not `main`
- Wait for CI to pass before merging
- Add descriptive PR descriptions

### 3. Testing Before Merge

- Test feature branches locally
- Test on dev environment before promoting to main
- Run manual tests on Pi after auto-deployment

### 4. Version Tags

Use semantic versioning:

- `v1.0.0` - Major release
- `v1.1.0` - Minor feature addition
- `v1.0.1` - Bug fix/patch

### 5. Monitoring

- Check GitHub Actions after each push
- Monitor Watchtower logs on Pi
- Set up notifications for failed workflows (GitHub Settings → Notifications)

---

## Quick Reference Commands

### Development

```bash
# Start feature
git checkout -b feature/my-feature

# Push and create PR to dev
git push origin feature/my-feature

# After merge to dev, check deployment
# (wait 5 minutes for Watchtower)
```

### Deployment

```bash
# Deploy to dev (automatic on push)
git push origin dev

# Deploy to production (automatic on push)
git push origin main

# Create release
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin v1.0.0
```

### Monitoring

```bash
# Check GitHub Actions
# Visit: https://github.com/jts599/alarm_clock/actions

# Check Docker Hub
# Visit: https://hub.docker.com/u/your-username

# Check Pi logs
ssh pi@raspberrypi.local
docker logs watchtower -f
docker compose logs -f
```

---

## Summary

Your CI/CD pipeline is now configured to:

✅ **Automatically test** every pull request
✅ **Automatically build and publish** Docker images on push to `main` or `dev`
✅ **Support multi-environment** deployment (production and development)
✅ **Enable automatic updates** via Watchtower on Raspberry Pi
✅ **Create releases** with version tags
✅ **Build for multiple architectures** (AMD64 and ARM64)

The workflow enables rapid, safe deployment with proper testing gates and environment separation!
