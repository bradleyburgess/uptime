# Uptime

A small self-hosted uptime monitor. It polls a single target on an interval, records the results and outages in SQLite, and serves a status page at `/`.

- If the target is an IP address or hostname, it is checked with ICMP ping.
- Otherwise it is treated as a URL and checked with an HTTP `GET` (any 2xx counts as up).

Built with .NET 10 and published as a Native AOT binary.

## Configuration

Settings are read from `appsettings.json` and environment variables.

| Setting | Env var | Default | Description |
| --- | --- | --- | --- |
| `UrlToPing` | `UrlToPing` | `https://cp.cloudflare.com/generate_204` | URL, hostname or IP to monitor |
| `PollInterval` | `PollInterval` | none | Seconds between checks. Must be set. |
| `ConnectionStrings:Sqlite` | `ConnectionStrings__Sqlite` | `Data Source=/data/app.db` in the image | SQLite connection string |

## Run with Docker

```sh
docker run -d --name uptime \
  -p 8080:8080 \
  -v uptime-data:/data \
  -e PollInterval=60 \
  -e UrlToPing=https://example.com \
  ghcr.io/bradleyburgess/uptime:latest
```

The status page is then at <http://localhost:8080>.

The container runs as a non-root user. If you monitor an IP or hostname (ICMP) and pings fail, add `--cap-add=NET_RAW`.

## Develop

```sh
dotnet run --project Uptime.Web
```

In `Development` the database is `app.db` in the working directory.

Build the image locally:

```sh
docker build -t uptime .
```

## Releasing

Images are built and published by [.github/workflows/docker.yml](.github/workflows/docker.yml) as a multi-arch (`linux/amd64`, `linux/arm64`) image to `ghcr.io/bradleyburgess/uptime`.

| Trigger | Tags published |
| --- | --- |
| Push tag `vX.Y.Z` | `X.Y.Z`, `X.Y`, `sha-<short>` |
| Push tag `vX.Y.Z-rc.1` | `X.Y.Z-rc.1`, `sha-<short>` |

To cut a release:

1. Make sure `main` is green and contains what you want to ship.
2. Tag the commit and push the tag:

   ```sh
   git checkout main && git pull
   git tag v1.2.3
   git push origin v1.2.3
   ```

3. Watch the **Docker** workflow in the Actions tab. When both architecture builds and the `merge` job finish, the tags above are live.
4. Verify:

   ```sh
   docker buildx imagetools inspect ghcr.io/bradleyburgess/uptime:1.2.3
   ```

5. Optionally create a GitHub Release from the tag.

Notes:

- Tags must start with `v` and follow semver; anything else will not trigger a release build.
- `latest` only moves on pushes to `main`, not on tag pushes.
- To fix a bad release, push a new patch tag rather than moving the existing one.
- On first publish the package is private; set it to public in the package settings on GitHub to allow unauthenticated pulls.
