# CI/CD Quick Reference

## GitHub Secrets Setup

1. Go to repository **Settings** → **Secrets and variables** → **Actions**
2. Add these secrets:
   - `DOCKER_USERNAME` - Your Docker Hub username
   - `DOCKER_PASSWORD` - Your Docker Hub access token

## Branch Strategy

| Branch      | Purpose      | Auto-Deploy | Docker Tag |
| ----------- | ------------ | ----------- | ---------- |
| `main`      | Production   | ✅ Yes      | `latest`   |
| `dev`       | Development  | ✅ Yes      | `dev`      |
| `feature/*` | Feature work | ❌ No       | N/A        |

## Development Workflow

```bash
# 1. Start new feature from dev
git checkout dev
git pull origin dev
git checkout -b feature/my-feature

# 2. Make changes and commit
git add .
git commit -m "feat: description"
git push origin feature/my-feature

# 3. Create PR to dev on GitHub
# CI tests will run automatically

# 4. After merge to dev
# - Deploy workflow runs automatically
# - Images tagged as :dev
# - Dev Raspberry Pi updates in ~5 min

# 5. Test on dev environment
# Access at http://<dev-pi-ip>:3001

# 6. Create PR from dev to main
# After merge:
# - Deploy workflow runs automatically
# - Images tagged as :latest
# - Production Raspberry Pi updates in ~5 min

# 7. Optional: Create release
git checkout main
git pull origin main
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin v1.0.0
# Creates GitHub release and version-tagged images
```

## Quick Deploy Script

```bash
# Build and push manually (multi-arch)
./scripts/deploy.sh              # tags as :latest
./scripts/deploy.sh dev          # tags as :dev
./scripts/deploy.sh v1.0.0       # tags as :v1.0.0
```

## Raspberry Pi Commands

```bash
# Check status
docker ps
docker compose logs -f
docker logs watchtower -f

# Check for updates manually
docker compose pull
docker compose up -d

# Restart services
sudo systemctl restart alarm-clock-prod.service
sudo systemctl restart alarm-clock-dev.service

# View systemd logs
journalctl -u alarm-clock-prod.service -f
```

## GitHub Actions

**View workflows:** https://github.com/jts599/alarm_clock/actions

**Workflows:**

- `ci.yml` - Runs tests on PRs and pushes
- `deploy.yml` - Builds and publishes images
- `release.yml` - Creates releases from tags

**Manual trigger:**

1. Go to Actions tab
2. Select "Deploy - Build and Publish Docker Images"
3. Click "Run workflow"
4. Select branch
5. Click "Run workflow"

## Docker Hub

**View images:** https://hub.docker.com/u/your-username

**Expected images:**

- `your-username/alarm-clock-backend:latest`
- `your-username/alarm-clock-backend:dev`
- `your-username/alarm-clock-frontend:latest`
- `your-username/alarm-clock-frontend:dev`

## Troubleshooting

| Issue               | Solution                                                                                  |
| ------------------- | ----------------------------------------------------------------------------------------- |
| CI tests fail       | Check GitHub Actions logs for details                                                     |
| Deploy fails        | Verify GitHub secrets are set correctly                                                   |
| Pi not updating     | Check `docker logs watchtower`, ensure images pushed to Docker Hub                        |
| Wrong image version | Check docker-compose.yml uses correct image name/tag                                      |
| Build fails         | Test locally: `docker buildx build --platform linux/arm64 -f backend/Dockerfile backend/` |

## Environment Ports

| Environment | Frontend | Backend | Location                |
| ----------- | -------- | ------- | ----------------------- |
| Production  | 3000     | 5000    | `/opt/alarm-clock-prod` |
| Development | 3001     | 5001    | `/opt/alarm-clock-dev`  |

## Useful Links

- [Full Deployment Guide](DEPLOYMENT_GUIDE.md)
- [Complete CI/CD Setup](CI_CD_SETUP.md)
- [Main README](README.md)
